using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Rumbles
{
    /// <summary>
    /// This class enables you to create a rumble when a gameobject is enabled, or call the rumble by adding a function from this script to a Unity Event.
    /// </summary>
    public class AddRumble : MonoBehaviour
    {

        [Header("Settings")]

        [Tooltip("Whether to begin the rumble when the gameobject is activated.")]
        [SerializeField]
        protected bool runOnEnable = true;
        public virtual bool RunOnEnable
        {
            get => runOnEnable;
            set => runOnEnable = value;
        }

        [Header("Rumble Parameters")]

        [Tooltip("Whether the rumble is based on distance or global (felt equally regardless of distance).")]
        [SerializeField]
        protected bool distanceBased = true;
        public virtual bool DistanceBased
        {
            get => distanceBased;
            set => distanceBased = value;
        }

        [Tooltip("The delay before the rumble occurs after it is called.")]
        [SerializeField]
        protected float delay = 0;
        public virtual float Delay
        {
            get => delay;
            set => delay = value;
        }

        [Tooltip("The peak rumble level.")]
        [SerializeField]
        protected float maxLevel = 1;
        public virtual float MaxLevel
        {
            get => maxLevel;
            set => maxLevel = value;
        }

        [Tooltip("The rumble duration.")]
        [SerializeField]
        protected float duration = 1;
        public virtual float Duration
        {
            get => duration;
            set => duration = value;
        }

        [Tooltip("How long the rumble takes to go from 0 to maximum.")]
        [SerializeField]
        protected AnimationCurve rumbleCurve = AnimationCurve.Linear(0, 1, 1, 0);
        public virtual AnimationCurve RumbleCurve
        {
            get => rumbleCurve;
            set => rumbleCurve = value;
        }



        protected virtual void OnEnable()
        {
            if (runOnEnable)
            {
                Run();
            }
        }


        /// <summary>
        /// Add this rumble.
        /// </summary>
        public virtual void Run()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RunCoroutine(maxLevel, delay));
            }
        }


        /// <summary>
        /// Add this rumble with a specified delay.
        /// </summary>
        /// <param name="delay">The default delay before the rumble.</param>
        public virtual void Run(float delay)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RunCoroutine(maxLevel, delay));
            }
        }


        /// <summary>
        /// Run this rumble with a custom max level (0 - 1) and a specified delay.
        /// </summary>
        /// <param name="customMaxLevel">The custom max level (0 - 1).</param>
        /// <param name="delay">The delay before the rumble begins.</param>
        public virtual void Run(float customMaxLevel, float delay)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RunCoroutine(customMaxLevel, delay));
            }
        }


        // The delayed rumble coroutine.
        protected IEnumerator RunCoroutine(float thisMaxLevel, float delay)
        {

            yield return new WaitForSeconds(delay);
         
            // Add a rumble
            if (RumbleManager.Instance != null)
            {
                RumbleManager.Instance.AddRumble(distanceBased, transform.position, thisMaxLevel, duration, rumbleCurve);
            }
        }
    }
}