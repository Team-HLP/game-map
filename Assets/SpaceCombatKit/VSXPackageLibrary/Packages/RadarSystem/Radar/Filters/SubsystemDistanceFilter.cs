using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.RadarSystem
{
    /// <summary>
    /// This class filters out subsystems from target tracking unless the target is selected and within a specified distance.
    /// </summary>
    public class SubsystemDistanceFilter : RadarFilter
    {
        [Tooltip("The distance within which the subsystems may appear on the radar.")]
        [SerializeField]
        protected float distanceThreshold = 1500;


        [Tooltip("The target selectors which, if the target is selected by one of them, that target's subsystems may appear on radar.")]
        [SerializeField]
        protected List<TargetSelector> targetSelectors = new List<TargetSelector>();


        /// <summary>
        /// Whether a target can pass the filter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Whether the target passes the filter.</returns>
        public override bool CanPass(Trackable target)
        {
            bool isSubsystem = target.RootTrackable != target;
            if (!isSubsystem) return true;

            bool isSelectedTarget = false;
            for (int i = 0; i < targetSelectors.Count; ++i)
            {
                if (targetSelectors[i].SelectedTarget == null) continue;
                if (targetSelectors[i].SelectedTarget.RootTrackable == target.RootTrackable)
                {
                    isSelectedTarget = true;
                    break;
                }
            }
            if (!isSelectedTarget) return false;

            float distance = Vector3.Distance(target.RootTrackable.transform.position, transform.position);

            if (distance < distanceThreshold)
            {
                return true;
            }

            return false;
        }
    }
}

