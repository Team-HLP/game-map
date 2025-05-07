using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls;

namespace VSX.Vehicles
{
    /// <summary>
    /// Base class for vehicle input components.
    /// </summary>
    public class VehicleInput : GeneralInput
    {

        [Header("Vehicle Input")]

        // Vehicle to control when the scene starts
        [SerializeField]
        protected Vehicle startingVehicle;

        [SerializeField]
        protected List<VehicleClass> compatibleVehicleClasses = new List<VehicleClass>();


        protected override void Start()
        {
            // Initialize with the starting vehicle
            if (startingVehicle != null)
            {
                SetVehicle(startingVehicle);
            }
        }

        

        /// <summary>
        /// Set a new vehicle for the input component.
        /// </summary>
        /// <param name="vehicle">The new vehicle</param>
        public virtual void SetVehicle(Vehicle vehicle)
        {
            SetTargetObject(vehicle == null ? null : vehicle.gameObject);
        }


        protected override bool Initialize(GameObject inputTarget)
        {
            if (inputTarget == null) return false;

            Vehicle vehicle = inputTarget.GetComponent<Vehicle>();
            if (vehicle == null) return false;

            return Initialize(vehicle);
        }


        protected virtual bool Initialize(Vehicle vehicle)
        {
            if (compatibleVehicleClasses.Count > 0)
            {
                for (int i = 0; i < compatibleVehicleClasses.Count; ++i)
                {
                    if (compatibleVehicleClasses[i] == vehicle.VehicleClass)
                    {
                        return true;
                    }
                }

                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize. Vehicle is not a compatible vehicle class.");
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}