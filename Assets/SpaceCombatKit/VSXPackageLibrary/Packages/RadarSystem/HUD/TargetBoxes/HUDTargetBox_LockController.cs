using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Utilities;

namespace VSX.RadarSystem
{
    [System.Serializable]
    public class LockBoxAnimation
    {
        public RectTransform rectTransform;
        public float lockingMargin;
        public float lockedMargin;
    }

    /// <summary>
    /// Manages a single lock on a target box on the HUD.
    /// </summary>
    public class HUDTargetBox_LockController : MonoBehaviour
    {

        public List<AnimationController> lockingAnimations = new List<AnimationController>();
        public List<AnimationController> lockedAnimations = new List<AnimationController>();

        public UnityEvent onActive;
        public UnityEvent onInactive;


        // Activate the lock box
        public virtual void Activate()
        {
            onActive.Invoke();
        }

        // Deactivate the lock box
        public virtual void Deactivate()
        {
            for (int i = 0; i < lockingAnimations.Count; ++i)
            {
                lockingAnimations[i].StopAnimation();
            }

            for (int i = 0; i < lockedAnimations.Count; ++i)
            {
                lockedAnimations[i].StopAnimation();
            }

            onInactive.Invoke();
        }


        public virtual void UpdateLock(TargetLocker targetLocker)
        {
            switch (targetLocker.LockState)
            {
                case LockState.NoLock:

                    for (int i = 0; i < lockingAnimations.Count; ++i)
                    {
                        lockingAnimations[i].StopAnimation();
                    }

                    for (int i = 0; i < lockedAnimations.Count; ++i)
                    {
                        lockedAnimations[i].StopAnimation();
                    }

                    break;

                case LockState.Locking:

                    for (int i = 0; i < lockedAnimations.Count; ++i)
                    {
                        lockedAnimations[i].StopAnimation();
                    }

                    for (int i = 0; i < lockingAnimations.Count; ++i)
                    {
                        lockingAnimations[i].StartAnimation();
                    }

                    for (int i = 0; i < lockingAnimations.Count; ++i)
                    {
                        lockingAnimations[i].SetAnimationTime(Time.time - targetLocker.LockStateChangeTime);
                    }

                    break;

                case LockState.Locked:

                    for (int i = 0; i < lockingAnimations.Count; ++i)
                    {
                        lockingAnimations[i].StopAnimation();
                    }

                    for (int i = 0; i < lockedAnimations.Count; ++i)
                    {
                        lockedAnimations[i].StartAnimation();
                    }

                    for (int i = 0; i < lockedAnimations.Count; ++i)
                    {
                        lockedAnimations[i].SetAnimationTime(Time.time - targetLocker.LockStateChangeTime);
                    }

                    break;
            }
        }
    }
}