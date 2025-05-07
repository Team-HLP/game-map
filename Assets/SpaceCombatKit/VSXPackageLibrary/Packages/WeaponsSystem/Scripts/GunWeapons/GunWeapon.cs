using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;

namespace VSX.Weapons
{
    public class GunWeapon : Weapon
    {

        public virtual Vector3 GetLeadTargetPosition(Vector3 targetPosition, Vector3 targetVelocity)
        {
            // Return the target position when out of range
            if (Vector3.Distance (targetPosition, transform.position) > Range)
            {
                return targetPosition;
            }
            // Return the lead target position when inside of range
            else
            {
                return Speed == Mathf.Infinity ? targetPosition : TargetLeader.GetLeadPosition(transform.position, Speed, targetPosition, targetVelocity);
            }
        }
    }
}
