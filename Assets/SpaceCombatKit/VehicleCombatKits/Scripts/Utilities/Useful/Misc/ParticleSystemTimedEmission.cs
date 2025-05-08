using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Emit particles from a particle system for a specified time.
    /// </summary>
    public class ParticleSystemTimedEmission : MonoBehaviour
    {
        [Tooltip("The particle system to emit from.")]
        [SerializeField]
        protected List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        [Tooltip("How long to emit for.")]
        [SerializeField]
        protected float emitTime = 2;

        [Tooltip("Whether to emit when the object becomes active in the scene.")]
        [SerializeField]
        protected bool emitOnEnable = true;

        protected List<ParticleSystem.EmissionModule> emissionModules = new List<ParticleSystem.EmissionModule>();
        protected float startTime;
        

        protected virtual void Awake()
        {
            for(int i = 0; i < particleSystems.Count; ++i)
            {
                emissionModules.Add(particleSystems[i].emission);
            }
        }


        protected virtual void OnEnable()
        {
            if (emitOnEnable) Emit();
        }


        /// <summary>
        /// Begin emitting particles.
        /// </summary>
        public virtual void Emit()
        {
            startTime = Time.time;
            for (int i = 0; i < emissionModules.Count; ++i)
            {
                ParticleSystem.EmissionModule mod = emissionModules[i];
                mod.enabled = true;
            }
        }


        protected virtual void Update()
        {
            if (Time.time - startTime > emitTime)
            {
                for (int i = 0; i < emissionModules.Count; ++i)
                {
                    ParticleSystem.EmissionModule mod = emissionModules[i];
                    mod.enabled = false;
                }
            }
        }
    }
}
