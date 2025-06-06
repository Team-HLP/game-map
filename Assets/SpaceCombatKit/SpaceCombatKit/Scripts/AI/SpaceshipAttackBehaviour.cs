﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;
using UnityEngine.Events;
using VSX.Vehicles;
using VSX.Weapons;
using VSX.Engines3D;


namespace VSX.SpaceCombatKit
{
    public class SpaceshipAttackBehaviour : AISpaceshipBehaviour
    {

        [Header("Primary Weapons")]

        [Tooltip("Whether to use primary weapons.")]
        [SerializeField]
        protected bool primaryWeaponsEnabled = true;

        [Tooltip("This is the minimum random amount of time the primary weapons will be firing continuously before a pause, when the target is in the sights.")]
        [SerializeField]
        protected float minPrimaryFiringPeriod = 1;

        [Tooltip("This is the maximum random amount of time the primary weapons will be firing continuously before a pause, when the target is in the sights.")]
        [SerializeField]
        protected float maxPrimaryFiringPeriod = 3;

        [Tooltip("This is the minimum random amount of time the primary weapons will pause before firing again, when the target is in the sights.")]
        [SerializeField]
        protected float minPrimaryFiringPause = 0.5f;

        [Tooltip("This is the maximum random amount of time the primary weapons will pause before firing again, when the target is in the sights.")]
        [SerializeField]
        protected float maxPrimaryFiringPause = 2;

        [Tooltip("This is the maximum angle to target (relative to the ship's forward vector) within which the AI will fire the primary weapons.")]
        [SerializeField]
        protected float maxFiringAngle = 15f;

        [Tooltip("This is the maximum distance to target within which the AI will fire the primary weapons.")]
        [SerializeField]
        protected float maxFiringDistance = 600;

        protected float primaryWeaponActionStartTime = 0;
        protected float primaryWeaponActionPeriod = 0f;
        protected bool primaryWeaponEngaged = false;
        protected bool primaryWeaponFiring = false;

        [Header("Secondary Weapons")]

        [Tooltip("Whether to use secondary weapons.")]
        [SerializeField]
        protected bool secondaryWeaponsEnabled = true;
        public virtual bool SecondaryWeaponsEnabled
        {
            get { return secondaryWeaponsEnabled; }
            set { secondaryWeaponsEnabled = value; }
        }

        [Tooltip("This is the minimum (x-value) and maximum (y-value) random interval between firing of the secondary weapons (missiles).")]
        [SerializeField]
        protected Vector2 minMaxSecondaryFiringInterval = new Vector2(10, 25);

        [Tooltip("If false, will fire a missile immediately upon engaging, otherwise, upon engaging a target, will wait sometime between 0 and 'Min Max Secondary Firing Interval' y-value to fire the first shot.")]
        [SerializeField]
        protected bool randomizeFirstSecondaryFiringTime = true;

        protected float secondaryWeaponActionStartTime = 0;
        protected float secondaryWeaponActionPeriod = 0f;
        protected bool secondaryWeaponFiring = false;

        protected WeaponsController weapons;
        protected TriggerablesManager triggerablesManager;

        public UnityAction onSecondaryWeaponFired;
        

        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            weapons = vehicle.GetComponent<WeaponsController>();
            if (weapons == null) return false;
            
            triggerablesManager = vehicle.GetComponent<TriggerablesManager>();
            if (triggerablesManager == null) return false;

            engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines == null) return false;

