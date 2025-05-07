using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;


namespace VSX.Weapons
{
    public class MissileWarningEmitter : MonoBehaviour
    {
        public Missile missile;
        public TargetLocker targetLocker;
        protected MissileWarningReceiver receiver;


        private void Awake()
        {
            targetLocker.onTargetChanged.AddListener(OnTargetChanged);
            missile.onDetonated.AddListener(OnDetonated);
            missile.onTargetLockLost.AddListener(OnTargetLockLost);
            OnTargetChanged();
        }


        void OnTargetChanged()
        {
            if (receiver != null) receiver.StopWarning();

            if (targetLocker.Target != null)
            {
                receiver = targetLocker.Target.GetComponentInChildren<MissileWarningReceiver>();
            }
            else
            {
                receiver = null;
            }
        }


        void OnTargetLockLost()
        {
            if (receiver != null) receiver.OnLockLost();
        }


        void OnDetonated()
        {
            if (receiver != null)
            {
                receiver.StopWarning();
            }
        }


        protected void Update()
        {
            if (receiver != null && targetLocker.LockState == LockState.Locked && !missile.Detonated)
            {
                receiver.OnMissileWarning(missile);
            }
        }
    }
}

