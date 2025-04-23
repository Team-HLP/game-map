using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Base class for a script that calculates an aim ray (origin and direction) and checks for aim hits in the scene.
    /// </summary>
    public abstract class AimControllerBase : MonoBehaviour
    {
        protected Ray aim;
        /// <summary>
        /// Get the ray that represents the aim origin and direction.
        /// </summary>
        public virtual Ray Aim { get => aim; }


        protected bool hitFound = false;
        /// <summary>
        /// Whether an aim hit was found.
        /// </summary>
        public virtual bool HitFound { get => hitFound; }


        protected RaycastHit hit;
        /// <summary>
        /// The collider that the aim hit.
        /// </summary>
        public virtual RaycastHit Hit { get => hit; }
    }
}

