using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Weapons
{
    /// <summary>
    /// Controls a gun turret with fire control parameters.
    /// </summary>
    public class GunTurretController : TurretController
    {
        [Header("Gun Turret")]

        [Tooltip("The minimum length of the firing burst.")]
        [SerializeField]
        protected float minFiringPeriod = 1;
        public float MinFiringPeriod
        {
            get { return minFiringPeriod; }
            set { minFiringPeriod = value; }
        }

        [Tooltip("The maximum length of the firing burst.")]
        [SerializeField]
        protected float maxFiringPeriod = 1;
        public float MaxFiringPeriod
        {
            get { return maxFiringPeriod; }
            set { maxFiringPeriod = value; }
        }


        [Tooltip("The minimum time between firing bursts.")]
        [SerializeField]
        protected float minFiringInterval = 1;
        public float MinFiringInterval
        {
            get { return minFiringInterval; }
            set { minFiringInterval = value; }
        }

        [Tooltip("The maximum time between firing bursts.")]
        [SerializeField]
        protected float maxFiringInterval = 1;
        public float MaxFiringInterval
        {
            get { return maxFiringInterval; }
            set { maxFiringInterval = value; }
        }


        [Tooltip("The minimum time that the turret spends engaged (firing at intervals).")]
        [SerializeField]
        protected float minEngagementPeriod = 5;
        public float MinEngagementPeriod
        {
            get { return minEngagementPeriod; }
            set { minEngagementPeriod = value; }
        }

        [Tooltip("The maximum time that the turret spends engaged (firing at intervals).")]
        [SerializeField]
        protected float maxEngagementPeriod = 5;
        public float MaxEngagementPeriod
        {
            get { return maxEngagementPeriod; }
            set { maxEngagementPeriod = value; }
        }


        [Tooltip("The minimum time that the turret spends in standby mode between engagements.")]
        [SerializeField]
        protected float minStandbyPeriod = 5;
        public float MinStandbyPeriod
        {
            get { return minStandbyPeriod; }
            set { minStandbyPeriod = value; }
        }


        [Tooltip("The maximum time that the turret spends in standby mode between engagements.")]
        [SerializeField]
        protected float maxStandbyPeriod = 5;
        public float MaxStandbyPeriod
        {
            get { return maxStandbyPeriod; }
            set { maxStandbyPeriod = value; }
        }


        [Tooltip("The maximum angle to target where the turret will fire.")]
        [SerializeField]
        protected float minFiringAngle = 5;

        public enum TurretState
        {
            None,
            Acquiring,
            Engaged,
            Standby
        }


        protected TurretState turretState;
        public TurretState State { get { return turretState; } }


        protected float nextTurretStateTime;
        public float NextTurretStateTime { get { return nextTurretStateTime; } }


        protected float turretStateStartTime;
        public float TurretStateStartTime { get { return turretStateStartTime; } }


        // Whether the turret is currently engaged in firing
        protected bool isFiring;    
        public bool IsFiring { get { return isFiring; } }

        // The time that the turret last changed state (became engaged in firing or stopped)
        protected float firingStateStartTime;
        public float FiringStateStartTime { get { return firingStateStartTime; } }

        // The period for the current firing state of the turret
        protected float nextFiringStatePeriod;
        public float NextFiringStatePeriod { get { return nextFiringStatePeriod; } }

        [Tooltip("Whether the turret should track the target while engaged in firing.")]
        [SerializeField]
        protected bool trackTargetWhenEngaged = true;

        [Tooltip("Whether the turret should track the target while not engaged in firing (standby).")]
        [SerializeField]
        protected bool trackTargetWhenOnStandby = true;

        public UnityAction onTurretStateChanged;



        protected virtual void Update()
        {
            Vector3 targetPosition = GetAimPosition();


            switch (turretState)
            {
                case TurretState.None:

                    if (target != null)
                    {
                        SetTurretState(TurretState.Acquiring);
                    }
                    else
                    {
                        // Return the turret to center
                        if (noTargetReturnToCenter) weapon.Gimbal.ResetGimbal(false);
                    }

                    break;

                case TurretState.Acquiring:

                    if (target == null)
                    {
                        SetTurretState(TurretState.None);
                    }
                    else
                    {
                        TrackPosition(targetPosition);

                        if (AngleToTarget(targetPosition) < minFiringAngle && weapon.FiringConditionsMet())
                        {
                            SetTurretState(TurretState.Engaged);
                        }
                    }

                    break;

                case TurretState.Engaged:

                    if (target == null)
                    {
                        SetTurretState(TurretState.None);
                    }
                    else if (Time.time - turretStateStartTime > nextTurretStateTime)
                    {
                        SetTurretState(TurretState.Standby);
                    }
                    else if (AngleToTarget(targetPosition) > minFiringAngle)
                    {
                        SetTurretState(TurretState.Acquiring);
                    }
                    else
                    {
                        if (trackTargetWhenEngaged) TrackPosition(targetPosition);

                        if (Time.time - firingStateStartTime > nextFiringStatePeriod)
                        {
                            if (isFiring)
                            {
                                StopFiring();
                            }
                            else
                            {
                                StartFiring();
                            }
                        }
                    }

                    break;

                case TurretState.Standby:

                    if (target == null)
                    {
                        SetTurretState(TurretState.None);
                    }
                    else 
                    { 
                        if (Time.time - turretStateStartTime > nextTurretStateTime)
                        {
                            SetTurretState(TurretState.Acquiring);
                        }
                        else
                        {
                            if (trackTargetWhenOnStandby) TrackPosition(targetPosition);
                        }
                    }

                    break;
            }
        }
       

        protected virtual void SetTurretState(TurretState turretState)
        {

            if (this.turretState == turretState) return;

            turretStateStartTime = Time.time;

            switch (turretState)
            {
                case TurretState.None:

                    StopFiring();
                    break;

                case TurretState.Acquiring:

                    StopFiring();
                    break;

                case TurretState.Engaged:

                    nextTurretStateTime = Random.Range(minEngagementPeriod, maxEngagementPeriod);
                    StartFiring();
                    break;

                case TurretState.Standby:

                    nextTurretStateTime = Random.Range(minStandbyPeriod, maxStandbyPeriod);
                    StopFiring();
                    break;

            }

            this.turretState = turretState;

            if (onTurretStateChanged != null) onTurretStateChanged.Invoke();
        }


        // Start firing the turret
        protected virtual void StartFiring()
        {
            isFiring = true;
            firingStateStartTime = Time.time;
            nextFiringStatePeriod = Random.Range(minFiringPeriod, maxFiringPeriod);
            weapon.Triggerable.StartTriggering();
        }


        // Stop firing the turret
        protected virtual void StopFiring()
        {
            isFiring = false;
            firingStateStartTime = Time.time;
            nextFiringStatePeriod = Random.Range(minFiringInterval, maxFiringInterval);
            weapon.Triggerable.StopTriggering();
        }
    }
}

