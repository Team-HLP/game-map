using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls a vehicle's auto aim functionality.
    /// </summary>
    public class PlayerInput_Base_AutoAimControls : VehicleInput
    {

        protected GimballedVehicleAutoAim aimComponent;


        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle))
            {
                return false;
            }

            aimComponent = vehicle.GetComponent<GimballedVehicleAutoAim>();

            return (aimComponent != null);
        }


        protected virtual void ToggleAutoAim()
        {
            if (!CanRunInput()) return;

            if (aimComponent.AutoAimEnabled)
            {
                aimComponent.DisableAutoAim();
            }
            else
            {
                aimComponent.EnableAutoAim();
            }
        }
    }
}

