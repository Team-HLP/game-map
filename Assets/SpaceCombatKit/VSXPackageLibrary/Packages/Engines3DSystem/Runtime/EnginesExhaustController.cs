using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Engines3D
{
    /// <summary>
    /// Controls the visual effects for an Engines component.
    /// </summary>
    public class EnginesExhaustController : MonoBehaviour
    {

        /// <summary>
        /// A renderer that is animated by the engines input.
        /// </summary>
        [System.Serializable]
        public class AnimatedRenderer
        {

            [Tooltip("The renderer being animated.")]
            [SerializeField]
            protected Renderer renderer;

            [Tooltip("The material index of the material to animate properties of.")]
            [SerializeField]
            protected int materialIndex;

            protected Material mat;

            [Tooltip("The list of shader color animations.")]
            public List<ShaderColorSetting> shaderColorSettings = new List<ShaderColorSetting>();


            /// <summary>
            /// A shader float animation.
            /// </summary>
            [System.Serializable]
            public class ShaderFloatSetting
            {
                [Tooltip("The shader float key.")]
                public string key;

                [Tooltip("The float value over the course of the animation (0-1).")]
                public AnimationCurve curve;
            }


            [System.Serializable]
            public class ShaderColorSetting
            {
                [Tooltip("The shader color key.")]
                public string key;

                [Tooltip("The color of the shader property over the course of the animation.")]
                [GradientUsage(true)]
                public Gradient color;
            }


            [Tooltip("The list of shader float animations.")]
            public List<ShaderFloatSetting> shaderFloatSettings = new List<ShaderFloatSetting>();


            /// <summary>
            /// Initialize the animated renderer.
            /// </summary>
            public virtual void Initialize()
            {
                mat = renderer.materials[materialIndex];
            }


            /// <summary>
            /// Update the renderer animation.
            /// </summary>
            /// <param name="animationPosition">The normalised (0-1) animation position.</param>
            public virtual void Update(float animationPosition)
            {
                foreach (ShaderFloatSetting floatSetting in shaderFloatSettings)
                {
                    mat.SetFloat(floatSetting.key, floatSetting.curve.Evaluate(animationPosition));
                }

                foreach (ShaderColorSetting colorSetting in shaderColorSettings)
                {
                    mat.SetColor(colorSetting.key, colorSetting.color.Evaluate(animationPosition));
                }
            }
        }


        /// <summary>
        /// A particle system that is animated by the engines component.
        /// </summary>
        [System.Serializable]
        public class AnimatedParticle
        {
            [Tooltip("The particle system to animate.")]
            [SerializeField]
            protected ParticleSystem particleSystem;
            public ParticleSystem ParticleSystem { get { return particleSystem; } }

            protected ParticleSystem.MainModule mainModule;
            public ParticleSystem.MainModule MainModule { get { return mainModule; } }

            protected ParticleSystem.EmissionModule emissionModule;
            public ParticleSystem.EmissionModule EmissionModule { get { return emissionModule; } }

            [Tooltip("Whether to animate the particle system's start speed.")]
            [SerializeField]
            protected bool animateStartSpeed;

            [Tooltip("The particle system's start speed over the duration of the animation.")]
            [SerializeField]
            protected AnimationCurve startSpeedCurve;

            /// <summary>
            /// Initialize the animated particle system.
            /// </summary>
            public virtual void Initialize()
            {
                mainModule = particleSystem.main;
                emissionModule = particleSystem.emission;
            }


            public virtual void Update(float animationPosition)
            {
                emissionModule.enabled = !Mathf.Approximately(animationPosition, 0);

                if (animateStartSpeed)
                {
                    mainModule.startSpeed = startSpeedCurve.Evaluate(animationPosition);
                }
            }
        }


        /// <summary>
        /// A transform that is animated by the engines component.
        /// </summary>
        [System.Serializable]
        public class AnimatedTransform
        {

            [Tooltip("The transform to animate.")]
            [SerializeField]
            protected Transform m_Transform;

            [Tooltip("The scale that the transform has when the control value is 0.")]
            [SerializeField]
            protected Vector3 startScale = new Vector3(1, 1, 1);

            [Tooltip("The scale that the transform has when the control value is 1.")]
            [SerializeField]
            protected Vector3 endScale = new Vector3(1, 1, 1);

            [Tooltip("The curve for transitioning between start and end values. A linear curve from (0, 0) to (1, 1) is a linear transition from start to end values.")]
            [SerializeField]
            protected AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);


            /// <summary>
            /// Called when the control value changes.
            /// </summary>
            /// <param name="animationPosition">The animation position (0-1). </param>
            public virtual void Update(float animationPosition)
            {
                float val = curve.Evaluate(animationPosition);
                m_Transform.localScale = val * endScale + (1 - val) * startScale;
            }
        }


        /// <summary>
        /// A light that is animated by the Engines component.
        /// </summary>
        [System.Serializable]
        public class AnimatedLight
        {
            [Tooltip("The animated light.")]
            public Light m_Light;

            [Tooltip("The color of the light over the course of the animation.")]
            [GradientUsage(true)]
            public Gradient lightColor;

            [Tooltip("The light intensity over the range of the engine control.")]
            public AnimationCurve lightIntensity = AnimationCurve.Linear(0, 0, 1, 1);

            public virtual void Update(float level)
            {
                m_Light.color = lightColor.Evaluate(level);
                m_Light.intensity = lightIntensity.Evaluate(level);
            }
        }


        [Tooltip("The Engines component to show visual effects for.")]
        [SerializeField]
        protected Engines engines;


        [Tooltip("The transform representing the center of mass of the vehicle.")]
        [SerializeField]
        protected Transform centerOfMass;

        public enum MovementAxis
        {
            Procedural,
            None,
            Horizontal,
            Vertical,
            Forward
        }

        public enum SteeringAxis
        {
            Procedural,
            None,
            Pitch,
            Yaw,
            Roll
        }

        [Tooltip("The steering axes to show effects for.")]
        [SerializeField]
        protected SteeringAxis steeringAxis;

        [Tooltip("The movement axes to show effects for.")]
        [SerializeField]
        protected MovementAxis movementAxis;

        public enum EngineMode
        {
            All,
            Cruising,
            Boost
        }


        [System.Serializable]
        public class EngineModeAnimationSettings
        {

            [Tooltip("The engine mode that drives the effects.")]
            public EngineMode mode;


            [Tooltip("The particles animated by the engine.")]
            public List<AnimatedParticle> animatedParticles = new List<AnimatedParticle>();


            [Tooltip("The renderers animated by the engine.")]
            public List<AnimatedRenderer> animatedRenderers = new List<AnimatedRenderer>();


            [Tooltip("The transforms animated by the engine.")]
            public List<AnimatedTransform> animatedTransforms = new List<AnimatedTransform>();


            [Tooltip("The lights animated by the engine.")]
            public List<AnimatedLight> animatedLights = new List<AnimatedLight>();



            public virtual void Initialize()
            {
                foreach (AnimatedParticle animatedParticle in animatedParticles)
                {
                    animatedParticle.Initialize();
                }

                foreach (AnimatedRenderer animatedRenderer in animatedRenderers)
                {
                    animatedRenderer.Initialize();
                }
            }


            public virtual void Update(float level)
            {
                foreach (AnimatedParticle animatedParticle in animatedParticles)
                {
                    animatedParticle.Update(level);
                }

                foreach (AnimatedRenderer animatedRenderer in animatedRenderers)
                {
                    animatedRenderer.Update(level);
                }

                foreach (AnimatedTransform animatedTransform in animatedTransforms)
                {
                    animatedTransform.Update(level);
                }

                foreach (AnimatedLight animatedLight in animatedLights)
                {
                    animatedLight.Update(level);
                }
            }
        }


        [Tooltip("The visual effects settings for the engine.")]
        public List<EngineModeAnimationSettings> settings = new List<EngineModeAnimationSettings>();


        protected virtual void Reset()
        {
            engines = GetComponent<Engines>();
        }


        protected virtual void Awake()
        {
            foreach (EngineModeAnimationSettings setting in settings)
            {
                setting.Initialize();
            }
        }


        protected virtual void LateUpdate()
        {
            if (engines == null) return;

            Vector3 thrusterLocalPos = centerOfMass.InverseTransformPoint(transform.position);
            Vector3 thrusterLocalDirection = centerOfMass.InverseTransformDirection(transform.forward);

            // Movement

            Vector3 translationAxis;
            switch (movementAxis)
            {
                case MovementAxis.Procedural:

                    translationAxis = engines.ModulatedMovementInputs;
                    break;

                case MovementAxis.Horizontal:

                    translationAxis = new Vector3(engines.ModulatedMovementInputs.x, 0, 0);
                    break;

                case MovementAxis.Vertical:

                    translationAxis = new Vector3(0, engines.ModulatedMovementInputs.y, 0);
                    break;

                case MovementAxis.Forward:

                    translationAxis = new Vector3(0, 0, engines.ModulatedMovementInputs.z);
                    break;

                default:

                    translationAxis = Vector3.zero;
                    break;

            }
            float movementAmount = Mathf.Clamp(-Vector3.Dot(thrusterLocalDirection, translationAxis), 0, 1);

            // Steering

            Vector3 rotationAxis;
            switch (steeringAxis)
            {
                case SteeringAxis.Procedural:
                    rotationAxis = engines.ModulatedSteeringInputs;
                    break;

                case SteeringAxis.Pitch:
                    rotationAxis = new Vector3(engines.ModulatedSteeringInputs.x, 0, 0);
                    break;

                case SteeringAxis.Yaw:
                    rotationAxis = new Vector3(0, engines.ModulatedSteeringInputs.y, 0);
                    break;

                case SteeringAxis.Roll:
                    rotationAxis = new Vector3(0, 0, engines.ModulatedSteeringInputs.z);
                    break;

                default:

                    rotationAxis = Vector3.zero;
                    break;

            }

            Vector3 tmp = Vector3.ProjectOnPlane(thrusterLocalPos, thrusterLocalDirection).normalized;
            if (Mathf.Abs(tmp.x) > 0.01f) tmp.x = Mathf.Sign(tmp.x) * (tmp.x / tmp.x);
            if (Mathf.Abs(tmp.y) > 0.01f) tmp.y = Mathf.Sign(tmp.y) * (tmp.y / tmp.y);
            if (Mathf.Abs(tmp.z) > 0.01f) tmp.z = Mathf.Sign(tmp.z) * (tmp.z / tmp.z);

            float steeringAmount = Mathf.Clamp(-Vector3.Dot(Vector3.Cross(rotationAxis, tmp), thrusterLocalDirection.normalized), 0, 1);

            // Calculate thruster level

            float level = Mathf.Min((movementAmount + steeringAmount), 1);


            foreach (EngineModeAnimationSettings setting in settings)
            {
                switch (setting.mode)
                {
                    case EngineMode.All:

                        setting.Update(level);
                        break;

                    case EngineMode.Cruising:

                        if (engines.ModulatedBoostInputs.magnitude < 0.5f)
                        {
                            setting.Update(level);
                        }
                        break;

                    case EngineMode.Boost:

                        if (engines.ModulatedBoostInputs.magnitude >= 0.5f)
                        {
                            setting.Update(1);
                        }
                        break;
                }
            }
        }
    }
}


