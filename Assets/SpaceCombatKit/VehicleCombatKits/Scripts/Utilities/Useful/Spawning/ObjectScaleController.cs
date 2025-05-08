using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Handles scaling of properties that don't automatically scale with the transform, including light ranges, particle system gravity, and line renderers.
    /// Useful for adding effects and objects to a scene at different scales without visual problems.
    /// </summary>
    public class ObjectScaleController : MonoBehaviour
    {
        [Tooltip("Whether to scale the range of lights in the hierarchy according to the Transform's scale.")]
        [SerializeField]
        protected bool scaleLightRanges = true;
        protected List<Light> lights = new List<Light>();
        protected List<float> lightBaseRanges = new List<float>();


        [Tooltip("Whether to scale the gravity of particle systems in the hierarchy according to the Transform's scale.")]
        [SerializeField]
        protected bool scaleParticleSystemGravity = true;
        protected ParticleSystem[] particleSystems;
        protected ParticleSystem.MainModule[] particleSystemMainModules;
        protected float[] particleSystemBaseGravityMultipliers;


        [Tooltip("Whether to scale the width of line renderers in the hierarchy according to the Transform's scale.")]
        [SerializeField]
        protected bool scaleLineRenderers = true;
        protected LineRenderer[] lineRenderers;
        protected float[] lineRendererBaseWidths;


        protected virtual void Awake()
        {
            // Gather particle system info.

            particleSystems = GetComponentsInChildren<ParticleSystem>();
            particleSystemMainModules = new ParticleSystem.MainModule[particleSystems.Length];
            particleSystemBaseGravityMultipliers = new float[particleSystems.Length];
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystemMainModules[i] = particleSystems[i].main;
                particleSystemBaseGravityMultipliers[i] = particleSystems[i].main.gravityModifierMultiplier;
            }

            // Gather lights info.

            lights = new List<Light>(GetComponentsInChildren<Light>());
            foreach(Light light in lights)
            {
                lightBaseRanges.Add(light.range);
            }

            // Gather line renderer info.

            lineRenderers = GetComponentsInChildren<LineRenderer>();
            lineRendererBaseWidths = new float[lineRenderers.Length];
            for(int i = 0; i < lineRenderers.Length; ++i)
            {
                lineRendererBaseWidths[i] = lineRenderers[i].widthMultiplier;
            }
        }


        protected virtual void OnEnable()
        {
            SetScale((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3.0f);
        }


        /// <summary>
        /// Set the scale of this object.
        /// </summary>
        /// <param name="scale">The new scale.</param>
        public virtual void SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, scale);

            // Update particle systems
            if (scaleParticleSystemGravity)
            {
                for (int i = 0; i < particleSystemMainModules.Length; i++)
                {
                    if (particleSystemMainModules[i].scalingMode == ParticleSystemScalingMode.Hierarchy)
                    {
                        particleSystemMainModules[i].gravityModifierMultiplier = particleSystemBaseGravityMultipliers[i] * scale;
                    }
                }
            }           

            // Update lights
            if (scaleLightRanges)
            {
                for (int i = 0; i < lights.Count; ++i)
                {
                    lights[i].range = lightBaseRanges[i] * scale;
                }
            }

            // Update line renderers
            if (scaleLineRenderers)
            {
                for(int i = 0; i < lineRenderers.Length; ++i)
                {
                    lineRenderers[i].widthMultiplier = lineRendererBaseWidths[i] * scale;
                }
            }
        }
    }
}

