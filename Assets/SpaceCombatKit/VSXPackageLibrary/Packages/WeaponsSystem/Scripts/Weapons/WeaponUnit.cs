using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Health;

namespace VSX.Weapons
{
    /// <summary>
    /// Base class for a weapon unit that makes up part of a weapon.
    /// </summary>
    public class WeaponUnit : MonoBehaviour
    {

        protected virtual void Reset()
        {
            Weapon weapon = GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                if (!weapon.WeaponUnits.Contains(this))
                {
                    weapon.WeaponUnits.Add(this);
                }
            }
        }

        /// <summary>
        /// Start triggering the weapon unit.
        /// </summary>
        public virtual void StartTriggering() { }


        /// <summary>
        /// Stop triggering the weapon unit.
        /// </summary>
        public virtual void StopTriggering() { }


        /// <summary>
        /// Check if the weapon unit can be triggered.
        /// </summary>
        public virtual bool CanTrigger
        {
            get { return true; }
        }


        /// <summary>
        /// Trigger the weapon unit once.
        /// </summary>
        public virtual void TriggerOnce() { }


        public virtual bool TimeBasedDamageHealing
        {
            get { return false; }
            set { }
        }


        /// <summary>
        /// Set the damage multiplier for this weapon unit.
        /// </summary>
        /// <param name="damageMultiplier">The damage multiplier.</param>
        public virtual void SetDamageMultiplier(float damageMultiplier) { }


        /// <summary>
        /// Get the damage for this weapon unit (typically in DPS - Damage Per Second).
        /// </summary>
        /// <param name="healthType">The health type to get damage for.</param>
        /// <returns>The damage done for a particular health type.</returns>
        public virtual float Damage(HealthType healthType)
        {
            return 0;
        }


        /// <summary>
        /// Set the healing multiplier for this weapon unit.
        /// </summary>
        /// <param name="healingMultiplier">The healing multiplier.</param>
        public virtual void SetHealingMultiplier(float healingMultiplier) { }


        /// <summary>
        /// Get the amount of healing this weapon unit does (typically in hit points per second).
        /// </summary>
        /// <param name="healthType">The health type to get healing for.</param>
        /// <returns>The healing done for a particular health type.</returns>
        public virtual float Healing(HealthType healthType)
        {
            return 0;
        }


        /// <summary>
        /// Get the weapon unit speed.
        /// </summary>
        public virtual float Speed
        {
            get { return 0; }
        }


        /// <summary>
        /// Get the range of this weapon unit.
        /// </summary>
        public virtual float Range
        {
            get { return 0; }
        }

        [SerializeField]
        protected bool aimAssistEnabled = true;
        public bool AimAssistEnabled
        {
            get { return aimAssistEnabled; }
            set { aimAssistEnabled = value; }
        }

        /// <summary>
        /// Aim the weapon unit at a world position.
        /// </summary>
        /// <param name="aimPosition">The world position to aim the weapon unit at.</param>
        public virtual void Aim(Vector3 aimPosition) { }


        /// <summary>
        /// Clear any aiming currently implemented on this weapon unit.
        /// </summary>
        public virtual void ClearAim() { }
    }
}
