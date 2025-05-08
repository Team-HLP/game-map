using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.RadarSystem
{
    /// <summary>
    /// Base class for a radar filter which can be used to control what the radar can track at any given time.
    /// </summary>
    public abstract class RadarFilter : MonoBehaviour
    {
        /// <summary>
        /// Whether a target can pass the filter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Whether the target passes the filter.</returns>
        public virtual bool CanPass(Trackable target)
        {
            return true;
        }
    }
}
