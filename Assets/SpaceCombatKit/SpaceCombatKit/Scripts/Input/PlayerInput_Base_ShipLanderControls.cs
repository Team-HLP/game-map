using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Vehicles;


namespace VSX.SpaceCombatKit
{

    /// <summary>
    /// Base class for a player input script that controls takeoff/landing of a vehicle.
    /// </summary>
    public class PlayerInput_Base_ShipLanderControls : VehicleInput
    {

        [Tooltip("Whether to override the UI prompt for landing/takeoff with the ones from this script.")]
        [SerializeField]
        protected bool overridePrompts = true;

        [Tooltip("The UI prompt for launching (note {control} is automatically replaced with a reference to the currently bound input).")]
        [SerializeField]
        protected string launchPrompt = "Press {control} to launch";

        [Tooltip("The UI prompt for landing (note {control} is automatically replaced with a reference to the currently bound input).")]
        [SerializeField]
        protected string landPrompt = "Press {control} to land";

        protected ShipLander shipLander;
        protected HUDShipLander hudShipLander;



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            shipLander = vehicle.GetComponentInChildren<ShipLander>();

            hudShipLander = vehicle.GetComponentInChildren<HUDShipLander>();

            if (shipLander == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + shipLander.GetType().Name + " component was not found on the vehicle.");
                }

                return false;
            }
            else
            {
                if (overridePrompts)
                {
                    hudShipLander.SetPrompts(launchPrompt.Replace("{control}", GetControlDisplayString()),
                                            landPrompt.Replace("{control}", GetControlDisplayString()));
                }

                if (debugInitialization)
                {
                    Debug.Log(GetType().Name + " successfully initialized.");
                }

                return true;
            }
        }


        // Get the string to display the input on the UI.
        protected virtual string GetControlDisplayString()
        {
            return "";
        }


        protected virtual void LaunchLand()
        {
            if (!CanRunInput()) return;

            switch (shipLander.CurrentState)
            {
                case (ShipLander.ShipLanderState.Launched):

                    shipLander.Land();

                    break;

                case (ShipLander.ShipLanderState.Landed):

                    shipLander.Launch();

                    break;
            }
        }
    }
}
