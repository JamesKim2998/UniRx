#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define SupportCustomYieldInstruction
#endif

using System;
using System.Collections;
using UniRx.InternalUtil;
using UnityEngine;

namespace UniRx
{
    public sealed class MainThreadDispatcher : MonoBehaviour
    {
        public enum CullingMode
        {
            /// <summary>
            /// Won't remove any MainThreadDispatchers.
            /// </summary>
            Disabled,

            /// <summary>
            /// Checks if there is an existing MainThreadDispatcher on Awake(). If so, the new dispatcher removes itself.
            /// </summary>
            Self,

            /// <summary>
            /// Search for excess MainThreadDispatchers and removes them all on Awake().
            /// </summary>
            All
        }

        public static CullingMode cullingMode = CullingMode.Self;

#if UNITY_EDITOR

        // In UnityEditor's EditorMode can't instantiate and work MonoBehaviour.Update.
        // EditorThreadDispatcher use EditorApplication.update instead of MonoBehaviour.Update.
        class EditorThreadDispatcher
        {
            static object gate = new object();
            static EditorThreadDispatcher instance;

            public static EditorThreadDispatcher Instance
            {
                get
                {
                    // Activate EditorThreadDispatcher is dangerous, completely Lazy.
                    lock (gate)
                    {
                        if (instance == null)
                        {
                            instance = new EditorThreadDispatcher();
                        }

                        return instance;
                    }
                }
            }

            ThreadSafeQueueWorker editorQueueWorker = new ThreadSafeQueueWorker();

            EditorThreadDispatcher()
            {
                UnityEditor.EditorApplication.update += Update;
            }

            public void PseudoStartCoroutine(IEnumerator routine)
            {
                editorQueueWorker.Enqueue(_ => ConsumeEnumerator(routine), null);
            }

            void Update()
            {
                editorQueueWorker.ExecuteAll(x => Debug.LogException(x));
            }

            void ConsumeEnumerator(IEnumerator routine)
            {
                if (routine.MoveNext())
                {
                    var current = routine.Current;
                    if (current == null)
                    {
                        goto ENQUEUE;
                    }

                    if (current is IEnumerator)
                    {
                        var enumerator = (IEnumerator)current;
                        editorQueueWorker.Enqueue(_ => ConsumeEnumerator(UnwrapEnumerator(enumerator, routine)), null);
                        return;
                    }

                    ENQUEUE:
                    editorQueueWorker.Enqueue(_ => ConsumeEnumerator(routine), null); // next update
                }
            }

            IEnumerator UnwrapEnumerator(IEnumerator enumerator, IEnumerator continuation)
            {
                while (enumerator.MoveNext())
                {
                    yield return null;
                }
                ConsumeEnumerator(continuation);
            }
        }

#endif

