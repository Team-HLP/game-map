using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.Utilities
{
    /// <summary>
    /// Controls physics settings at runtime.
    /// </summary>
    public class ScenePhysicsManager : MonoBehaviour
    {
        [Tooltip("The gravity to use for this scene.")]
        [SerializeField]
        protected Vector3 gravity;


        protected virtual void Awake()
        {
            Physics.gravity = gravity;
        }
    }
}
