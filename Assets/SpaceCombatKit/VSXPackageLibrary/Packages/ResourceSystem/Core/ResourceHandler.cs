using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.ResourceSystem
{
    /// <summary>
    /// Represents a resource handler (something that adds or removes resources from a container).
    /// </summary>
    [System.Serializable]
    public class ResourceHandler
    {
        [Tooltip("The resource container to add/remove resources from.")]
        public ResourceContainerBase resourceContainer;

        [Tooltip("The amount to add or remove (positive for add, negative for remove).")]
        public float unitResourceChange;

        [Tooltip("Whether the resource change is per second (multiplied by Time.deltaTime).")]
        public bool perSecond = false;

        /// <summary>
        /// Check if the resource container can add or remove the specified amount.
        /// </summary>
        /// <param name="numResourceChanges">How many resource changes to check for (e.g. if there are multiple weapon units making up the weapon).</param>
        /// <returns>Whether the resource container can add or remove the specified amount.</returns>
        public bool Ready(int numResourceChanges = 1)
        {
            if (perSecond)
            {
                if (unitResourceChange < 0) 
                    return resourceContainer.CurrentAmountFloat > 0;
                else
                    return (resourceContainer.CapacityFloat - resourceContainer.CurrentAmountFloat) > 0;
            }
            else
            {
                return resourceContainer.CanAddRemove(numResourceChanges * unitResourceChange);
            }
        }


        /// <summary>
        /// Implement the resource changes.
        /// </summary>
        /// <param name="numResourceChanges">The number of resource changes to implement.</param>
        public virtual void Implement(int numResourceChanges = 1)
        {
            resourceContainer.AddRemove(numResourceChanges * unitResourceChange * (perSecond ? Time.deltaTime : 1));
        }
    }
}
