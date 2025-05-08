using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VSX.Health;

namespace VSX.VehicleCombatKits
{
    public class HUDDisplayedDamageable : MonoBehaviour
    {

        [SerializeField]
        protected string damageableID;
        public string DamageableID
        {
            get { return damageableID; }
        }

        protected Damageable connectedDamageable;

        [SerializeField]
        protected GameObject visualElements;

        [SerializeField]
        protected List<Image> images = new List<Image>();

        public Gradient healthColorGradient;

        public Color destroyedColor = new Color(0f, 0f, 0f, 0.33f);

        [Header("Events")]

        public UnityEvent onDamaged;
        public UnityEvent onHealed;

        protected UnityAction<HealthEffectInfo> onDamagedAction;
        protected UnityAction<HealthEffectInfo> onHealedAction;



        protected virtual void Reset()
        {
            visualElements = gameObject;

            healthColorGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[] { new GradientColorKey(new Color(1, 0.1f, 0.1f, 1f), 0f), new GradientColorKey(new Color(1, 0.75f, 0.25f, 1f), 1f) };
            healthColorGradient.colorKeys = colorKeys;

            images = new List<Image>(transform.GetComponentsInChildren<Image>());
        }


        protected virtual void Awake()
        {
            if (connectedDamageable == null)
            {
                visualElements.SetActive(false);
            }
        }


        public void Connect(Damageable damageable)
        {
            Disconnect();
            connectedDamageable = damageable;

            onDamagedAction = delegate { OnDamaged(); };
            damageable.onDamaged.AddListener(onDamagedAction);

            onHealedAction = delegate { OnHealed(); };
            damageable.onHealed.AddListener(onHealedAction);

            damageable.onDestroyed.AddListener(UpdateUI);
            damageable.onRestored.AddListener(UpdateUI);

            visualElements.SetActive(true);

            UpdateUI();
        }

        public void Disconnect()
        {
            if (connectedDamageable != null)
            {
                connectedDamageable.onDamaged.RemoveListener(onDamagedAction);
                connectedDamageable.onHealed.RemoveListener(onHealedAction);

                connectedDamageable.onDestroyed.RemoveListener(UpdateUI);
                connectedDamageable.onRestored.RemoveListener(UpdateUI);
            }

            visualElements.SetActive(false);
        }


        protected virtual void OnDamaged()
        {
            UpdateUI();
            onDamaged.Invoke();
        }


        protected virtual void OnHealed()
        {
            UpdateUI();
            onHealed.Invoke();
        }


        protected void UpdateUI()
        {
            float healthFraction = connectedDamageable.HealthCapacity == 0 ? 0 : (connectedDamageable.CurrentHealth / connectedDamageable.HealthCapacity);

            for (int i = 0; i < images.Count; ++i)
            {
                if (connectedDamageable.Destroyed)
                {
                    images[i].color = destroyedColor;
                }
                else
                {
                    images[i].color = healthColorGradient.Evaluate(healthFraction);
                }
            }

            onDamaged.Invoke();
        }
    }
}

