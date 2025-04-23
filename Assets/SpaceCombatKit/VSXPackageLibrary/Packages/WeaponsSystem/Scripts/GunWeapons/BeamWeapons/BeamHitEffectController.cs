using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.Utilities;

namespace VSX.Weapons
{
    
	/// <summary>
    /// Controls the hit effect that is shown when a beam strikes a surface.
    /// </summary>
	public class BeamHitEffectController : BeamHitEffectControllerBase 
	{
        public BeamWeaponUnit beamWeapon;

        [Tooltip("The audios that are modulated by the beam level.")]
        [SerializeField] 
        protected List<ModulatedAudio> hitEffectAudios = new List<ModulatedAudio>();


        protected virtual void Awake()
        {
            beamWeapon.onBeamLevelSet.AddListener(SetLevel);
            beamWeapon.onHit.AddListener(OnHit);
            beamWeapon.onNoHit.AddListener(OnNoHit);

            SetLevel(0);
        }


        /// <summary>
        /// Set the 'on' level of the hit effect.
        /// </summary>
        /// <param name="level">The 'on' level.</param>
        public override void SetLevel(float level) 
        {
            for(int i = 0; i < hitEffectAudios.Count; ++i)
            {
                hitEffectAudios[i].SetLevel(level);
            }
        }



        /// <summary>
        /// Set the activation of the hit effect.
        /// </summary>
        /// <param name="activate">Whether it is activated or not.</param>
        public override void SetActivation(bool activate)
        {
            gameObject.SetActive(activate);
        }


        /// <summary>
        /// Do stuff when the beam hit something.
        /// </summary>
        /// <param name="hit">The hit information.</param>
        public override void OnHit(RaycastHit hit)
        {
            SetActivation(true);
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(hit.normal);
        }


        public override void OnNoHit()
        {
            SetActivation(false);
        }
    }
}