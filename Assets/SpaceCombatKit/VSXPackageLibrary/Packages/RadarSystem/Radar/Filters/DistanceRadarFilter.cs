using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.RadarSystem
{
    /// <summary>
    /// Filters targets based on distance and type.
    /// </summary>
    public class DistanceRadarFilter : RadarFilter
    {
        [Tooltip("The trackable types which this filter operates on.")]
        [SerializeField]
        protected List<TrackableType> trackableTypes = new List<TrackableType>();

        [Tooltip("Whether the targets are filtered when outside the distance threshold (vs. inside).")]
        [SerializeField]
        protected bool filterOutsideDistance = true;

        [Tooltip("The distance threshold at which the filter operates.")]
        [SerializeField]
        protected float distanceThreshold = 500;


        /// <summary>
        /// Whether a target can pass the filter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Whether the target passes the filter.</returns>
        public override bool CanPass(Trackable target)
        {
            if (trackableTypes.IndexOf(target.TrackableType) == -1) { return true; }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            if (distance < distanceThreshold && !filterOutsideDistance) return false;

            if (distance > distanceThreshold && filterOutsideDistance) return false;

            return true;
        }
    }
}


