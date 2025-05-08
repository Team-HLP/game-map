using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;
using VSX.RadarSystem;

namespace VSX.Weapons
{
    public class MissileTrackableToggle : MonoBehaviour
    {
        public TargetLocker targetLocker;
        public Trackable trackable;

        private void Awake()
        {
            targetLocker.onTargetChanged.AddListener(UpdateState);
            targetLocker.onLocked.AddListener(UpdateState);
            targetLocker.onNoLock.AddListener(UpdateState);
        }


        void UpdateState()
        {
            if (targetLocker.Target == null)
            {
                trackable.SetActivation(false);
            }
            else
            {
                Vehicle playerVehicle = GameAgentManager.Instance.FocusedGameAgent.Vehicle;
                if (playerVehicle != null)
                {
                    if (targetLocker.Target.transform.IsChildOf(playerVehicle.transform) && targetLocker.LockState == LockState.Locked)
                    {
                        trackable.SetActivation(true);
                        return;
                    }
                }

                trackable.SetActivation(false);
            }
        }
    }

}
