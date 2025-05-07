using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    [DefaultExecutionOrder(-30)]    // Create the Instance before any of the volumes' Awake/OnEnable functions are called.
    public class VolumeManager : MonoBehaviour
    {
        // All the volumes registered in the scene
        protected List<Volume> volumes = new List<Volume>();

        public static VolumeManager Instance;


        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        /// <summary>
        /// Register a volume.
        /// </summary>
        /// <param name="volume">The volume.</param>
        public virtual void Register(Volume volume)
        {
            if (volumes.IndexOf(volume) == -1)
            {
                volumes.Add(volume);
            }
        }


        /// <summary>
        /// Deregister a volume.
        /// </summary>
        /// <param name="volume">The volume.</param>
        public virtual void Deregister(Volume volume)
        {
            volumes.Remove(volume);
        }


        /// <summary>
        /// Get the maximum value for all volumes on the layer mask.
        /// </summary>
        /// <param name="position">The world position.</param>
        /// <param name="volumeLayerMask">The layer mask to look for volumes on.</param>
        /// <returns>The maximum value.</returns>
        public virtual float GetValue(Vector3 position, LayerMask volumeLayerMask)
        {
            float maxVal = 0;

            for (int i = 0; i < volumes.Count; ++i)
            {
                if ((volumeLayerMask & (1 << volumes[i].gameObject.layer)) != 0)
                {
                    maxVal = Mathf.Max(maxVal, volumes[i].GetValue(position));
                }
            }

            return maxVal;
        }
    }
}

