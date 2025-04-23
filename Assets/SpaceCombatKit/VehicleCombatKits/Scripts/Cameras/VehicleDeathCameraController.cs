using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;
using UnityEngine.Events;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    public class VehicleDeathCameraController : CameraOrbiter
    {

        protected Vehicle targetVehicle;


        [SerializeField]
        protected bool adjustDistanceToVehicleSize = true;

        [SerializeField]
        protected float vehicleSizeToCameraDistance = 2;

        [Tooltip("Whether to disable the camera collision when the death camera controller is activated.")]
        [SerializeField]
        protected bool disableCameraCollision = true;

        public UnityEvent onStarted;


        protected virtual void Awake()
        {
            m_CameraEntity.onCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }


        /// <summary>
        /// Called when the vehicle camera's camera target changes.
        /// </summary>
        /// <param name="newTarget">The new camera target.</param>
        protected virtual void OnCameraTargetChanged(CameraTarget newTarget)
        {
            if (targetVehicle != null)
            {
                targetVehicle.onDestroyed.RemoveListener(OnVehicleDestroyed);
            }

            if (newTarget != null)
            {
                targetVehicle = newTarget.GetComponent<Vehicle>();
                if (targetVehicle != null)
                {
                    targetVehicle.onDestroyed.AddListener(OnVehicleDestroyed);
                }
            }
            else
            {
                targetVehicle = null;
            }
        }


        protected virtual void OnVehicleDestroyed()
        {
            m_CameraEntity.SetCameraViewTarget(null);
            if (disableCameraCollision) m_CameraEntity.CameraCollisionEnabled = false;

            if (adjustDistanceToVehicleSize)
            {
                orbitOffset = orbitOffset.normalized * vehicleSizeToCameraDistance * ((targetVehicle.Bounds.size.x + targetVehicle.Bounds.size.y + targetVehicle.Bounds.size.z) / 3);
            }

            Orbit(targetVehicle.transform.position);

            onStarted.Invoke();
        }


        protected override void FixedUpdate()
        {
            if (targetVehicle != null) orbitTargetPosition = targetVehicle.transform.position;
            base.FixedUpdate();
        }
    }
}
