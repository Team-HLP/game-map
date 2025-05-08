using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;
using VSX.Vehicles;
using VSX.Utilities;
using UnityEngine.Events;


namespace VSX.Weapons
{
    /// <summary>
    /// Manages the weapons loaded on a vehicle.
    /// </summary>
    [DefaultExecutionOrder(30)]
    public class WeaponsController : ModuleManager, ILeadTargetInfo
    {

        [Tooltip("The aim controller used for aim assist and other aim-based functionality.")]
        [SerializeField]
        protected AimControllerBase aimController;

        protected List<GunWeapon> gunWeapons = new List<GunWeapon>();
        public List<GunWeapon> GunWeapons
        {
            get { return gunWeapons; }
        }

        public virtual float MaxGunWeaponRange
        {
            get
            {
                float maxRange = 0;
                for(int i = 0; i < gunWeapons.Count; ++i)
                {
                    maxRange = Mathf.Max(maxRange, gunWeapons[i].Range);
                }

                return maxRange;
            }
        }

        protected List<MissileWeapon> missileWeapons = new List<MissileWeapon>();
        public List<MissileWeapon> MissileWeapons
        {
            get { return missileWeapons; }
        }

        [Tooltip("The target selector that the weapons use for lead target calculation and missile locking.")]
        [SerializeField]
        protected TargetSelector weaponsTargetSelector;
        public TargetSelector WeaponsTargetSelector { get { return weaponsTargetSelector; } }


        [Tooltip("Whether to snap the weapon aim toward a target when it is within a specified angle of the aim direction.")]
        [SerializeField]
        protected bool aimAssist = true;

        protected bool aimAssistState;
        public bool AimAssistState { get => aimAssistState; }

        [Tooltip("The maximum angle within which aim assist occurs.")]
        [SerializeField]
        protected float aimAssistAngle = 3;

        public enum AimAssistLeadTargetMode
        {
            MaxSpeed,
            MinSpeed,
            Average
        }

        [Tooltip("The criteria for choosing which weapon governs the aim assist implementation.")]
        [SerializeField]
        protected AimAssistLeadTargetMode aimAssistLeadTargetMode;

        [Tooltip("Whether to always aim the weapons at wherever the Aim Controller is aiming.")]
        [SerializeField]
        protected bool aimWeaponsAtAimPosition = true;

        [Tooltip("The maximum average aim angle that the weapons can take while aiming at the Aim Controller's aim position.")]
        [SerializeField]
        protected float maxAverageWeaponAimAngle = 45;

        [Header("Turrets")]

        [Tooltip("Whether turrets should snap to the target (rather that smoothly rotating toward it).")]
        [SerializeField]
        protected bool snapTurretsToTarget = false;

        public UnityAction onAimAssistUpdated;


        public virtual Transform Target
        {
            get
            {
                if (weaponsTargetSelector != null && weaponsTargetSelector.SelectedTarget != null)
                {
                    return weaponsTargetSelector.SelectedTarget.transform;
                }
                else
                {
                    return null;
                }
            }
        }

        protected List<Vector3> leadTargetPositions = new List<Vector3>();
        public List<Vector3> LeadTargetPositions 
        { 
            get 
            { 
                if (weaponsTargetSelector != null && weaponsTargetSelector.SelectedTarget != null)
                {
                    return leadTargetPositions;
                }
                else
                {
                    return null;
                }
            } 
        }


        protected int leadTargetAimedIndex = -1;
        public virtual int LeadTargetAimedIndex
        {
            get { return leadTargetAimedIndex; }
        }


        protected float aimStateStartTime;
        public virtual float AimStateStartTime
        {
            get { return aimStateStartTime; }
        }


        protected override void Awake()
        {
            base.Awake();

            if (weaponsTargetSelector != null)
            {
                weaponsTargetSelector.onSelectedTargetChanged.AddListener((selectedTarget) => { UpdateLeadTargetPositions(true); });
            }
        }


        protected override void Start()
        {
            base.Start();

            Weapon[] weapons = GetComponentsInChildren<Weapon>();
            foreach(Weapon weapon in weapons)
            {
                AddWeapon(weapon);
            }
        }


        // Called when a module is mounted on one of the vehicle's module mounts.
        protected override void OnModuleMounted(Module module)
        { 
            // Look for gun weapons
            GunWeapon gunWeapon = module.GetComponentInChildren<GunWeapon>();
            if (gunWeapon != null)
            {
                AddWeapon(gunWeapon);
            }

            MissileWeapon missileWeapon = module.GetComponentInChildren<MissileWeapon>();
            if (missileWeapon != null)
            {
                AddWeapon(missileWeapon);
            }
        }


