using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Health
{
    public class DamageableThresholdTrigger : MonoBehaviour
    {
        [Range(0, 1)]
        public float minValue;

        [Range(0, 1)]
        public float maxValue;

        public Damageable damageable;

        bool triggered = false;

        public UnityEvent onTriggered;
        public UnityEvent onReset;


        private void OnValidate()
        {
            minValue = Mathf.Min(minValue, maxValue);
        }


        private void Awake()
        {
            damageable.onHealthChanged.AddListener(OnHealthChanged);

            OnReset();
        }


        void OnHealthChanged()
        {
            if (damageable.CurrentHealthFraction >= minValue && damageable.CurrentHealthFraction <= maxValue)
            {
                if (!triggered)
                {
                    triggered = true;
                    OnTriggered();
                }
            }
            else
            {
                if (triggered)
                {
                    triggered = false;
                    OnReset();
                }
                
            }
        }


        void OnTriggered()
        {
            onTriggered.Invoke();
        }


        void OnReset()
        {
            onReset.Invoke();
        }
    }
}