            return true;

        }


        public override void StartBehaviour()
        {
            base.StartBehaviour();
            if (randomizeFirstSecondaryFiringTime)
            {
                secondaryWeaponActionPeriod = Random.Range(0, minMaxSecondaryFiringInterval.y);
                secondaryWeaponActionStartTime = Time.time;
            }
        }


        public override void StopBehaviour()
        {
            if (initialized) triggerablesManager.StopTriggeringAll();
        }
        

        protected virtual void StopPrimaryWeapon()
        {
            triggerablesManager.StartTriggeringAtIndex(0);
            primaryWeaponFiring = false;
        }


        protected virtual void SetPrimaryWeaponAction(bool fire)
        {
            if (fire)
            {
                triggerablesManager.StartTriggeringAtIndex(0);
                primaryWeaponFiring = true;

                primaryWeaponActionStartTime = Time.time;
                primaryWeaponActionPeriod = Random.Range(minPrimaryFiringPeriod, maxPrimaryFiringPeriod);
            }
            else
            {
                triggerablesManager.StopTriggeringAtIndex(0);
                primaryWeaponFiring = false;

                primaryWeaponActionStartTime = Time.time;
                primaryWeaponActionPeriod = Random.Range(minPrimaryFiringPause, maxPrimaryFiringPause);
            }
        }


        protected virtual void PrimaryWeaponUpdate(Vector3 toTargetVector)
        {
            // Do primary weapons
            bool canFire = primaryWeaponsEnabled && Vector3.Angle(vehicle.transform.forward, toTargetVector) < maxFiringAngle && toTargetVector.magnitude < maxFiringDistance;

            if (canFire)
            {
                if (!primaryWeaponEngaged)
                {
                    primaryWeaponEngaged = true;
                    SetPrimaryWeaponAction(true);
                }

                // Fire in bursts
                if (Time.time - primaryWeaponActionStartTime > primaryWeaponActionPeriod)
                {
                    SetPrimaryWeaponAction(!primaryWeaponFiring);
                }
            }
            else
            {
                if (primaryWeaponEngaged)
                {
                    primaryWeaponEngaged = false;
                    SetPrimaryWeaponAction(false);
                }
            }
        }


        protected virtual void SecondaryWeaponUpdate()
        {
            if (secondaryWeaponsEnabled && weapons.MissileWeapons.Count > 0)
            {
                TargetLocker targetLocker = weapons.MissileWeapons[0].GetComponent<TargetLocker>();
                if (targetLocker != null && targetLocker.LockState == LockState.Locked)
                {
                    if (Time.time - secondaryWeaponActionStartTime > secondaryWeaponActionPeriod)
                    {
                        triggerablesManager.TriggerOnce(1);
                        secondaryWeaponActionPeriod = Random.Range(minMaxSecondaryFiringInterval.x, minMaxSecondaryFiringInterval.y);
                        secondaryWeaponActionStartTime = Time.time;
                        if (onSecondaryWeaponFired != null) onSecondaryWeaponFired.Invoke();
                    }
                }
            }
        }


        /// <summary>
        /// Update the behaviour.
        /// </summary>
        public override bool BehaviourUpdate()
        {

            if (!base.BehaviourUpdate()) return false;

            if (weapons.WeaponsTargetSelector == null || weapons.WeaponsTargetSelector.SelectedTarget == null)
            {
                return false;
            }

            Vector3 velocity = weapons.WeaponsTargetSelector.SelectedTarget.Rigidbody == null ? Vector3.zero : weapons.WeaponsTargetSelector.SelectedTarget.Rigidbody.velocity;

            Vector3 targetPos = weapons.GetAverageLeadTargetPosition(weapons.WeaponsTargetSelector.SelectedTarget.transform.position, velocity);

            // 현재 기체보다 높이를 최소 2~최대 6 m 사이로 제한
            float yOffset = Mathf.Clamp(targetPos.y - vehicle.transform.position.y, 2f, 6f);
            targetPos.y   = vehicle.transform.position.y + yOffset;

            Vector3 toTargetVector = targetPos - vehicle.transform.position;

            // Do primary weapons
            PrimaryWeaponUpdate(toTargetVector);

            // Do the secondary weapons
            SecondaryWeaponUpdate();
            
            // Turn toward target
            Maneuvring.TurnToward(vehicle.transform, targetPos, maxRotationAngles, shipPIDController.steeringPIDController);
            engines.SetSteeringInputs(shipPIDController.steeringPIDController.GetControlValues());

            Maneuvring.TranslateToward(engines.Rigidbody, targetPos, shipPIDController.movementPIDController);

            Vector3 movementInputs = shipPIDController.movementPIDController.GetControlValues();
            engines.SetMovementInputs(new Vector3(0, 0, movementInputs.z));

            return true;

        }
    }
}