        /// <summary>
        /// Store a reference to a weapon.
        /// </summary>
        /// <param name="weapon">The weapon.</param>
        protected virtual void AddWeapon(Weapon weapon)
        {
            GunWeapon gunWeapon = weapon as GunWeapon;
            if (gunWeapon != null)
            {
                AddWeapon(gunWeapon);
            }

            MissileWeapon missileWeapon = weapon as MissileWeapon;
            if (missileWeapon != null)
            {
                AddWeapon(missileWeapon);
            }
        }


        /// <summary>
        /// Store a reference to a gun weapon.
        /// </summary>
        /// <param name="gunWeapon">The gun weapon.</param>
        protected virtual void AddWeapon(GunWeapon gunWeapon)
        {
            // Store gun weapon reference
            if (!gunWeapons.Contains(gunWeapon))
            {
                gunWeapons.Add(gunWeapon);

                leadTargetPositions.Add(Vector3.zero);
            }
        }


        /// <summary>
        /// Store a reference to a missile weapon.
        /// </summary>
        /// <param name="missileWeapon">The missile weapon.</param>
        protected virtual void AddWeapon(MissileWeapon missileWeapon)
        {
            if (!missileWeapons.Contains(missileWeapon))
            {
                missileWeapons.Add(missileWeapon);
                if (weaponsTargetSelector != null) weaponsTargetSelector.onSelectedTargetChanged.AddListener(missileWeapon.TargetLocker.SetTarget);
            }
        }


        /// <summary>
        /// Remove a reference to a weapon.
        /// </summary>
        /// <param name="weapon">The weapon.</param>
        protected virtual void RemoveWeapon(Weapon weapon)
        {
            GunWeapon gunWeapon = weapon as GunWeapon;
            if (gunWeapon != null)
            {
                RemoveWeapon(gunWeapon);
            }

            MissileWeapon missileWeapon = weapon as MissileWeapon;
            if (missileWeapon != null)
            {
                RemoveWeapon(missileWeapon);
            }
        }


        /// <summary>
        /// Remove a reference to a gun weapon.
        /// </summary>
        /// <param name="gunWeapon">The gun weapon.</param>
        protected virtual void RemoveWeapon(GunWeapon gunWeapon)
        {
            int index = gunWeapons.IndexOf(gunWeapon);
            if (index != -1)
            {
                gunWeapons.RemoveAt(index);

                leadTargetPositions.RemoveAt(index);
            }
        }


        /// <summary>
        /// Remove a reference to a missile weapon.
        /// </summary>
        /// <param name="missileWeapon">The missile weapon.</param>
        protected virtual void RemoveWeapon(MissileWeapon missileWeapon)
        {
            missileWeapons.Remove(missileWeapon);
            if (weaponsTargetSelector != null) weaponsTargetSelector.onSelectedTargetChanged.RemoveListener(missileWeapon.TargetLocker.SetTarget);
        }


        // Called when a module is unmounted from one of the vehicle's module mounts.
        protected override void OnModuleUnmounted(Module module)
        {
            // Unlink gun weapons
            GunWeapon gunWeapon = module.GetComponentInChildren<GunWeapon>();
            if (gunWeapon != null)
            {
                RemoveWeapon(gunWeapon);
            }

            // Unlink missile weapons
            MissileWeapon missileWeapon = module.GetComponentInChildren<MissileWeapon>();
            if (missileWeapon != null)
            {
                RemoveWeapon(missileWeapon);
            }
        }


        /// <summary>
        /// Get the average of the lead target positions for all gun weapons for a target with a position and velocity.
        /// </summary>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetVelocity">The target velocity.</param>
        /// <returns>The lead target position.</returns>
        public virtual Vector3 GetAverageLeadTargetPosition(Vector3 targetPosition, Vector3 targetVelocity)
        {

            if (gunWeapons.Count == 0) 
            {
                return targetPosition;
            }

            Vector3 leadTargetPosition = Vector3.zero;

            // Get the average lead target position
            for(int i = 0; i < gunWeapons.Count; ++i)
            {
                leadTargetPosition += gunWeapons[i].GetLeadTargetPosition(targetPosition, targetVelocity);
            }

            leadTargetPosition /= gunWeapons.Count;

            return leadTargetPosition;
        }


