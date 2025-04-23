using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.CameraSystem
{
    /// <summary>
    /// Shake the camera by rotating the view direction by small amounts.
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {

        [Tooltip("The transform to be shaken.")]
        [SerializeField]
        public Transform cameraTransform;


        [Header("Shake Parameters")]


        [Tooltip("The maximum shake vector length (the camera is rotated by the angle between the original view direction and a view direction offset by this amount).")]
        [SerializeField]
        protected float maxShakeVectorLength = 0.05f;


        protected virtual void Awake()
        {
            StartCoroutine(ResetRotationCoroutine());
        }


        /// <summary>
        /// Shake the camera for one frame.
        /// </summary>
        /// <param name="shakeStrength">The shake strength.</param>
        public virtual void SingleFrameShake(float shakeStrength)
        {
            
            // Get a random vector on the xy plane
            Vector3 localShakeVector = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f).normalized;

            // Scale according to desired shake magnitude
            localShakeVector *= shakeStrength * maxShakeVectorLength;

            // Calculate the look target 
            Vector3 shakeLookTarget = cameraTransform.TransformPoint(Vector3.forward + localShakeVector);
            
            // Look at the target
            cameraTransform.LookAt(shakeLookTarget, transform.up);

        }      


        // Reset the camera local rotation at the end of the frame.
        IEnumerator ResetRotationCoroutine()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                cameraTransform.localRotation = Quaternion.identity;
            }
        }
    }
}