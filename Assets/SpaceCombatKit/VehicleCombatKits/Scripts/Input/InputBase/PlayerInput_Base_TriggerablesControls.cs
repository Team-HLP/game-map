using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;
using VSX.Weapons;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls triggerables (e.g. weapons) on a vehicle.
    /// </summary>
    public class PlayerInput_Base_TriggerablesControls : VehicleInput
    {
        [Tooltip("The primary weapon trigger index.")]
        [SerializeField]
        protected int primaryWeaponTriggerIndex = 0;

        [Tooltip("The secondary weapon trigger index.")]
        [SerializeField]
        protected int secondaryWeaponTriggerIndex = 1;

        protected TriggerablesManager triggerablesManager;



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            if (!base.Initialize(vehicle)) return false;

            triggerablesManager = vehicle.GetComponent<TriggerablesManager>();

            if (triggerablesManager == null)
            {
                if (debugInitialization)
                {
                    Debug.Log(GetType().Name + " component failed to initialize, TriggerablesManager component not found on vehicle.");
                }
                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " component successfully initialized.");
            }

            return true;
        }


        protected virtual void StartFiring(int triggerIndex)
        {
            if (!CanRunInput()) return;

            triggerablesManager.StartTriggeringAtIndex(triggerIndex);
        }


        protected virtual void StopFiring(int triggerIndex)
        {
            if (!CanRunInput()) return;

            triggerablesManager.StopTriggeringAtIndex(triggerIndex);
        }
    }
}