        /// <summary>
        /// Get the lead target position for the gun weapon with the maximum speed for a target with a position and velocity.
        /// </summary>
        /// <param name="targetPosition">The target's position.</param>
        /// <param name="targetVelocity">The target's velocity.</param>
        /// <returns>The lead target position.</returns>
        public virtual Vector3 GetMaxSpeedLeadTargetPosition(Vector3 targetPosition, Vector3 targetVelocity)
        {

            if (gunWeapons.Count == 0)
            {
                return targetPosition;
            }

            Vector3 leadTargetPosition = Vector3.zero;

            // Get the average lead target position
            float maxSpeed = 0;
            for (int i = 0; i < gunWeapons.Count; ++i)
            {
                if (gunWeapons[i].Speed > maxSpeed)
                {
                    maxSpeed = gunWeapons[i].Speed;
                    leadTargetPosition = gunWeapons[i].GetLeadTargetPosition(targetPosition, targetVelocity);
                }
            }

            return leadTargetPosition;
        }


        /// <summary>
        /// Get the lead target position for the gun weapon with the minimum speed for a target with a position and velocity.
        /// </summary>
        /// <param name="targetPosition">The target's position.</param>
        /// <param name="targetVelocity">The target's velocity.</param>
        /// <returns>The lead target position.</returns>
        public virtual Vector3 GetMinSpeedLeadTargetPosition(Vector3 targetPosition, Vector3 targetVelocity)
        {

            if (gunWeapons.Count == 0)
            {
                return targetPosition;
            }

            Vector3 leadTargetPosition = Vector3.zero;

            // Get the average lead target position
            float minSpeed = Mathf.Infinity;
            for (int i = 0; i < gunWeapons.Count; ++i)
            {
                if (gunWeapons[i].Speed < minSpeed)
                {
                    minSpeed = gunWeapons[i].Speed;
                    leadTargetPosition = gunWeapons[i].GetLeadTargetPosition(targetPosition, targetVelocity);
                }
            }

            return leadTargetPosition;
        }



        protected virtual void UpdateLeadTargetPositions(bool reset)
        {
            if (weaponsTargetSelector == null || weaponsTargetSelector.SelectedTarget == null) return;

            if (reset)
            {
                leadTargetAimedIndex = -1;
            }

            int newLeadTargetAimedIndex = -1;
            Vector3 targetPos = weaponsTargetSelector.SelectedTarget.transform.TransformPoint(weaponsTargetSelector.SelectedTarget.TrackingBounds.center);
            for (int i = 0; i < gunWeapons.Count; ++i)
            {
                
                leadTargetPositions[i] = gunWeapons[i].GetLeadTargetPosition(targetPos, weaponsTargetSelector.SelectedTarget.Velocity);

                if (aimController != null && Vector3.Angle(aimController.Aim.direction, leadTargetPositions[i] - aimController.Aim.origin) < aimAssistAngle)
                {
                    if (newLeadTargetAimedIndex == -1 || newLeadTargetAimedIndex != leadTargetAimedIndex)   // Defer to the currently aimed index if there are multiple
                    {
                        newLeadTargetAimedIndex = i;
                    }
                }
            }

            if (newLeadTargetAimedIndex != leadTargetAimedIndex)
            {
                leadTargetAimedIndex = newLeadTargetAimedIndex;
                aimStateStartTime = Time.time;
            }
        }


