using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Health
{
    /// <summary>
    /// Runs events when a health modification occurs to damageable(s) with a specific health modifier type. E.g. used to destroy immediately when missile
    /// hits vs detonate slowly/spin out of control when gun hits.
    /// </summary>
    public class HealthModifierTypeEvents : MonoBehaviour
    {
        [Tooltip("The damageables to run events for.")]
        [SerializeField]
        protected List<Damageable> damageables = new List<Damageable>();

        /// <summary>
        /// Runs events when a specified health modifier type is responsible for a health modification to one of the damageables.
        /// </summary>
        [System.Serializable]
        public class HealthModifierTypeEventItem
        {
            [Tooltip("The health modifier type associated with the following events.")]
            public HealthModifierType healthModifierType;

            [Tooltip("Event called when the damageable is damaged by the above health modifier type.")]
            public UnityEvent onDamaged;

            [Tooltip("Event called when the damageable is healed by the above health modifier type.")]
            public UnityEvent onHealed;

            [Tooltip("Event called when the damageable is destroyed by the above health modifier type.")]
            public UnityEvent onDestroyed;
        }


        [Tooltip("The damageables to run events for.")]
        [SerializeField] 
        protected List<HealthModifierTypeEventItem> healthModifierTypeEventItems = new List<HealthModifierTypeEventItem>();
        protected HealthModifierType lastDamageHealthModifierType;  // Store the last health modifier type that damaged the object, to know which type caused the destruction.



        protected virtual void Awake()
        {
            for (int i = 0; i < damageables.Count; ++i)
            {
                Damageable damageable = damageables[i];

                damageable.onDamaged.AddListener((healthEffectInfo) => { OnDamaged(damageable, healthEffectInfo); });
                damageable.onHealed.AddListener((healthEffectInfo) => { OnHealed(damageable, healthEffectInfo); });
                damageable.onDestroyed.AddListener(() => { OnDestroyed(damageable); });
            }
        }


        // Called when one of the damageables is destroyed.
        protected virtual void OnDestroyed(Damageable damageable)
        {
            for (int i = 0; i < healthModifierTypeEventItems.Count; ++i)
            {
                if (healthModifierTypeEventItems[i].healthModifierType == lastDamageHealthModifierType)
                {
                    healthModifierTypeEventItems[i].onDestroyed.Invoke();
                }
            }
        }


        // Called when one of the damageables is damaged.
        protected virtual void OnDamaged(Damageable damageable, HealthEffectInfo info)
        {
            lastDamageHealthModifierType = info.healthModifierType;

            for (int i = 0; i < healthModifierTypeEventItems.Count; ++i)
            {
                if (healthModifierTypeEventItems[i].healthModifierType == info.healthModifierType)
                {
                    healthModifierTypeEventItems[i].onDamaged.Invoke();
                }
            }
        }


        // Called when one of the damageables is healed.
        protected virtual void OnHealed(Damageable damageable, HealthEffectInfo info)
        {
            for (int i = 0; i < healthModifierTypeEventItems.Count; ++i)
            {
                if (healthModifierTypeEventItems[i].healthModifierType == info.healthModifierType)
                {
                    healthModifierTypeEventItems[i].onHealed.Invoke();
                }
            }
        }
    }
}
