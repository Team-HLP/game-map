using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;
using VSX.Teams;


namespace VSX.Weapons
{
    public class TurretController : WeaponController
    {
        [Tooltip("The team the turret belongs to. Setting this will update all relevant components on the turret with the team information.")]
        [SerializeField]
        protected Team team;
        public virtual Team Team { get => team; }

        protected bool teamInitialized = false;

        protected Trackable target;

        protected List<Trackable> selfTrackables = new List<Trackable>();

        [Header("Turret Properties")]

        [Tooltip("Whether the turret is locked to the target or smoothly moves toward it.")]
        [SerializeField]
        protected bool lockToTarget = false;

        [Tooltip("Whether the turret snaps toward the target when within a certain angle.")]
        [SerializeField]
        protected bool aimAssist = true;

        [Tooltip("The angle within which the turret snaps toward the target.")]
        [SerializeField]
        protected float aimAssistAngleThreshold = 3;


        [Tooltip("The target selector for the turret's own target acquisition.")]
        [SerializeField]
        protected TargetSelector targetSelector;
        public TargetSelector TargetSelector
        {
            get { return targetSelector; }
            set { targetSelector = value; }
        }

        [Tooltip("Whether to lead the target when aiming or not.")]
        [SerializeField]
        protected bool leadTarget = true;

        [Tooltip("Whether the turret rotates back to the center when no target is present.")]
        [SerializeField]
        protected bool noTargetReturnToCenter = true;

        protected float firingAngle;


        // Called when this component is first added to a gameobject, or when it is reset in the inspector
        protected virtual void Reset()
        {
            // Get/add a target selector
            targetSelector = GetComponentInChildren<TargetSelector>();
        }


        protected virtual void Awake()
        {
            if (targetSelector != null)
            {
                targetSelector.onSelectedTargetChanged.AddListener(SetTarget);
            }

            selfTrackables = new List<Trackable>(GetComponentsInChildren<Trackable>());
        }


        protected virtual void Start()
        {
            if (!teamInitialized) SetTeam(team);
        }


        public virtual void SetTeam(Team team)
        {
            this.team = team;

            for(int i = 0; i < selfTrackables.Count; i++)
            {
                selfTrackables[i].Team = team;
            }

            if (team != null)
            {
                if (targetSelector != null)
                {
                    targetSelector.SelectableTeams = new List<Team>(team.HostileTeams);
                }
            }

            teamInitialized = true;
        }


        protected virtual float AngleToTarget(Vector3 targetPosition)
        {
            return Vector3.Angle(targetPosition - weapon.Gimbal.VerticalPivot.position, weapon.Gimbal.VerticalPivot.forward);
        }


        protected virtual Vector3 GetAimPosition()
        {
            if (target == null) return weapon.Gimbal.VerticalPivot.position + weapon.Gimbal.VerticalPivot.forward;

            if (leadTarget && target.Rigidbody != null)
            {
                return TargetLeader.GetLeadPosition(weapon.Gimbal.VerticalPivot.position, weapon.Speed,
                                                                target.transform.TransformPoint(target.TrackingBounds.center),
                                                                target.Rigidbody.velocity);
            }
            else
            {
                return target.transform.TransformPoint(target.TrackingBounds.center);
            }
        }


        protected virtual void TrackPosition(Vector3 targetPosition)
        {
            weapon.Gimbal.TrackPosition(targetPosition, out firingAngle, lockToTarget);

            if (aimAssist)
            {
                if (Vector3.Angle(weapon.Gimbal.VerticalPivot.forward, targetPosition - weapon.Gimbal.VerticalPivot.position) < aimAssistAngleThreshold)
                {
                    weapon.Aim(targetPosition);
                }
                else
                {
                    weapon.ClearAim();
                }
            }
        }


        public override void SetTarget(Trackable target)
        {
            base.SetTarget(target);

            this.target = target;
        }
    }

}
