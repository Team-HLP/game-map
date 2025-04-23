using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Designates a Volume that is in the shape of a collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ColliderVolume : Volume
    {

        protected Collider m_Collider;


        protected virtual void Awake()
        {
            m_Collider = GetComponent<Collider>();
        }


        protected virtual void Reset()
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<MeshCollider>();
            }
        }


        /// <summary>
        /// Get the Volume's value (0 - 1) for a specified world position.
        /// </summary>
        /// <param name="position">The world position.</param>
        /// <returns>The Volume value (0 - 1).</returns>
        public override float GetValue(Vector3 position)
        {
            float dist = Vector3.Distance(m_Collider.ClosestPoint(position), position);

            if (Mathf.Approximately(dist, 0f))
            {
                return 1;
            }
            else
            {
                if (Mathf.Approximately(blendDistance, 0))
                {
                    return 0;
                }
                else
                {
                    return Mathf.Max(1 - dist / blendDistance, 0);
                }
            }
        }
    }
}

