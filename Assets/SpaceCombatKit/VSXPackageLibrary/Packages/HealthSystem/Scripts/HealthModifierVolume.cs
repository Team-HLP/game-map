using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Health
{

    /// <summary>
    /// This component represents a 3D space (represented by either a trigger collider or a radius) that causes a change in health to objects that enter it.
    /// </summary>
    public class HealthModifierVolume : MonoBehaviour
    {

        [System.Serializable]
        public class HealthTypeChangeRate
        {
            [Tooltip("The Health Type.")]
            public HealthType healthType;

            [Tooltip("The change rate (either per second, or applied in a single instance, depending on whether Continuous Over Time is checked).")]
            public float changeRate;
        }

        [Tooltip("The type of health change that this volume will cause.")]
        public HealthModifierType healthModifierType;

        [Tooltip("The change rates for different health types.")]
        public List<HealthTypeChangeRate> healthTypeChangeRates = new List<HealthTypeChangeRate>();

        [Tooltip("If True, the health change is applied per second for as long as the object is inside the trigger collider. If False, the health change is applied once upon entering the trigger collider.")]
        [SerializeField]
        protected bool continuousPerSecond = true;

        [Tooltip("The maximum distance used by the 'Effect By Distance' curve to calculate the effect applied. The object still must be in the trigger for this to work.")]
        [SerializeField]
        protected float maxEffectDistance = 500;

        [Tooltip("The effect amount based on the distance to tha target.")]
        [SerializeField]
        protected AnimationCurve effectByDistanceCurve = AnimationCurve.Linear(0, 1, 1, 1);

        public UnityEvent onTriggered;

        protected List<Damageable> damageablesInVolume = new List<Damageable> ();



        // Called when the component is first added to a gameobject or reset in the inspector.
        protected virtual void Reset()
        {
            // Check/add rigidbody
            if (GetComponent<Rigidbody>() == null)
            {
                Rigidbody m_Rigidbody = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                m_Rigidbody.isKinematic = true;
            }

            // Check/add collider
            if (GetComponent<Collider>() == null)
            {
                SphereCollider addedCollider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
                addedCollider.radius = 500;
                maxEffectDistance = 500;
            }

            // Make sure the collider is a trigger collider
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }


        // Apply the health modification to a damage receiver
        protected virtual void ApplyEffect(Damageable damageable, bool perSecond = false)
        {

            float effectAmount = effectByDistanceCurve.Evaluate(Vector3.Distance(transform.position, damageable.GetClosestPoint(transform.position)) / maxEffectDistance);

            for (int i = 0; i < healthTypeChangeRates.Count; ++i)
            {
                if (healthTypeChangeRates[i].healthType == damageable.HealthType)
                {
                    float change = effectAmount * healthTypeChangeRates[i].changeRate * (perSecond ? Time.deltaTime : 1);

                    HealthEffectInfo info = new HealthEffectInfo();
                    info.amount = Mathf.Abs(change);
                    info.worldPosition = damageable.GetClosestPoint(transform.position);
                    info.healthModifierType = healthModifierType;
                    info.sourceRootTransform = transform;


                    if (change < 0)
                    {
                        damageable.Damage(info);
                    }
                    else if (change > 0)
                    {
                        damageable.Heal(info);
                    }

                    return;
                }
            }

            onTriggered.Invoke();
        }


        // Called when another collider enters the trigger collider
        protected virtual void OnTriggerEnter(Collider other)
        {
            
            DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
            if (damageReceiver != null)
            {
                if (damageReceiver.Damageable != null)
                {
                    if (damageablesInVolume.IndexOf(damageReceiver.Damageable) == -1)
                    {
                        damageablesInVolume.Add(damageReceiver.Damageable);

                        if (!continuousPerSecond)
                        {
                            ApplyEffect(damageReceiver.Damageable, false);
                        }
                    }
                }
            }
        }


        // Called when another collider exits the trigger collider
        protected virtual void OnTriggerExit(Collider other)
        {

            DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
            if (damageReceiver != null)
            {
                if (damageReceiver.Damageable != null)
                {
                    damageablesInVolume.Remove(damageReceiver.Damageable);
                }
            }
        }


        /// <summary>
        /// Trigger the damage/healing effect.
        /// </summary>
        /// <param name="perSecond">Whether to apply the effect value per second or as a single instantaneous full effect.</param>
        public virtual void Trigger(bool perSecond = false)
        {
            foreach (Damageable damageable in damageablesInVolume)
            {
                ApplyEffect(damageable, perSecond);
            }
        }


        protected virtual void Update()
        {
            if (continuousPerSecond)
            {
                foreach (Damageable damageable in damageablesInVolume)
                {
                    ApplyEffect(damageable, true);
                }
            }
        }
    }
}
