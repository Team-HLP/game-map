using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.Vehicles.ControlAnimations
{
    /// <summary>
    /// Animate a vehicle transform.
    /// </summary>
    public class VehicleTransformAnimation : VehicleControlAnimation
    {

        Quaternion rotation = Quaternion.Euler(0, 0, 0);


        [Tooltip("The local rotation applied when the animation value is 1.")]
        [SerializeField]
        protected Vector3 rotationAmount;


        [Tooltip("The animation value over the duration of the animation.")]
        [SerializeField]
        protected AnimationCurve rotationCurve;


        [Tooltip("The duration of the animation.")]
        [SerializeField]
        protected float duration = 1;


        [Tooltip("The delay before the animation begins, after it is started.")]
        [SerializeField]
        protected float delay = 0;


        /// <summary>
        /// Get the current rotation for the animation.
        /// </summary>
        /// <returns></returns>
        public override Quaternion GetRotation()
        {
            return rotation;
        }


        /// <summary>
        /// Start the animation
        /// </summary>
        public virtual void Action()
        {
            StartCoroutine(AnimationCoroutine());
        }


        // The animation coroutine
        protected IEnumerator AnimationCoroutine()
        {
            yield return new WaitForSeconds(delay);

            float startTime = Time.time;
            while (true)
            {
                float amount = (Time.time - startTime) / duration;
                if (amount >= 1)
                {
                    rotation = Quaternion.Euler(rotationAmount * rotationCurve.Evaluate(1));
                    break;
                }
                else
                {
                    rotation = Quaternion.Euler(rotationAmount * rotationCurve.Evaluate(amount));
                }

                yield return null;
            }
        }
    }
}

