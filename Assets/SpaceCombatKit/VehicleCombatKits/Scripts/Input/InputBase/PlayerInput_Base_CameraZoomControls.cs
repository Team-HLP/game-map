using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls;
using VSX.CameraSystem;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls camera zoom functionality on a vehicle.
    /// </summary>
    public class PlayerInput_Base_CameraZoomControls : VehicleInput
    {

        protected CameraEntity m_Camera;

        protected CameraTarget cameraTarget;

        [Tooltip("The minimum field-of-view (represents the zoom-in limit)")]
        [SerializeField]
        protected float minFOV = 20;

        [Tooltip("The speed that the zoom changes in response to input.")]
        [SerializeField]
        protected float zoomSpeed = 2;

        protected float currentFOV;

        protected float zoomInputValue;


        protected override void Start()
        {
            base.Start();
        }


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


        protected override void OnUninitialized(GameObject targetObject)
        {
            if (m_Camera != null) m_Camera.SetFieldOfView(m_Camera.DefaultFieldOfView);
        }


        protected virtual void OnCameraFollowingVehicle(CameraEntity cameraEntity)
        {
            this.m_Camera = cameraEntity;

            currentFOV = cameraEntity.DefaultFieldOfView;
        }


        // Called every frame that this input script is running.
        protected override void OnInputUpdate()
        {
            // Calculate the FOV
            currentFOV = Mathf.Clamp(currentFOV - zoomInputValue * zoomSpeed * Time.deltaTime, minFOV, m_Camera.DefaultFieldOfView);

            // Set the FOV
            m_Camera.SetFieldOfView(currentFOV);
        }
    }
}

