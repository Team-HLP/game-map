using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a script that controls the camera for a specific type of vehicle.
    /// </summary>
    public class VehicleCameraController : CameraController
    {

        [Header("General")]

        [Tooltip("Whether to specify the vehicle classes that this camera controller is for.")]
        [SerializeField]
        protected bool specifyCompatibleVehicleClasses;

        [Tooltip("The vehicle classes that this camera controller is compatible with.")]
        [SerializeField]
        protected List<VehicleClass> compatibleVehicleClasses = new List<VehicleClass>();

        protected VehicleCamera vehicleCamera;
        public override void SetCamera(CameraEntity cameraEntity)
        {
            base.SetCamera(cameraEntity);
            vehicleCamera = cameraEntity.GetComponent<VehicleCamera>();
            if (vehicleCamera == null)
            {
                Debug.LogError("Cannot use a Vehicle Camera Controller with the Camera Entity component. Must be used with Vehicle Camera instead.");
            }
        }


        protected override bool Initialize(CameraTarget newTarget)
        {

            Vehicle vehicle = newTarget.GetComponent<Vehicle>();
            if (vehicle == null)
            {
                return false;
            }

            // If compatible vehicle classes are specified, check that the list contains this vehicle's class.
            if (specifyCompatibleVehicleClasses)
            {
                if (compatibleVehicleClasses.IndexOf(vehicle.VehicleClass) != -1)
                {
                    return (base.Initialize(newTarget));
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return (base.Initialize(newTarget));
            }
        }


        public virtual bool IsCompatible(Vehicle vehicle)
        {
            // If compatible vehicle classes are specified, check that the list contains this vehicle's class.
            if (specifyCompatibleVehicleClasses)
            {
                if (compatibleVehicleClasses.IndexOf(vehicle.VehicleClass) != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        public virtual CameraViewTarget GetDefaultCameraViewTarget(Vehicle vehicle)
        {
            CameraTarget cameraTarget = vehicle.GetComponentInChildren<CameraTarget>();

            if (cameraTarget == null) return null;

            if (startingView != null)
            {
                CameraViewTarget result = null;
                for(int i = 0; i < cameraTarget.CameraViewTargets.Count; ++i)
                {
                    if (cameraTarget.CameraViewTargets[i].CameraView == startingView)
                    {
                        result = cameraTarget.CameraViewTargets[i];
                    }
                }

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return cameraTarget.CameraViewTargets.Count > 0 ? cameraTarget.CameraViewTargets[0] : null;
                }
            }
            else
            {
                if (cameraTarget.CameraViewTargets.Count > 0)
                {
                    return cameraTarget.CameraViewTargets[0];
                }
                else
                {
                    return null;
                }
            }
            
        }
    }
}