using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;

namespace VSX.Health
{
    /// <summary>
    /// Controls a health bar that uses Unity's UI.
    /// </summary>
    public class HealthFillBarController : MonoBehaviour
    {

        [Tooltip("The fill bar.")]
        [SerializeField]
        protected UIFillBar fillBar;

        [Tooltip("The health type that is health bar should display.")]
        [SerializeField]
        protected HealthType healthType;
        public HealthType HealthType { get { return healthType; } }

        [Tooltip("The health component that this health bar displays information for. Must be a component implementing the IHealthInfo interface, such as Health or Damageable.")]
        [SerializeField]
        protected Component healthInfoComponent;

        protected IHealthInfo healthInfo;


        private void OnValidate()
        {
            if (healthInfoComponent == null)
            {
                healthInfo = null;
            }
            else
            {

                Component[] components = healthInfoComponent.gameObject.GetComponents<Component>();
                healthInfoComponent = null;

                foreach(Component c in components)
                {
                    healthInfo = c as IHealthInfo;
                    if (healthInfo != null)
                    {
                        healthInfoComponent = c;
                        break;
                    }
                }
            }
        }


        protected virtual void Awake()
        {
            healthInfo = healthInfoComponent as IHealthInfo;
        }


        // Called every frame
        protected virtual void Update()
        {
            // Update the health bar
            if (healthInfo != null)
            {
                fillBar.SetFill(healthInfo.GetCurrentHealthFractionByType(healthType));
            }
        }
    }
}