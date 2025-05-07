using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;


namespace VSX.RadarSystem
{
    /// <summary>
    /// Manages the locks for a target box displayed on the HUD.
    /// </summary>
    public class HUDTargetBox_LocksController : MonoBehaviour
    {

        [SerializeField]
        protected UVCText numLocksText;
        protected int numLocks;

        [SerializeField]
        protected bool showTextForMultipleLocksOnly = true;

        [SerializeField]
        protected List<HUDTargetBox_LockController> lockBoxes = new List<HUDTargetBox_LockController>();

        protected int lastUsedIndex = -1;

        protected Coroutine resetCoroutine;


        /// <summary>
        /// Add a lock to the target box.
        /// </summary>
        /// <param name="targetLocker">The target locker that is locked onto the target.</param>
        public virtual void AddLock(TargetLocker targetLocker)
        {

            lastUsedIndex += 1;

            if (lastUsedIndex < lockBoxes.Count)
            {
                UpdateLockBox(targetLocker, lockBoxes[lastUsedIndex]);
            }
            else
            {
                return;
            }
        }

        protected virtual void UpdateLockBox(TargetLocker targetLocker, HUDTargetBox_LockController lockBox)
        {

            lockBox.gameObject.SetActive(true);

            // Update the lock state
            switch (targetLocker.LockState)
            {
                case LockState.NoLock:

                    lockBox.Deactivate();

                    break;

                case LockState.Locking:

                    lockBox.Activate();

                    lockBox.UpdateLock(targetLocker);

                    break;

                case LockState.Locked:

                    lockBox.Activate();

                    lockBox.UpdateLock(targetLocker);

                    numLocks += 1;

                    break;
            }

            if (numLocksText != null)
            {
                numLocksText.text = numLocks.ToString();
                numLocksText.gameObject.SetActive(!showTextForMultipleLocksOnly || numLocks > 1);
            }
        }

        protected virtual void OnEnable()
        {
            resetCoroutine = StartCoroutine(ResetLockBoxesCoroutine());
        }

        protected virtual void OnDisable()
        {
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        }

        // Coroutine for resetting the lead target boxes at the end of the frame
        protected virtual IEnumerator ResetLockBoxesCoroutine()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                for (int i = 0; i < lockBoxes.Count; ++i)
                {
                    lockBoxes[i].Deactivate();
                }
                lastUsedIndex = -1;
                numLocks = 0;
                if (numLocksText != null) numLocksText.text = numLocks.ToString();
            }
        }
    }
}