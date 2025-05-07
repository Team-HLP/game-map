using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.SpaceCombatKit
{
    public class ShipEnterExitManager : VehicleEnterExitManager
    {

        [Header("Ship Enter/Exit Settings")]

        [Tooltip("The enter/exit manager for this ship.")]
        [SerializeField]
        protected ShipLander shipLander;

        [SerializeField]
        protected bool launchShipOnChildEnter = true;

        [SerializeField]
        protected float launchDelay = 1;
        protected bool launchDelayActive = false;
        public bool LaunchDelayActive { get => launchDelayActive; }

        [SerializeField]
        protected bool exitOnlyWhenLanded = true;


        /// <summary>
        /// Whether the child vehicle that has entered this vehicle can exit.
        /// </summary>
        /// <returns>Whether the child vehicle that has entered this vehicle can exit.</returns>
        public override bool CanExitToChild()
        {
            if (!base.CanExitToChild()) return false;

            if (launchDelayActive) return false;

            // Only allow exiting if the ship has landed
            if (exitOnlyWhenLanded && shipLander != null && shipLander.CurrentState != ShipLander.ShipLanderState.Landed)
            {
                return false;
            }

            return true;
        }

        public override void OnChildEntered(VehicleEnterExitManager child)
        {
            base.OnChildEntered(child);

            if (shipLander != null && child != null && launchShipOnChildEnter)
            {
                StartCoroutine(LaunchCoroutine());
            }
        }


        IEnumerator LaunchCoroutine()
        {
            launchDelayActive = true;
            yield return new WaitForSeconds(launchDelay);
            launchDelayActive = false;

            if (shipLander != null && child != null)
            {
                shipLander.Launch();
            }
        }
    }
}