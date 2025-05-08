using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Utilities;


namespace VSX.Weapons
{
    /// <summary>
    /// Control the sound effects for a beam weapon unit.
    /// </summary>
    public class BeamSoundEffectsController : MonoBehaviour
    {

        [Tooltip("The beam weapon unit to control sound effects for.")]
        [SerializeField]
        protected BeamWeaponUnit beamWeaponUnit;

        [Tooltip("The audio sources that play when the beam is started from the 'off' state.")]
        [SerializeField]
        protected List<AudioSource> beamStartedAudios = new List<AudioSource>();

        [Tooltip("The audio sources that play when the beam turns off.")]
        [SerializeField]
        protected List<AudioSource> beamStoppedAudios = new List<AudioSource>();

        [Tooltip("The modulated audio loops that play while the beam is active, with properties controlled by the beam level.")]
        [SerializeField]
        protected List<ModulatedAudio> beamLoopAudios = new List<ModulatedAudio>();



        protected virtual void Reset()
        {
            beamWeaponUnit = GetComponentInChildren<BeamWeaponUnit>();
        }


        protected virtual void Awake()
        {
            beamWeaponUnit.onBeamLevelSet.AddListener(SetLevel);

            SetLevel(0);
        }


        /// <summary>
        /// Called when the beam is started from the 'off' state.
        /// </summary>
        protected virtual void OnBeamStarted()
        {
            for (int i = 0; i < beamStartedAudios.Count; ++i)
            {
                beamStartedAudios[i].Play();
            }
        }


        /// <summary>
        /// Called when the beam is stopeed/turned off.
        /// </summary>
        protected virtual void OnBeamStopped()
        {
            for (int i = 0; i < beamStoppedAudios.Count; ++i)
            {
                beamStoppedAudios[i].Play();
            }
        }


        /// <summary>
        /// Called when the beam level changes to adjust the audio.
        /// </summary>
        /// <param name="level">The beam level.</param>
        protected virtual void SetLevel(float level)
        {
            for (int i = 0; i < beamLoopAudios.Count; ++i)
            {
                beamLoopAudios[i].SetLevel(level);
            }
        }
    }
}
