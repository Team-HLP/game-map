using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Animate the alpha of a renderer.
    /// </summary>
    public class RendererFader : MonoBehaviour
    {
        [Tooltip("The renderer to animate.")]
        [SerializeField] protected Renderer m_Renderer;

        [Tooltip("Whether to use a specific color ID when setting the material alpha (if unchecked, Material.color is used).")]
        [SerializeField]
        protected bool overrideMaterialColorID = false;

        [Tooltip("if 'Override Material Color ID' is checked, this color ID will be used when setting the materials alpha.")]
        [SerializeField]
        protected string materialColorIDOverride = "";

        [Tooltip("The animation duration.")]
        [SerializeField] protected float duration = 1;

        [Tooltip("The curve that describes the alpha over time.")]
        [SerializeField] protected AnimationCurve alphaFadeCurve;

        [Tooltip("The multiplier for the alpha.")]
        [SerializeField] protected float alphaMultiplier = 1;

        protected float startTime;
        protected bool animating = false;


        protected virtual void OnEnable()
        {
            StartAnimation();
        }

        public virtual void StartAnimation()
        {
            animating = true;
            startTime = Time.time;
        }

        protected virtual void Update()
        {
            if (animating)
            {
                float amount = (Time.time - startTime) / duration;
                if (amount > 1)
                {
                    animating = false;
                    SetAlpha(alphaFadeCurve.Evaluate(1) * alphaMultiplier);
                }
                else
                {
                    SetAlpha(alphaFadeCurve.Evaluate(amount) * alphaMultiplier);
                }
            }
        }

        protected virtual void SetAlpha(float alphaValue)
        {
            Color c;
            if (overrideMaterialColorID)
            {
                c = m_Renderer.material.GetColor(materialColorIDOverride);
            }
            else
            {
                c = m_Renderer.material.color;
            }

            c.a = alphaValue;
            
            if (overrideMaterialColorID)
            {
                m_Renderer.material.SetColor(materialColorIDOverride, c);
            }
            else
            {
                m_Renderer.material.color = c;
            }
        }
    }
}

