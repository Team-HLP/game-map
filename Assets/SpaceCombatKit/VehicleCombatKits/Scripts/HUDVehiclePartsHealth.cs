using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;

namespace VSX.VehicleCombatKits
{
    public class HUDVehiclePartsHealth : MonoBehaviour
    {

        [SerializeField]
        protected VehicleHealth startingTargetHealth;

        protected VehicleHealth targetHealth;

        [SerializeField]
        protected List<TrackableType> displayableTypes = new List<TrackableType>();

        [SerializeField]
        protected GameObject visualElementsParent;

        [SerializeField]
        protected List<HUDDisplayedDamageable> damageableDisplayItems = new List<HUDDisplayedDamageable>();

        [SerializeField]
        protected TargetSelector targetSelector;



        protected virtual void Start()
        {
            if (startingTargetHealth != null)
            {
                SetTarget(startingTargetHealth);
            }

            if (targetSelector != null)
            {
                targetSelector.onSelectedTargetChanged.AddListener(SetTarget);
                SetTarget(targetSelector.SelectedTarget);
            }
        }


        public virtual void SetTarget(Trackable target)
        {
            if (target == null)
            {
                SetTarget((VehicleHealth)null);
            }
            else
            {
                Trackable rootTrackable = target.RootTrackable;

                bool show = true;
                if (displayableTypes.Count > 0)
                {
                    show = false;
                    for (int i = 0; i < displayableTypes.Count; ++i)
                    {
                        if (displayableTypes[i] == rootTrackable.TrackableType)
                        {
                            show = true;
                            break;
                        }
                    }
                }

                if (show)
                {
                    VehicleHealth health = rootTrackable.GetComponent<VehicleHealth>();
                    SetTarget(health);
                }
            }
        }


        public virtual void SetTarget(VehicleHealth targetHealth)
        {
            for (int i = 0; i < damageableDisplayItems.Count; ++i)
            {
                damageableDisplayItems[i].Disconnect();
            }

            this.targetHealth = targetHealth;

            if (targetHealth == null)
            {
                visualElementsParent.SetActive(false);
                return;
            }

            visualElementsParent.SetActive(true);

            for (int i = 0; i < targetHealth.Damageables.Count; ++i)
            {
                for (int j = 0; j < damageableDisplayItems.Count; ++j)
                {
                    if (damageableDisplayItems[j].DamageableID == targetHealth.Damageables[i].DamageableID)
                    {
                        damageableDisplayItems[j].Connect(targetHealth.Damageables[i]);
                    }
                }
            }
        }
    }
}

