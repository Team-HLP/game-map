using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Animates the intensity of a light.
    /// </summary>
    public class LightAnimator : AnimationController
    {

        [Tooltip("The light to animate.")]
        [SerializeField]
        protected Light m_Light;
        public virtual Light Light { get => m_Light; }


        [Tooltip("The maximum intensity (the intensity when the animation curve reaches 1).")]
        [SerializeField]
        protected float maxIntensity = 1;


        [Tooltip("The curve describing the intensity of the light over the animation period.")]
        [SerializeField]
        protected AnimationCurve intensityCurve = AnimationCurve.Linear(0, 1, 1, 0);


        protected float intensityMultiplier = 1;
        public float IntensityMultiplier
        {
            get { return intensityMultiplier; }
            set 
            { 
                intensityMultiplier = value;
                SetAnimationPosition(animationPosition);
            }
        }


        protected virtual void Reset()
        {
            m_Light = GetComponentInChildren<Light>();
        }


        protected virtual void Awake()
        {
            m_Light.intensity = 0;
        }


        public override void SetAnimationPosition(float normalizedPosition)
        {
            m_Light.intensity = intensityCurve.Evaluate(normalizedPosition) * maxIntensity * intensityMultiplier;
        }
    }
}

