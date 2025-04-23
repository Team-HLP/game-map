using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Health;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Play one-shot audio in the event of vehicle collisions.
    /// </summary>
    public class VehicleCollisionAudio : MonoBehaviour
    {
        [Tooltip("The vehicle health component to access damage information from.")]
        [SerializeField]
        protected VehicleHealth vehicleHealth;

        [Tooltip("The health types (i.e. the type of damaged object) to play audio effects for. If list is empty, all health types are valid.")]
        [SerializeField]
        protected List<HealthType> damagedObjectHealthTypes = new List<HealthType>();

        [Tooltip("The audio sources available to play a clip (does NOT mean they are all played at once, only one audio is played per damage event). Add as many as you need to prevent clipping when many sounds are played in quick succession.")]
        [SerializeField]
        protected List<AudioSource> audioSources = new List<AudioSource>();

        [Tooltip("The collection of audio clips to draw randomly from.")]
        [SerializeField]
        protected List<AudioClip> clips = new List<AudioClip>();

        [Tooltip("The volume to play the audio at as a function of the collision relative velocity (how fast the two objects collided).")]
        [SerializeField]
        protected AnimationCurve collisionVelocityToVolumeCurve = AnimationCurve.Linear(0, 0, 100, 1);

        [Tooltip("The minimum randomized pitch of the audio.")]
        [SerializeField]
        protected float minPitch = 0.8f;

        [Tooltip("The maximum randomized pitch of the audio.")]
        [SerializeField]
        protected float maxPitch = 1.2f;

        [Tooltip("The minimum randomized volume multiplier for the audio.")]
        [SerializeField]
        protected float minVolumeMultiplier = 0.5f;

        [Tooltip("The minimum randomized volume multiplier for the audio.")]
        [SerializeField]
        protected float maxVolumeMultiplier = 1f;


        protected virtual void Awake()
        {
            for (int i = 0; i < vehicleHealth.Damageables.Count; ++i)
            {
                OnDamageableAdded(vehicleHealth.Damageables[i]);
            }

            vehicleHealth.onDamageableAdded += OnDamageableAdded;
            vehicleHealth.onDamageableRemoved += OnDamageableRemoved;
        }


        /// <summary>
        /// Called for each damageable that is part of the vehicle or added at runtime.
        /// </summary>
        /// <param name="damageable">The damageable component.</param>
        protected virtual void OnDamageableAdded(Damageable damageable)
        {
            if (damagedObjectHealthTypes.Count == 0 || damagedObjectHealthTypes.IndexOf(damageable.HealthType) != -1)
            {
                damageable.onCollision.AddListener(OnCollision);
            }
        }


        /// <summary>
        /// Called for each damageable that is part of the vehicle or added at runtime, that is removed.
        /// </summary>
        /// <param name="damageable">The damageable component that is removed.</param>
        protected virtual void OnDamageableRemoved(Damageable damageable)
        {
            damageable.onCollision.RemoveListener(OnCollision);
        }


        /// <summary>
        /// Called in the event of a collision.
        /// </summary>
        /// <param name="collision">The collision information.</param>
        protected virtual void OnCollision(Collision collision)
        {
            // Find the first audio source that isn't playing, or else the oldest one

            int index = 0;
            float maxTime = 0;
            for (int i = 0; i < audioSources.Count; ++i)
            {
                if (!audioSources[i].isPlaying)
                {
                    index = i;
                    break;
                }

                if (audioSources[i].time > maxTime)
                {
                    maxTime = audioSources[i].time;
                    index = i;
                }
            }

            audioSources[index].Stop();
            audioSources[index].clip = clips[Random.Range(0, clips.Count)];
            audioSources[index].pitch = Random.Range(minPitch, maxPitch);
            audioSources[index].volume = collisionVelocityToVolumeCurve.Evaluate(collision.relativeVelocity.magnitude) * Random.Range(minVolumeMultiplier, maxVolumeMultiplier);
            audioSources[index].Play();
        }
    }
}

