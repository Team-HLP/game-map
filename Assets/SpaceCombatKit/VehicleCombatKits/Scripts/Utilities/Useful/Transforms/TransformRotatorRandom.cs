using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Rotates a transform's local rotation with a random 3D rotation when activated.
    /// </summary>
    public class TransformRotatorRandom : MonoBehaviour
    {
        [Tooltip("The transform being rotated (locally).")]
        [SerializeField]
        protected Transform rotationTarget;
        protected Vector3 targetRotations;    // The target rotation speeds
        protected Vector3 currentRotations = Vector3.zero; // The next rotation speeds 


        [Tooltip("Whether to reset the local rotation of the transform when this component is enabled in the scene.")]
        [SerializeField]
        protected bool resetTransformOnEnable = true;


        [Tooltip("The minimum and maximum values for the random rotation around the local X axis. Does not take rotation direction into account.")]
        [SerializeField]
        protected Vector2 minMaxRotationsX;


        [Tooltip("The minimum and maximum values for the random rotation around the local Y axis. Does not take rotation direction into account.")]
        [SerializeField]
        protected Vector2 minMaxRotationsY;


        [Tooltip("The minimum and maximum values for the random rotation around the local Z axis. Does not take rotation direction into account.")]
        [SerializeField]
        protected Vector2 minMaxRotationsZ;


        [Tooltip("How fast the rotation lerps toward the target rotation speed from its current rotation speed.")]
        [SerializeField]
        protected float lerpSpeed = 3;

        protected bool activated = false;

        


        protected virtual void OnEnable()
        {
            if (resetTransformOnEnable)
            {
                ResetTransform();
            }
        }


        /// <summary>
        /// Activate the random rotation.
        /// </summary>
        public virtual void Activate()
        {
            activated = true;

            targetRotations = new Vector3(Random.Range(minMaxRotationsX.x, minMaxRotationsX.y), Random.Range(minMaxRotationsY.x, minMaxRotationsY.y), Random.Range(minMaxRotationsZ.x, minMaxRotationsZ.y));
            targetRotations.x *= -1 + Random.Range(0, 2) * 2;
            targetRotations.y *= -1 + Random.Range(0, 2) * 2;
            targetRotations.y *= -1 + Random.Range(0, 2) * 2;
        }


        /// <summary>
        /// Deactivate the random rotation.
        /// </summary>
        public virtual void Deactivate()
        {
            activated = false;
        }


        /// <summary>
        /// Reset the rotated transform's local rotation.
        /// </summary>
        public virtual void ResetTransform()
        {
            rotationTarget.localRotation = Quaternion.identity;
            currentRotations = Vector3.zero;
            activated = false;
        }


        // Update is called once per frame
        void Update()
        {
            if (activated)
            {
                currentRotations = Vector3.Lerp(currentRotations, targetRotations, lerpSpeed * Time.deltaTime);
                rotationTarget.Rotate(currentRotations * Time.deltaTime);
            }
        }
    }

}
