using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Utilities;
using VSX.UI;
using VSX.RadarSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using VSX.Health;

namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Displays information about the currently selected target on the HUD.
    /// </summary>
    public class HUDTargetInfo : MonoBehaviour, IRootTransformUser
    {
        [Tooltip("The target selector whose selected target is displayed.")]
        [SerializeField]
        protected TargetSelector targetSelector;

        [Tooltip("The handle that is toggled to show or hide the target information.")]
        [SerializeField]
        protected GameObject handle;

        [Tooltip("The color manager that controls the color for UI elements when targets from different teams/factions are displayed.")]
        [SerializeField]
        protected UIColorManager colorManager;

        [Tooltip("The default color used for the target information when no color information is provided by the target.")]
        [SerializeField]
        protected Color defaultColor = Color.white;

        [Tooltip("The text displaying the target's label.")]
        [SerializeField]
        protected UVCText targetLabelText;

        [Tooltip("The text displaying the target faction.")]
        [SerializeField]
        protected UVCText targetFactionText;

        [Tooltip("The text displaying the distance to the target.")]
        [SerializeField]
        protected UVCText distanceText;

        [Tooltip("The text displaying the speed of the target.")]
        [SerializeField]
        protected UVCText speedText;

        [SerializeField]
        protected Transform rootTransform;
        public virtual Transform RootTransform { set { rootTransform = value; } }


        /// <summary>
        /// Displays health information for a specific health type on the HUD.
        /// </summary>
        [System.Serializable]
        public class HealthDisplay
        {
            public GameObject handle;
            public HealthType healthType;
            public UVCText healthRemainingText;
            public Image fillBar;

            [Tooltip("Called when the displayed target is damaged.")]
            public UnityEvent onDamage;

            [Tooltip("Called when the displayed target heals.")]
            public UnityEvent onHealing;

            protected float lastHealthValue;


            /// <summary>
            /// Initialize the health display (don't run events).
            /// </summary>
            /// <param name="currentHealth">The current health of the target.</param>
            /// <param name="healthCapacity">The health capacity of the target.</param>
            public virtual void Initialize(float currentHealth, float healthCapacity)
            {
                UpdateInternal(currentHealth, healthCapacity);
            }


            /// <summary>
            /// Update the health display with events.
            /// </summary>
            /// <param name="currentHealth">The current health of the target.</param>
            /// <param name="healthCapacity">The health capacity of the target.</param>
            public virtual void Update(float currentHealth, float healthCapacity)
            {
                if (Mathf.Abs(currentHealth - lastHealthValue) > 0.0001f)
                {
                    if (currentHealth > lastHealthValue)
                    {
                        onHealing.Invoke();
                    }
                    else
                    {
                        onDamage.Invoke();
                    }
                }

                UpdateInternal(currentHealth, healthCapacity);
            }


            /// <summary>
            /// Internal update of health display (does not include events).
            /// </summary>
            /// <param name="currentHealth">The current health of the target.</param>
            /// <param name="healthCapacity">The health capacity of the target.</param>
            protected virtual void UpdateInternal(float currentHealth, float healthCapacity)
            {
                if (healthRemainingText != null)
                {
                    healthRemainingText.text = currentHealth.ToString();
                }

                if (fillBar != null)
                {
                    fillBar.fillAmount = healthCapacity < 0.0001f ? 0 : Mathf.Clamp(currentHealth / healthCapacity, 0, 1);
                }

                lastHealthValue = currentHealth;
            }
        }


        [Tooltip("The health displays available for displaying target health information.")]
        [SerializeField]
        protected List<HealthDisplay> healthDisplays = new List<HealthDisplay>();

        protected Trackable target;
        protected IHealthInfo healthInfo;



        protected virtual void Awake()
        {
            handle.SetActive(false);
            if (targetSelector != null) targetSelector.onSelectedTargetChanged.AddListener(OnTargetSelected);
        }


        // Called when the selected target changes.
        protected virtual void OnTargetSelected(Trackable newTarget)
        {
            if (healthInfo != null)
            {
                healthInfo.OnHealthChanged.RemoveListener(UpdateHealthDisplays);
            }

            target = newTarget;

            if (newTarget == null)
            {
                target = null;
                healthInfo = null;
                handle.SetActive(false);
                return;
            }
            else
            {
                handle.SetActive(true);

                // Update label

                if (targetLabelText != null)
                {
                    targetLabelText.text = newTarget.Label;
                }

                // Update faction label

                if (targetFactionText != null)
                {
                    if (newTarget.Team != null)
                    {
                        targetFactionText.gameObject.SetActive(true);
                        targetFactionText.text = newTarget.Team.name;
                    }
                    else
                    {
                        targetFactionText.gameObject.SetActive(false);
                    }
                }

                // Update colors

                if (colorManager != null)
                {
                    if (newTarget.Team != null)
                    {
                        colorManager.SetColor(newTarget.Team.DefaultColor);
                    }
                    else
                    {
                        colorManager.SetColor(defaultColor);
                    }
                }

                // Initialize health displays

                for (int i = 0; i < healthDisplays.Count; ++i)
                {
                    healthDisplays[i].handle.SetActive(false);
                }

                if (newTarget.Rigidbody != null)
                {
                    healthInfo = newTarget.Rigidbody.transform.GetComponentInChildren<IHealthInfo>();
                }
                else
                {
                    healthInfo = newTarget.GetComponentInChildren<IHealthInfo>();
                }
                

                if (healthInfo != null)
                {
                    healthInfo.OnHealthChanged.AddListener(UpdateHealthDisplays);

                    for (int i = 0; i < healthDisplays.Count; ++i)
                    {
                        if (!Mathf.Approximately(healthInfo.GetHealthCapacityByType(healthDisplays[i].healthType), 0))
                        {
                            healthDisplays[i].handle.SetActive(true);

                            healthDisplays[i].Initialize(healthInfo.GetCurrentHealthByType(healthDisplays[i].healthType), healthInfo.GetHealthCapacityByType(healthDisplays[i].healthType));
                        }
                    }
                }

                // Update distance and speed

                UpdateDistanceText();
                UpdateSpeedText();
            }
        }


        // Update the text displaying the distance to the target.
        protected virtual void UpdateDistanceText()
        {
            if (distanceText != null)
            {
                distanceText.text = Mathf.RoundToInt(Vector3.Distance(rootTransform.position, target.transform.position)).ToString() + " M";
            }
        }


        // Update the text displaying the target's speed.
        protected virtual void UpdateSpeedText()
        {
            if (speedText != null)
            {
                if (target.Rigidbody != null)
                {
                    speedText.gameObject.SetActive(true);
                    speedText.text = Mathf.RoundToInt(target.Rigidbody.velocity.magnitude).ToString() + " M/S";
                }
                else
                {
                    speedText.gameObject.SetActive(false);
                }
            }
        }
       

        // Update the health displays
        protected virtual void UpdateHealthDisplays()
        {
            for (int i = 0; i < healthDisplays.Count; ++i)
            {
                healthDisplays[i].Update(healthInfo.GetCurrentHealthByType(healthDisplays[i].healthType), healthInfo.GetHealthCapacityByType(healthDisplays[i].healthType));
            }
        }


        protected virtual void Update()
        {
            if (target != null)
            {
                UpdateDistanceText();
                UpdateSpeedText();
            }
        }
    }
}

