using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Utilities
{
    /// <summary>
    /// Enables you to set up in the inspector to call a function every update using events.
    /// </summary>
    public class UpdateEvents : MonoBehaviour
    {
        public UnityEvent onUpdate;
        public UnityEvent onLateUpdate;
        public UnityEvent onFixedUpdate;


        protected virtual void Update()
        {
            onUpdate.Invoke();
        }


        protected virtual void FixedUpdate()
        {
            onFixedUpdate.Invoke();
        }


        protected virtual void LateUpdate()
        {
            onLateUpdate.Invoke();
        }
    }
}

