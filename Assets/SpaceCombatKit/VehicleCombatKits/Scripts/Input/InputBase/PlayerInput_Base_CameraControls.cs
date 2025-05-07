using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls camera functionality on a vehicle.
    /// </summary>
    public class PlayerInput_Base_CameraControls : VehicleInput
    {

        protected CameraTarget cameraTarget;

        protected CameraEntity cameraEntity;


        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            // Unlink from previous camera target
            if (cameraTarget != null)
            {
                cameraTarget.onCameraEntityTargeting.RemoveListener(OnCameraFollowingVehicle);
            }

            // Link to camera following camera target
            cameraTarget = vehicle.GetComponent<CameraTarget>();

            if (cameraTarget != null)
            {
                // Link to any new camera following
                cameraTarget.onCameraEntityTargeting.AddListener(OnCameraFollowingVehicle);

                // Link to camera already following
                if (cameraTarget.CameraEntity != null)
                {
                    OnCameraFollowingVehicle(cameraTarget.CameraEntity);
                }
            }

            return true;
        }


        protected virtual void OnCameraFollowingVehicle(CameraEntity cameraEntity)
        {
            this.cameraEntity = cameraEntity;
        }


        protected virtual void CycleCameraView(bool forward)
        {
            if (!CanRunInput()) return;

            if (cameraEntity != null)
            {
                cameraEntity.CycleCameraView(forward);
            }
        }
    }
}