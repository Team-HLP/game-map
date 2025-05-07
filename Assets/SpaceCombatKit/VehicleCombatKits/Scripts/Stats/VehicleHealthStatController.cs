using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Health;
using VSX.Loadouts;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Display stat for vehicle health on the UI.
    /// </summary>
    public class VehicleHealthStatController : VehicleStatController
    {
        [Tooltip("The health type to display the stat for.")]
        [SerializeField]
        protected HealthType healthType;


        /// <summary>
        /// Get whether an object is relevant to display the stat for.
        /// </summary>
        /// <param name="statTarget">The object to display the stat for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            VehicleHealth health = statTarget.GetComponent<VehicleHealth>();
            if (health == null) return false;

            if (hideIfValueIsZero && Mathf.Approximately(health.GetHealthCapacityByType(healthType), 0))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Get the float value of the stat for an object.
        /// </summary>
        /// <param name="statTarget">The object to get the stat value for.</param>
        /// <returns>The stat value.</returns>
        protected override float GetStatValue(GameObject statTarget)
        {
            VehicleHealth health = statTarget.GetComponent<VehicleHealth>();
            if (health == null) return 0f;

            return health.GetHealthCapacityByType(healthType);
        }
    }
}