        /// <summary>
        /// Aim the weapons at the Aim Controller's hit position.
        /// </summary>
        protected virtual void AimUpdate()
        {
            foreach (GunWeapon gunWeapon in gunWeapons)
            {
                gunWeapon.ClearAim();
            }

            foreach (MissileWeapon missileWeapon in missileWeapons)
            {
                missileWeapon.ClearAim();
            }

            // Calculate the weapon-range-limited aim position, taking into account that the camera (aim origin) may be far back from the weapons.

            float hyp = MaxGunWeaponRange;

            Vector3 averageWeaponPosition = Vector3.zero;
            for(int i = 0; i< gunWeapons.Count; ++i)
            {
                averageWeaponPosition += gunWeapons[i].AimPosition();
            }
            for (int i = 0; i < missileWeapons.Count; ++i)
            {
                averageWeaponPosition += missileWeapons[i].AimPosition();
            }
            averageWeaponPosition /= gunWeapons.Count + missileWeapons.Count;

            // Get closest point on the ray from the average weapon position, this forms the opposite of the triangle
            Vector3 temp = aimController.Aim.origin + Vector3.Dot(averageWeaponPosition - aimController.Aim.origin, aimController.Aim.direction) * aimController.Aim.direction;
            float opp = (temp - averageWeaponPosition).magnitude;
            float adj = Mathf.Sqrt(hyp * hyp - opp * opp);
            Vector3 aimPosition = temp + aimController.Aim.direction * adj;

            if (aimController.HitFound)
            {
                aimPosition = aimController.Hit.point;

                if (Vector3.Distance(aimController.Aim.origin, aimPosition) > MaxGunWeaponRange)
                {
                    aimPosition = averageWeaponPosition + (aimPosition - averageWeaponPosition).normalized * MaxGunWeaponRange;
                }
            }

            // Rotate turrets

            for (int i = 0; i < gunWeapons.Count; ++i)
            {
                if (gunWeapons[i].Gimbal != null && gunWeapons[i].WeaponController == null)
                {
                    // If this is both a vehicle and a turret, don't rotate itself.
                    if (gunWeapons[i].Gimbal.transform == transform) continue;

                    float angleToTarget;

                    gunWeapons[i].Gimbal.TrackPosition(aimPosition, out angleToTarget, snapTurretsToTarget);
                }
            }

            for (int i = 0; i < missileWeapons.Count; ++i)
            {
                if (missileWeapons[i].Gimbal != null && missileWeapons[i].WeaponController == null)
                {
                    float angleToTarget;

                    missileWeapons[i].Gimbal.TrackPosition(aimPosition, out angleToTarget, snapTurretsToTarget);
                }
            }

            if (aimWeaponsAtAimPosition)
            {
                if (aimController.HitFound)
                {
                    Vector3 averageAimerPosition = Vector3.zero;
                    Vector3 averageZeroedAimerDirection = Vector3.zero;
                    if (gunWeapons.Count > 0 || missileWeapons.Count > 0)
                    {
                        for (int i = 0; i < gunWeapons.Count; ++i)
                        {
                            averageAimerPosition += gunWeapons[i].AimPosition();
                            averageZeroedAimerDirection += gunWeapons[i].ZeroAimDirection();
                        }

                        for (int i = 0; i < missileWeapons.Count; ++i)
                        {
                            averageAimerPosition += missileWeapons[i].AimPosition();
                            averageZeroedAimerDirection += missileWeapons[i].ZeroAimDirection();
                        }

                        averageAimerPosition /= gunWeapons.Count + missileWeapons.Count;
                        averageZeroedAimerDirection /= gunWeapons.Count + missileWeapons.Count;
                    }

                    float angle = Vector3.Angle(aimController.Hit.point - averageAimerPosition, averageZeroedAimerDirection);

                    if (angle > maxAverageWeaponAimAngle)
                    {
                        aimPosition = aimController.Aim.origin + aimController.Aim.direction * MaxGunWeaponRange;
                    }
                }

                foreach (GunWeapon gunWeapon in gunWeapons)
                {
                    gunWeapon.Aim(aimPosition);
                }

                foreach (MissileWeapon missileWeapon in missileWeapons)
                {
                    missileWeapon.Aim(aimPosition);
                }
            }
        }


        /// <summary>
        /// Implement aim assist.
        /// </summary>
        /// <returns>Whether aim assist was applied.</returns>
        protected virtual bool AimAssistUpdate()
        {

            if (weaponsTargetSelector == null || weaponsTargetSelector.SelectedTarget == null) return false;

            Vector3 aimTarget = weaponsTargetSelector.SelectedTarget.WorldBoundsCenter;

            switch (aimAssistLeadTargetMode)
            {
                case AimAssistLeadTargetMode.MaxSpeed:

                    aimTarget = GetMaxSpeedLeadTargetPosition(aimTarget, weaponsTargetSelector.SelectedTarget.Velocity);

                    break;

                case AimAssistLeadTargetMode.MinSpeed:

                    aimTarget = GetMinSpeedLeadTargetPosition(aimTarget, weaponsTargetSelector.SelectedTarget.Velocity);

                    break;

                default: // average

                    aimTarget = GetAverageLeadTargetPosition(aimTarget, weaponsTargetSelector.SelectedTarget.Velocity);

                    break;
            }

            if (Vector3.Distance(aimController.Aim.origin, aimTarget) > MaxGunWeaponRange)
            {
                return false;
            }

            if (Vector3.Angle(aimController.Aim.direction, (aimTarget - aimController.Aim.origin)) > aimAssistAngle)
            {
                return false;
            }

            // Aim assist found
            if (aimAssist)
            {
                foreach (GunWeapon gunWeapon in gunWeapons)
                {
                    gunWeapon.Aim(aimTarget);
                }

                foreach (MissileWeapon missileWeapon in missileWeapons)
                {
                    missileWeapon.Aim(aimTarget);
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        // Called every frame
        protected virtual void LateUpdate()
        {
            UpdateLeadTargetPositions(false);

            if (aimController != null)
            {
                AimUpdate();

                aimAssistState = AimAssistUpdate();
            }
            else
            {
                aimAssistState = false;
            }
            if (onAimAssistUpdated != null) onAimAssistUpdated.Invoke();
        }
    }
}