﻿using System; // require keep for Windows Universal App
using UnityEngine;

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
using UnityEngine.EventSystems;
#endif

namespace UniRx.Triggers
{
    // for Component
    public static partial class ObservableTriggerExtensions
    {
        #region ObservableAnimatorTrigger

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public static IObservable<int> OnAnimatorIKAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<int>();
            return GetOrAddComponent<ObservableAnimatorTrigger>(component.gameObject).OnAnimatorIKAsObservable();
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public static IObservable<Unit> OnAnimatorMoveAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableAnimatorTrigger>(component.gameObject).OnAnimatorMoveAsObservable();
        }

        #endregion

        #region ObservableDestroyTrigger

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public static IObservable<Unit> OnDestroyAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Return(Unit.Default); // send destroy message
            return GetOrAddComponent<ObservableDestroyTrigger>(component.gameObject).OnDestroyAsObservable();
        }

        #endregion


        #region ObservableEnableTrigger

        /// <summary>This function is called when the object becomes enabled and active.</summary>
        public static IObservable<Unit> OnEnableAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableEnableTrigger>(component.gameObject).OnEnableAsObservable();
        }

        /// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
        public static IObservable<Unit> OnDisableAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableEnableTrigger>(component.gameObject).OnDisableAsObservable();
        }

        #endregion

        #region ObservableFixedUpdateTrigger

        /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
        public static IObservable<Unit> FixedUpdateAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableFixedUpdateTrigger>(component.gameObject).FixedUpdateAsObservable();
        }

        #endregion

        #region ObservableLateUpdateTrigger

        /// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
        public static IObservable<Unit> LateUpdateAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableLateUpdateTrigger>(component.gameObject).LateUpdateAsObservable();
        }

        #endregion

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

        #region ObservableMouseTrigger

        /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseDownAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseDownAsObservable();
        }

        /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
        public static IObservable<Unit> OnMouseDragAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseDragAsObservable();
        }

        /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseEnterAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseEnterAsObservable();
        }

        /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseExitAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseExitAsObservable();
        }

        /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseOverAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseOverAsObservable();
        }

        /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
        public static IObservable<Unit> OnMouseUpAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseUpAsObservable();
        }

        /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
        public static IObservable<Unit> OnMouseUpAsButtonAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(component.gameObject).OnMouseUpAsButtonAsObservable();
        }

        #endregion

#endif

        #region ObservableUpdateTrigger

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        public static IObservable<Unit> UpdateAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableUpdateTrigger>(component.gameObject).UpdateAsObservable();
        }

        #endregion

        #region ObservableVisibleTrigger

        /// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
        public static IObservable<Unit> OnBecameInvisibleAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableVisibleTrigger>(component.gameObject).OnBecameInvisibleAsObservable();
        }

        /// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
        public static IObservable<Unit> OnBecameVisibleAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableVisibleTrigger>(component.gameObject).OnBecameVisibleAsObservable();
        }

        #endregion

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        #region ObservableTransformChangedTrigger

        /// <summary>Callback sent to the graphic before a Transform parent change occurs.</summary>
        public static IObservable<Unit> OnBeforeTransformParentChangedAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTransformChangedTrigger>(component.gameObject).OnBeforeTransformParentChangedAsObservable();
        }

        /// <summary>This function is called when the parent property of the transform of the GameObject has changed.</summary>
        public static IObservable<Unit> OnTransformParentChangedAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTransformChangedTrigger>(component.gameObject).OnTransformParentChangedAsObservable();
        }

        /// <summary>This function is called when the list of children of the transform of the GameObject has changed.</summary>
        public static IObservable<Unit> OnTransformChildrenChangedAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTransformChangedTrigger>(component.gameObject).OnTransformChildrenChangedAsObservable();
        }

        #endregion

        #region ObservableCanvasGroupChangedTrigger

        /// <summary>Callback that is sent if the canvas group is changed.</summary>
        public static IObservable<Unit> OnCanvasGroupChangedAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableCanvasGroupChangedTrigger>(component.gameObject).OnCanvasGroupChangedAsObservable();
        }

        #endregion

        #region ObservableRectTransformTrigger

        /// <summary>Callback that is sent if an associated RectTransform has it's dimensions changed.</summary>
        public static IObservable<Unit> OnRectTransformDimensionsChangeAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableRectTransformTrigger>(component.gameObject).OnRectTransformDimensionsChangeAsObservable();
        }

        /// <summary>Callback that is sent if an associated RectTransform is removed.</summary>
        public static IObservable<Unit> OnRectTransformRemovedAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableRectTransformTrigger>(component.gameObject).OnRectTransformRemovedAsObservable();
        }

        #endregion

        // uGUI

        #region ObservableEventTrigger classes

        public static IObservable<BaseEventData> OnDeselectAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<BaseEventData>();
            return GetOrAddComponent<ObservableDeselectTrigger>(component.gameObject).OnDeselectAsObservable();
        }

        public static IObservable<PointerEventData> OnPointerDownAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservablePointerDownTrigger>(component.gameObject).OnPointerDownAsObservable();
        }

        public static IObservable<PointerEventData> OnPointerEnterAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservablePointerEnterTrigger>(component.gameObject).OnPointerEnterAsObservable();
        }

        public static IObservable<PointerEventData> OnPointerExitAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservablePointerExitTrigger>(component.gameObject).OnPointerExitAsObservable();
        }

        public static IObservable<PointerEventData> OnPointerUpAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservablePointerUpTrigger>(component.gameObject).OnPointerUpAsObservable();
        }

        public static IObservable<BaseEventData> OnSelectAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<BaseEventData>();
            return GetOrAddComponent<ObservableSelectTrigger>(component.gameObject).OnSelectAsObservable();
        }

        public static IObservable<PointerEventData> OnPointerClickAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservablePointerClickTrigger>(component.gameObject).OnPointerClickAsObservable();
        }

        public static IObservable<PointerEventData> OnDragAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservableDragTrigger>(component.gameObject).OnDragAsObservable();
        }

        public static IObservable<PointerEventData> OnBeginDragAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservableBeginDragTrigger>(component.gameObject).OnBeginDragAsObservable();
        }

        public static IObservable<PointerEventData> OnEndDragAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservableEndDragTrigger>(component.gameObject).OnEndDragAsObservable();
        }

        public static IObservable<PointerEventData> OnDropAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservableDropTrigger>(component.gameObject).OnDropAsObservable();
        }

        public static IObservable<BaseEventData> OnUpdateSelectedAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<BaseEventData>();
            return GetOrAddComponent<ObservableUpdateSelectedTrigger>(component.gameObject).OnUpdateSelectedAsObservable();
        }

        public static IObservable<PointerEventData> OnInitializePotentialDragAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservableInitializePotentialDragTrigger>(component.gameObject).OnInitializePotentialDragAsObservable();
        }

        public static IObservable<PointerEventData> OnScrollAsObservable(this UIBehaviour component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
            return GetOrAddComponent<ObservableScrollTrigger>(component.gameObject).OnScrollAsObservable();
        }

        #endregion

#endif
    }
}