using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.ResourceSystem;
using VSX.Loadouts;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Display stat for a vehicle's resource capacity on the UI.
    /// </summary>
    public class VehicleResourceCapacityStatController : VehicleStatController
    {
        [Tooltip("The resource type to display the stat for.")]
        [SerializeField]
        protected ResourceType resourceType;

        /// <summary>
        /// Get whether an object is relevant to display the stat for.
        /// </summary>
        /// <param name="statTarget">The object to display the stat for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            ResourceContainer[] resourceContainers = statTarget.GetComponentsInChildren<ResourceContainer>();
            foreach (ResourceContainer container in resourceContainers)
            {
                if (container.ResourceType == resourceType)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Get the float value of the stat for an object.
        /// </summary>
        /// <param name="statTarget">The object to get the stat value for.</param>
        /// <returns>The stat value.</returns>
        protected override float GetStatValue(GameObject statTarget)
        {
            float amount = 0;
            ResourceContainer[] resourceContainers = statTarget.GetComponentsInChildren<ResourceContainer>();
            foreach (ResourceContainer container in resourceContainers)
            {
                if (container.ResourceType == resourceType)
                {
                    amount += container.CapacityFloat;
                }
            }

            return amount;
        }
    }
}


