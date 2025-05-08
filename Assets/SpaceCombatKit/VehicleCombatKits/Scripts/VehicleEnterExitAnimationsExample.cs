using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Example script for playing animations for entering/exiting a vehicle.
    /// </summary>
    public class VehicleEnterExitAnimationsExample : VehicleEnterExitAnimations
    {
        [SerializeField]
        protected VehicleCamera cam;

        [SerializeField]
        protected AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField]
        protected float animationDuration = 2;


        /// <summary>
        /// Begin animation for moving from one vehicle to another.
        /// </summary>
        /// <param name="agent">The game agent involved.</param>
        /// <param name="from">The vehicle being exited.</param>
        /// <param name="to">The vehicle being entered.</param>
        public override void Animate(GameAgent agent, Vehicle fromVehicle, Vehicle toVehicle)
        {
            StartCoroutine(AnimationCoroutine(agent, fromVehicle, toVehicle));
        }


        // Coroutine that smoothly animates camera while entering/exiting.
        IEnumerator AnimationCoroutine(GameAgent agent, Vehicle fromVehicle, Vehicle toVehicle)
        {
            agent.ExitAllVehicles();

            CameraViewTarget toCameraViewTarget = cam.GetDefaultCameraViewTarget(toVehicle);

            Quaternion startRotation = cam.transform.rotation;
            Quaternion endRotation = toCameraViewTarget.transform.rotation;

            Vector3 startPosition = cam.transform.position;
            Vector3 endPosition = toCameraViewTarget.transform.position;

            float startTime = Time.time;

            while (true)
            {
                float amount = (Time.time - startTime) / animationDuration;

                bool exit = false;
                if (amount >= 1)
                {
                    amount = 1;
                    exit = true;
                }

                cam.transform.position = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(amount));
                cam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, curve.Evaluate(amount));

                if (exit)
                {
                    break;
                }

                yield return null;
            }

            agent.EnterVehicle(toVehicle);
        }
    }
}

