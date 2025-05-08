using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Rumbles;
using VSX.UI;

namespace VSX.Characters
{
    /// <summary>
    /// Controls the jetpack fuel consumption
    /// </summary>
    public class JetpackFuel : MonoBehaviour
    {

        [Tooltip("The Rigidbody Character Controller that the jetpack is on.")]
        [SerializeField]
        protected RigidbodyCharacterController characterController;

        [Tooltip("The maximum fuel capacity.")]
        [SerializeField]
        protected float maxFuel = 100;
        public virtual float FuelCapacity { get => maxFuel; }

        [Tooltip("The rate that fuel is replenished over time (per second).")]
        [SerializeField]
        protected float rechargeRate = 20;
        public virtual float RechargeRate { get => rechargeRate; }

        [Tooltip("The rate that fuel is used when jetpacking (per second).")]
        [SerializeField]
        protected float usageRate = 20;
        public virtual float UsageRate { get => usageRate; }

        protected float currentFuel;

        [Tooltip("The fuel gauge UI.")]
        [SerializeField]
        protected UIFillBar fuelGauge;

        [Tooltip("The rumble level when the jetpack is on.")]
        [SerializeField]
        protected float jetpackRumbleLevel = 0.2f;


        protected virtual void Awake()
        {
            currentFuel = maxFuel;
        }


        protected virtual void Update()
        {
            if (characterController.Jetpacking)
            {
                currentFuel = Mathf.Clamp(currentFuel - usageRate * Time.deltaTime, 0, maxFuel);
                if (RumbleManager.Instance != null) RumbleManager.Instance.AddSingleFrameRumble(jetpackRumbleLevel, transform.position);
            }
            else
            {
                currentFuel = Mathf.Clamp(currentFuel + rechargeRate * Time.deltaTime, 0, maxFuel);
            }

            characterController.JetpackingEnabled = currentFuel > 0;

            fuelGauge.SetFill(maxFuel == 0 ? 0 : currentFuel / maxFuel);
        }
    }
}

