using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;

namespace VSX.Vehicles.ControlAnimations
{
    public class VehicleControlAnimationHandler : MonoBehaviour, ICameraEntityUser
    {
        public bool applyLocal = true;

        [SerializeField]
        protected List<CameraView> cameraViews = new List<CameraView>();

        public bool clearAnimationsOnViewChange = true;

        public Transform vehicleHandle;

        public List<VehicleControlAnimation> animations = new List<VehicleControlAnimation>();

        protected CameraEntity cameraEntity;

        bool activated = false;


        public void SetCameraEntity(CameraEntity cameraEntity)
        {
            // Disconnect event from previous camera
            if (this.cameraEntity != null)
            {
                this.cameraEntity.onCameraViewTargetChanged.RemoveListener(OnCameraViewChanged);
            }

            // Set new camera
            this.cameraEntity = cameraEntity;

            // Connect to event on new camera
            if (this.cameraEntity != null)
            {
                OnCameraViewChanged(cameraEntity.CurrentViewTarget);
                this.cameraEntity.onCameraViewTargetChanged.AddListener(OnCameraViewChanged);
            }
        }


        // Called when the camera view changes
        protected virtual void OnCameraViewChanged(CameraViewTarget newCameraViewTarget)
        {

            // Check camera view
            if (cameraViews.Count > 0)
            {
                if (newCameraViewTarget == null) return;

                if (cameraViews.Count > 0 && cameraViews.IndexOf(newCameraViewTarget.CameraView) == -1)
                {
                    activated = false;

                    if (clearAnimationsOnViewChange)
                    {
                        Apply(Vector3.zero, Quaternion.identity);
                    }


                    return;
                }
            }

            activated = true;
        }

        private void FixedUpdate()
        {
            if (!activated) return;

            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            for (int i = 0; i < animations.Count; ++i)
            {
                rotation = animations[i].GetRotation() * rotation;
                position += animations[i].GetPosition();
            }

            Apply(position, rotation);

        }


        protected virtual void Apply(Vector3 position, Quaternion rotation)
        {
            if (applyLocal)
            {
                vehicleHandle.localPosition = position;
                vehicleHandle.localRotation = rotation;
            }
            else
            {
                vehicleHandle.position = position;
                vehicleHandle.rotation = rotation;
            }
        }
    }

}
