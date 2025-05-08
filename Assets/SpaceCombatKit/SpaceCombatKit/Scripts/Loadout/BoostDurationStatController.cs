using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Engines3D;
using VSX.Loadouts;

namespace VSX.SpaceCombatKit
{
    /// <summary>
    /// Display stat for spaceship boost duration on the UI.
    /// </summary>
    public class BoostDurationStatController : VehicleStatController
    {
        /// <summary>
        /// Get whether an object is relevant to display the stat for.
        /// </summary>
        /// <param name="statTarget">The object to display the stat for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            VehicleEngines3D engines = statTarget.GetComponent<VehicleEngines3D>();
            if (engines == null) return false;

            return true;
        }


        /// <summary>
        /// Get the float value of the stat for an object.
        /// </summary>
        /// <param name="statTarget">The object to get the stat value for.</param>
        /// <returns>The stat value.</returns>
        protected override float GetStatValue(GameObject statTarget)
        {
            VehicleEngines3D engines = statTarget.GetComponent<VehicleEngines3D>();
            if (engines == null) return 0f;

            if (engines.BoostResourceHandlers.Count == 0) return Mathf.Infinity;

            float maxDuration = 0;
            for (int i = 0; i < engines.BoostResourceHandlers.Count; ++i)
            {
                float usage = engines.BoostResourceHandlers[i].unitResourceChange;
                if (Mathf.Approximately(usage, 0) || usage > 0) continue;

                float capacity = engines.BoostResourceHandlers[i].resourceContainer.CapacityFloat;

                maxDuration = Mathf.Max(capacity / Mathf.Abs(usage), maxDuration);
            }

            return maxDuration;
        }
    }
}
