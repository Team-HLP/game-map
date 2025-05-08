using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Health;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Play looped audio in the event of vehicle damage (e.g. beam weapons).
    /// </summary>
    public class VehicleDamageLoopedAudio : MonoBehaviour
    {
        [Tooltip("The vehicle health component to access damage information from.")]
        [SerializeField]
        protected VehicleHealth vehicleHealth;

        [Tooltip("How fast the audio fades out when damage is not occurring.")]
        [SerializeField]
        protected float fadeSpeed = 2;

        [Tooltip("The audio source to play.")]
        [SerializeField]
        protected AudioSource audioSource;

        [Tooltip("The health modifier types (i.e. the type of weapon) to play audio effects for. If list is empty, all health modifier types are valid.")]
        [SerializeField]
        protected List<HealthModifierType> healthModifierTypes = new List<HealthModifierType>();

        [Tooltip("The health types (i.e. the type of damaged object) to play audio effects for. If list is empty, all health types are valid.")]
        [SerializeField]
        protected List<HealthType> damagedObjectHealthTypes = new List<HealthType>();

        [SerializeField]
        protected float volume = 1;

        protected bool damageThisFrame = false;


        protected virtual void Awake()
        {
            for (int i = 0; i < vehicleHealth.Damageables.Count; ++i)
            {
                OnDamageableAdded(vehicleHealth.Damageables[i]);
            }

            vehicleHealth.onDamageableAdded += OnDamageableAdded;
            vehicleHealth.onDamageableRemoved += OnDamageableRemoved;

            audioSource.volume = 0;
        }


        /// <summary>
        /// Called for each damageable that is part of the vehicle or added at runtime.
        /// </summary>
        /// <param name="damageable">The damageable component.</param>
        protected virtual void OnDamageableAdded(Damageable damageable)
        {
            if (damagedObjectHealthTypes.Count == 0 || damagedObjectHealthTypes.IndexOf(damageable.HealthType) != -1)
            {
                damageable.onDamaged.AddListener(OnDamage);
            }
        }


        /// <summary>
        /// Called for each damageable that is part of the vehicle or added at runtime, that is removed.
        /// </summary>
        /// <param name="damageable">The damageable component that is removed.</param>
        protected virtual void OnDamageableRemoved(Damageable damageable)
        {
            damageable.onDamaged.RemoveListener(OnDamage);
        }


        /// <summary>
        /// Called when a damage event occurs on the vehicle.
        /// </summary>
        /// <param name="info">The damage event information.</param>
        protected virtual void OnDamage(HealthEffectInfo info)
        {
            if (healthModifierTypes.Count > 0)
            {
                if (healthModifierTypes.IndexOf(info.healthModifierType) == -1) return;
            }

            audioSource.volume = volume;
            damageThisFrame = true;
        }


        // Called every frame.
        protected virtual void Update()
        {
            // Fade out the audio only if damage was not done in this frame.

            if (damageThisFrame)
            {
                damageThisFrame = false;
            }
            else
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0, fadeSpeed * Time.deltaTime);
            }
        }
    }
}
