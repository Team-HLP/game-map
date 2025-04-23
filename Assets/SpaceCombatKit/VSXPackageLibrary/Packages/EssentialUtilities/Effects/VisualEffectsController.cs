using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Control a set of visual effects.
    /// </summary>
    public class VisualEffectsController : MonoBehaviour
    {

        [Tooltip("Whether to override the color of the effects.")]
        [SerializeField]
        protected bool overrideColor = false;

        [Tooltip("The color to apply to the effects (if 'Override Color' above is checked).")]
        [SerializeField]
        protected Color colorOverride = Color.white;

        [Tooltip("The effects renderers to control.")]
        [SerializeField]
        protected List<RendererColorController> renderers = new List<RendererColorController>();

        [Tooltip("The effects lights.")]
        [SerializeField]
        protected List<LightColorController> lights = new List<LightColorController>();
        protected List<float> lightBaseIntensities = new List<float>();


        protected virtual void Awake()
        {
            foreach (RendererColorController item in renderers)
            {
                item.Initialize();
            }

            foreach (LightColorController light in lights)
            {
                light.Initialize();
            }
        }


        protected virtual void Start()
        {
            if (overrideColor)
            {
                ApplyColor(colorOverride);
            }
        }


        protected virtual void ApplyColor(Color newColor)
        {
            for(int i = 0; i < renderers.Count; ++i)
            {
                renderers[i].ApplyColor(newColor);
            }

            for (int i = 0; i < lights.Count; ++i)
            {
                lights[i].ApplyColor(newColor);
            }
        }


        /// <summary>
        /// Adjust the level of the visual effects.
        /// </summary>
        /// <param name="level">The effects level.</param>
        public virtual void SetLevel(float level)
        {
            // Update the renderers

            for (int i = 0; i < renderers.Count; ++i)
            {
                renderers[i].ApplyAlpha(level);
            }

            // Update the lights

            for (int i = 0; i < lights.Count; ++i)
            {
                lights[i].SetIntensityLevel(level);
            }
        }
    }
}