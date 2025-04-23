using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Represents a 3D volume that returns a value of 0 - 1 for a specified world position. Used to modulate values in different areas of a game world.
    /// </summary>
    public class Volume : MonoBehaviour
    {

        [Tooltip("The distance from the volume's boundary over which blending is applied.")]
        [SerializeField]
        protected float blendDistance = 0;


        protected virtual void OnEnable()
        {
            if (VolumeManager.Instance != null)
            {
                VolumeManager.Instance.Register(this);
            }
        }


        protected virtual void OnDisable()
        {
            if (VolumeManager.Instance != null)
            {
                VolumeManager.Instance.Deregister(this);
            }
        }


        /// <summary>
        /// Get the Volume's value (0 - 1) for a specified world position.
        /// </summary>
        /// <param name="position">The world position.</param>
        /// <returns>The Volume value (0 - 1).</returns>
        public virtual float GetValue(Vector3 position)
        {
            return 0f;
        }
    }
}

