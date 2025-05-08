using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Utilities
{
    public class ObjectTrigger : MonoBehaviour
    {

        [Tooltip("The object that triggers this component. If using a trigger collider trigger, this must be either the object that the rigidbody is on, or must be the transform of the collider.")]
        [SerializeField]
        protected Transform triggerObject;

        protected bool triggered = false;

        protected bool triggerObjectWithinDistance = false;

        [SerializeField]
        protected bool startTriggered = false;

        [Header("Distance Trigger")]

        [SerializeField]
        protected bool distanceTrigger = true;

        [SerializeField]
        protected float triggerDistance = 500;

        [SerializeField]
        protected bool resetOnDistanceExceeded = true;

        [Header("Trigger Collider")]

        [SerializeField]
        protected bool resetOnTriggerExit = true;

        protected bool triggerColliderTripped = false;

        [Header("Events")]

        public UnityEvent onTriggered;

        public UnityEvent onTriggerReset;


        public virtual bool Tripped
        {
            get
            {
                if (distanceTrigger && triggerObjectWithinDistance)
                {
                    return true;
                }
                else
                {
                    return triggerColliderTripped;
                }
            }
        }


        protected virtual void Start()
        {
            if (startTriggered)
            {
                OnTriggered();
            }
            else
            {
                OnTriggerReset();
            }
        }

        protected virtual void Update()
        {
            if (triggerObject == null)
            {
                triggerObjectWithinDistance = false;
            }
            else
            {
                if (Vector3.Distance(triggerObject.position, transform.position) <= triggerDistance)
                {
                    triggerObjectWithinDistance = true;

                    if (distanceTrigger && !triggered)
                    {
                        triggered = true;
                        onTriggered.Invoke();
                    }
                }
            }

            if (!triggerObjectWithinDistance)
            {
                
            }
            else
            {
                if (Vector3.Distance(triggerObject.position, transform.position) > triggerDistance)
                {
                    triggerObjectWithinDistance = false;

                    if (resetOnDistanceExceeded)
                    {
                        triggered = false;
                        onTriggerReset.Invoke();
                    }
                }
            }
        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (triggerObject != null)
            {
                if (other.attachedRigidbody != null)
                {
                    if (other.attachedRigidbody.transform == triggerObject)
                    {
                        onTriggered.Invoke();
                    }
                }
                else
                {
                    if (other.transform == triggerObject)
                    {
                        onTriggered.Invoke();
                    }
                }
            }
        }


        protected virtual void OnTriggerExit(Collider other)
        {
            if (resetOnTriggerExit)
            {
                if (triggerObject != null)
                {
                    if (other.attachedRigidbody != null)
                    {
                        if (other.attachedRigidbody.transform == triggerObject)
                        {
                            OnTriggerReset();
                        }
                    }
                    else
                    {
                        if (other.transform == triggerObject)
                        {
                            OnTriggered();
                        }
                    }
                }
            }
        }


        protected virtual void OnTriggered()
        {
            triggered = false;
            onTriggerReset.Invoke();
        }


        protected virtual void OnTriggerReset()
        {
            triggered = false;
            onTriggerReset.Invoke();
        }


        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }
    }
}

