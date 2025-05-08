using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;
using VSX.CameraSystem;
using VSX.Vehicles;
using VSX.Weapons;
using VSX.Utilities;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls a vehicle's radar functionality.
    /// </summary>
    public class PlayerInput_Base_RadarControls : VehicleInput
    {

        protected TargetSelector targetSelector;

        protected Tracker tracker;
        protected CustomCursor cursor;
        protected CameraTarget cameraTarget;


        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            // Update the dependencies
            cursor = vehicle.GetComponentInChildren<CustomCursor>();
            cameraTarget = vehicle.GetComponentInChildren<CameraTarget>();
            tracker = vehicle.GetComponentInChildren<Tracker>();

            WeaponsController weapons = vehicle.GetComponentInChildren<WeaponsController>();
            if (weapons != null)
            {
                if (weapons.WeaponsTargetSelector != null)
                {
                    targetSelector = weapons.WeaponsTargetSelector;

                    if (debugInitialization)
                    {
                        Debug.Log(GetType().Name + " successfully initialized.");
                    }

                    return true;
                }
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " failed to initialize. Failed to find a weapons target selector on the vehicle.");
            }

            return false;

        }


        /// <summary>
        /// Select the target under the cursor.
        /// </summary>
        protected virtual void TargetUnderCursor()
        {
            if (!CanRunInput()) return;
           
            if (cursor != null && cameraTarget != null && cameraTarget.CameraEntity != null)
            {
                targetSelector.SelectFront(cameraTarget.CameraEntity.transform.position, cursor.AimDirection);
            }
        }


        /// <summary>
        /// Select the next target.
        /// </summary>
        protected virtual void TargetNext()
        {
            if (!CanRunInput()) return;

            targetSelector.Cycle(true);

        }


        /// <summary>
        /// Select the previous target.
        /// </summary>
        protected virtual void TargetPrevious()
        {
            if (!CanRunInput()) return;

            targetSelector.Cycle(false);
        }


        /// <summary>
        /// Select the nearest target.
        /// </summary>
        protected virtual void TargetNearest()
        {
            if (!CanRunInput()) return;

            targetSelector.SelectNearest();
        }


        /// <summary>
        /// Select the target in front.
        /// </summary>
        protected virtual void TargetFront()
        {
            if (!CanRunInput()) return;

            targetSelector.SelectFront();
        }
    }
}

