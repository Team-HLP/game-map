using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using VSX.Utilities;

namespace VSX.Weapons
{
    
	/// <summary>
    /// Controls the hit effect that is shown when a beam strikes a surface.
    /// </summary>
	public abstract class BeamHitEffectControllerBase : MonoBehaviour 
	{

        /// <summary>
        /// Set the 'on' level of the hit effect.
        /// </summary>
        /// <param name="level">The 'on' level.</param>
        public virtual void SetLevel(float level) { }


        /// <summary>
        /// Set whether the effect is activated or not.
        /// </summary>
        /// <param name="activate">Whether it is activated or not.</param>
        public virtual void SetActivation(bool activate) { }


        /// <summary>
        /// Called every frame that the beam hits something.
        /// </summary>
        /// <param name="hit">The hit information.</param>
        public virtual void OnHit(RaycastHit hit) { }


        public virtual void OnNoHit() { }
    }
}