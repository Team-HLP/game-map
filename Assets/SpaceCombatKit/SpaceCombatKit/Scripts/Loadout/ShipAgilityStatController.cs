using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Engines3D;
using VSX.Loadouts;

namespace VSX.SpaceCombatKit
{
    /// <summary>
    /// Display stat for spaceship agility on the UI.
    /// </summary>
    public class ShipAgilityStatController : VehicleStatController
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

            Vector3 maxAngularSpeeds = engines.GetMaxAngularSpeedByAxis();
            return Mathf.Max(maxAngularSpeeds.x, maxAngularSpeeds.y, maxAngularSpeeds.z);
        }
    }
}


