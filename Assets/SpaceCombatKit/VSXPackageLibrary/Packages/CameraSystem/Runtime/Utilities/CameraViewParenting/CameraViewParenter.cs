using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.CameraSystem
{   
    /// <summary>
    /// This component changes the parent of specified transform(s) based on the camera view.
    /// </summary>
    public class CameraViewParenter : MonoBehaviour, ICameraEntityUser
    {

        [Tooltip("The camera entity on which camera view changes will trigger the parenting behaviour on this component.")]
        [SerializeField]
        protected CameraEntity cameraEntity;

        [Tooltip("The view (if any) that will be applied on Awake.")]
        [SerializeField]
        protected CameraView initialCameraView;

        [Tooltip("The list of camera view parenting items.")]
        public List<CameraViewParentingItem> cameraViewParentingItems = new List<CameraViewParentingItem>();


        protected virtual void Awake()
        {
            if (initialCameraView != null)
            {
                OnCameraViewChanged(initialCameraView);
            }

            if (cameraEntity != null)
            {
                SetCameraEntity(cameraEntity);
            }
        }


        /// <summary>
        /// Reset the parents to the defaults.
        /// </summary>
        public virtual void ResetToDefault()
        {
            OnCameraViewChanged(initialCameraView);
        }


        /// <summary>
        /// Set the camera entity to get camera view information from.
        /// </summary>
        /// <param name="cameraEntity">The camera entity.</param>
        public virtual void SetCameraEntity(CameraEntity cameraEntity)
        {
            if (this.cameraEntity != null)
            {
                this.cameraEntity.onCameraViewTargetChanged.RemoveListener(OnCameraViewTargetChanged);
            }

            this.cameraEntity = cameraEntity;

            if (cameraEntity != null)
            {
                cameraEntity.onCameraViewTargetChanged.AddListener(OnCameraViewTargetChanged);
            }
        }


        // Called when the camera view changes.
        protected virtual void OnCameraViewTargetChanged(CameraViewTarget cameraViewTarget)
        {
            OnCameraViewChanged(cameraViewTarget == null ? null : cameraViewTarget.CameraView);
        }


        // Called when the camera view changes.
        public virtual void OnCameraViewChanged(CameraView cameraView)
        {
            for (int i = 0; i < cameraViewParentingItems.Count; ++i)
            {
                for (int j = 0; j < cameraViewParentingItems[i].cameraViews.Count; ++j)
                {
                    if (cameraViewParentingItems[i].cameraViews[j] == cameraView)
                    {
                        switch (cameraViewParentingItems[i].parentType)
                        {
                            case CameraViewParentType.Transform:

                                cameraViewParentingItems[i].m_Transform.SetParent(cameraViewParentingItems[i].parentTransform);
                                break;

                            case CameraViewParentType.Camera:

                                if (cameraEntity != null) cameraViewParentingItems[i].m_Transform.SetParent(cameraEntity.MainCamera.transform);
                                break;

                            case CameraViewParentType.None:

                                cameraViewParentingItems[i].m_Transform.SetParent(null);
                                break;

                        }

                        // Position
                        if (cameraViewParentingItems[i].setLocalPosition)
                        {
                            cameraViewParentingItems[i].m_Transform.localPosition = cameraViewParentingItems[i].localPosition;
                        }

                        // Rotation
                        if (cameraViewParentingItems[i].setLocalRotation)
                        {
                            cameraViewParentingItems[i].m_Transform.transform.localRotation = Quaternion.Euler(cameraViewParentingItems[i].localRotationEulerAngles);
                        }

                        // Scale
                        if (cameraViewParentingItems[i].setLocalScale)
                        {
                            cameraViewParentingItems[i].m_Transform.localScale = cameraViewParentingItems[i].localScale;
                        }
                    }
                }
            }
        }
    }
}
