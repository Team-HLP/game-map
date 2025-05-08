using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Utilities;


namespace VSX.Controls
{
    /// <summary>
    /// Base class for vehicle input components.
    /// </summary>
    public abstract class GeneralInput : MonoBehaviour
    {

        [SerializeField]
        protected GameObject targetObject;

        // Whether this input component has everything it needs to run
        protected bool initialized = false;
        public bool Initialized { get { return initialized; } }

        [SerializeField]
        protected float inputStartDelay = 0.1f;
        protected float inputStartDelayStartTime;
        protected bool inputStartDelayActive = false;

        [SerializeField]
        protected List<Condition> inputUpdateConditions = new List<Condition>();

        [SerializeField]
        protected bool debugInitialization = false;


        protected bool started = false;
        public virtual bool Started { get { return started; } }


        protected virtual void Reset()
        {
            targetObject = gameObject;
        }


        protected virtual void Awake() { }


        protected virtual void Start()
        {
            SetTargetObject(targetObject);
        }


        protected virtual void SetTargetObject(GameObject targetObject)
        {
            if (initialized)
            {
                OnUninitialized(targetObject);
                this.targetObject = null;
                initialized = false;             
            }

            if (Initialize(targetObject))
            {
                initialized = true;
                StartCoroutine(InputStartDelayCoroutine());
                OnInitialized(targetObject);
            }
        }


        IEnumerator InputStartDelayCoroutine()
        {
            inputStartDelayActive = true;

            yield return new WaitForSeconds(inputStartDelay);

            inputStartDelayActive = false;
        }


        /// <summary>
        /// Attempt to initialize the input component.
        /// </summary>
        /// <returns> Whether initialization was successful. </returns>
        protected virtual bool Initialize(GameObject targetObject) { return targetObject != null; }


        protected virtual void OnUninitialized(GameObject targetObject) { }


        protected virtual void OnInitialized(GameObject targetObject) { }


        /// <summary>
        /// Put all your input code in this method.
        /// </summary>
        protected virtual void OnInputUpdate() { }


        public virtual void StartInput() { started = true; }


        public virtual void StopInput() { started = false; }


        public virtual bool CanRunInput()
        {
            if (inputStartDelayActive) return false;

            if (!gameObject.activeInHierarchy) return false;

            if (!enabled) return false;

            if (!initialized) return false;

            for (int i = 0; i < inputUpdateConditions.Count; ++i)
            {
                if (!inputUpdateConditions[i].Value())
                {
                    return false;
                }
            }

            return true;
        }


        protected virtual bool InputUpdate()
        {
            if (CanRunInput())
            {
                if (!started)
                {
                    StartInput();
                }

                OnInputUpdate();

                return true;
            }
            else
            {
                if (started)
                {
                    StopInput();
                }

                return false;
            }
        }


        protected virtual void Update()
        {
            InputUpdate();
        }
    }
}