        public static void StartUpdateMicroCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                dispatcher.updateMicroCoroutine.AddCoroutine(routine);
            }
        }

        public static void StartEndOfFrameMicroCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                dispatcher.endOfFrameMicroCoroutine.AddCoroutine(routine);
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return null; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                return (dispatcher as MonoBehaviour).StartCoroutine(routine);
            }
            else
            {
                return null;
            }
        }

        ThreadSafeQueueWorker queueWorker = new ThreadSafeQueueWorker();
        Action<Exception> unhandledExceptionCallback = ex => Debug.LogException(ex); // default

        MicroCoroutine updateMicroCoroutine = null;
        MicroCoroutine endOfFrameMicroCoroutine = null;

        static MainThreadDispatcher instance;
        static bool initialized;
        static bool isQuitting = false;

        static MainThreadDispatcher Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        public static void Initialize()
        {
            if (!initialized)
            {
#if UNITY_EDITOR
                // Don't try to add a GameObject when the scene is not playing. Only valid in the Editor, EditorView.
                if (!ScenePlaybackDetector.IsPlaying) return;
#endif
                MainThreadDispatcher dispatcher = null;

                try
                {
                    dispatcher = GameObject.FindObjectOfType<MainThreadDispatcher>();
                }
                catch
                {
                    // Throw exception when calling from a worker thread.
                    var ex = new Exception("UniRx requires a MainThreadDispatcher component created on the main thread. Make sure it is added to the scene before calling UniRx from a worker thread.");
                    UnityEngine.Debug.LogException(ex);
                    throw ex;
                }

                if (isQuitting)
                {
                    // don't create new instance after quitting
                    // avoid "Some objects were not cleaned up when closing the scene find target" error.
                    return;
                }

                if (dispatcher == null)
                {
                    // awake call immediately from UnityEngine
                    new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                }
                else
                {
                    dispatcher.Awake(); // force awake
                }
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                initialized = true;

                updateMicroCoroutine = new MicroCoroutine(ex => unhandledExceptionCallback(ex));
                endOfFrameMicroCoroutine = new MicroCoroutine(ex => unhandledExceptionCallback(ex));

                StartCoroutine(RunUpdateMicroCoroutine());
                StartCoroutine(RunEndOfFrameMicroCoroutine());

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (this != instance)
                {
                    if (cullingMode == CullingMode.Self)
                    {
                        // Try to destroy this dispatcher if there's already one in the scene.
                        Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Removing myself...");
                        DestroyDispatcher(this);
                    }
                    else if (cullingMode == CullingMode.All)
                    {
                        Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Cleaning up all excess dispatchers...");
                        CullAllExcessDispatchers();
                    }
                    else
                    {
                        Debug.LogWarning("There is already a MainThreadDispatcher in the scene.");
                    }
                }
            }
        }

        IEnumerator RunUpdateMicroCoroutine()
        {
            while (true)
            {
                yield return null;
                updateMicroCoroutine.Run();
            }
        }

        IEnumerator RunEndOfFrameMicroCoroutine()
        {
            while (true)
            {
                yield return YieldInstructionCache.WaitForEndOfFrame;
                endOfFrameMicroCoroutine.Run();
            }
        }

        static void DestroyDispatcher(MainThreadDispatcher aDispatcher)
        {
            if (aDispatcher != instance)
            {
                // Try to remove game object if it's empty
                var components = aDispatcher.gameObject.GetComponents<Component>();
                if (aDispatcher.gameObject.transform.childCount == 0 && components.Length == 2)
                {
                    if (components[0] is Transform && components[1] is MainThreadDispatcher)
                    {
                        Destroy(aDispatcher.gameObject);
                    }
                }
                else
                {
                    // Remove component
                    MonoBehaviour.Destroy(aDispatcher);
                }
            }
        }

        public static void CullAllExcessDispatchers()
        {
            var dispatchers = GameObject.FindObjectsOfType<MainThreadDispatcher>();
            for (int i = 0; i < dispatchers.Length; i++)
            {
                DestroyDispatcher(dispatchers[i]);
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = GameObject.FindObjectOfType<MainThreadDispatcher>();
                initialized = instance != null;
            }
        }

        void Update()
        {
            if (update != null)
            {
                try
                {
                    update.OnNext(Unit.Default);
                }
                catch (Exception ex)
                {
                    unhandledExceptionCallback(ex);
                }
            }
            queueWorker.ExecuteAll(unhandledExceptionCallback);
        }

        // for Lifecycle Management

        Subject<Unit> update;

        public static IObservable<Unit> UpdateAsObservable()
        {
            return Instance.update ?? (Instance.update = new Subject<Unit>());
        }

        Subject<Unit> lateUpdate;

        void LateUpdate()
        {
            if (lateUpdate != null) lateUpdate.OnNext(Unit.Default);
        }

        public static IObservable<Unit> LateUpdateAsObservable()
        {
            return Instance.lateUpdate ?? (Instance.lateUpdate = new Subject<Unit>());
        }

        Subject<bool> onApplicationFocus;

        void OnApplicationFocus(bool focus)
        {
            if (onApplicationFocus != null) onApplicationFocus.OnNext(focus);
        }

        public static IObservable<bool> OnApplicationFocusAsObservable()
        {
            return Instance.onApplicationFocus ?? (Instance.onApplicationFocus = new Subject<bool>());
        }

        Subject<bool> onApplicationPause;

        void OnApplicationPause(bool pause)
        {
            if (onApplicationPause != null) onApplicationPause.OnNext(pause);
        }

        public static IObservable<bool> OnApplicationPauseAsObservable()
        {
            return Instance.onApplicationPause ?? (Instance.onApplicationPause = new Subject<bool>());
        }

        Subject<Unit> onApplicationQuit;

        void OnApplicationQuit()
        {
            isQuitting = true;
            if (onApplicationQuit != null) onApplicationQuit.OnNext(Unit.Default);
        }

        public static IObservable<Unit> OnApplicationQuitAsObservable()
        {
            return Instance.onApplicationQuit ?? (Instance.onApplicationQuit = new Subject<Unit>());
        }
    }
}