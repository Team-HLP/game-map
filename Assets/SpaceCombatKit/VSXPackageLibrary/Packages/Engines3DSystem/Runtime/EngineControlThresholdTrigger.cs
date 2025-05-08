using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Utilities;

namespace VSX.Engines3D
{

    /// <summary>
    /// Calls events when an engine control crosses a threshold. Useful for running events e.g. when boost is activated or deactivated.
    /// </summary>
    public class EngineControlThresholdTrigger : MonoBehaviour
    {

        [Tooltip("The engines to trigger events for.")]
        [SerializeField]
        protected Engines engines;

        [Tooltip("The control type to run events for.")]
        [SerializeField]
        protected EngineControlType controlType;

        [Tooltip("The control axis to run events for.")]
        [SerializeField]
        protected Axis axis = Axis.Any;

        [Tooltip("The control threshold value which, when crossed, triggers the events.")]
        [SerializeField]
        protected float threshold = 0.5f;

        [Tooltip("Whether the events are triggered for a positive crossing (when the value exceeds the threshold). Uncheck for negative crossing (when value goes below threshold).")]
        [SerializeField]
        protected bool positiveCrossing = true;

        [Tooltip("Whether the events are triggered for both the positive and negative axes of the control (e.g. both left AND right, forward AND back, up AND down).")]
        [SerializeField]
        protected bool biDirectional = false;

        protected Vector3 previousMovementInputs = Vector3.zero;
        protected Vector3 previousSteeringInputs = Vector3.zero;
        protected Vector3 previousBoostInputs = Vector3.zero;

        [Tooltip("The event called when the trigger is triggered.")]
        public UnityEvent onTriggered;


        protected virtual void Start()
        {
            if (engines != null)
            {
                engines.onMovementInputsChanged += OnMovementInputsChanged;
                engines.onSteeringInputsChanged += OnSteeringInputsChanged;
                engines.onBoostInputsChanged += OnBoostInputsChanged;
            }
        }


        // Check if the threshold has been crossed using the current and previous value.
        protected virtual bool CheckThreshold(float currentValue, float previousValue)
        {
            if (biDirectional)
            {
                return (Mathf.Abs(currentValue) >= threshold && Mathf.Abs(previousValue) < threshold);
            }
            else
            {
                if (positiveCrossing)
                {
                    return (currentValue >= threshold && previousValue < threshold);
                }
                else
                {
                    return (currentValue < threshold && previousValue >= threshold);
                }
            }
        }


        // Trigger the events.
        protected virtual void Trigger()
        {
            onTriggered.Invoke();
        }


        // Update the stored values to check against to determine if the threshold has been crossed.
        protected virtual void UpdateStoredValues()
        {
            previousMovementInputs = engines.ModulatedMovementInputs;
            previousSteeringInputs = engines.ModulatedSteeringInputs;
            previousBoostInputs = engines.ModulatedBoostInputs;
        }


        // Called when the engine's movement inputs change
        protected virtual void OnMovementInputsChanged()
        {
            if (controlType != EngineControlType.Movement)
            {
                UpdateStoredValues();
                return;
            }

            if (axis == Axis.X || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedMovementInputs.x, previousMovementInputs.x))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }


            if (axis == Axis.Y || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedMovementInputs.y, previousMovementInputs.y))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }


            if (axis == Axis.Z || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedMovementInputs.z, previousMovementInputs.z))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }
        }


        // Called when the engine's steering inputs change.
        protected virtual void OnSteeringInputsChanged()
        {
            if (controlType != EngineControlType.Steering)
            {
                UpdateStoredValues();
                return;
            }

            if (axis == Axis.X || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedSteeringInputs.x, previousSteeringInputs.x))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }


            if (axis == Axis.Y || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedSteeringInputs.y, previousSteeringInputs.y))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }


            if (axis == Axis.Z || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedSteeringInputs.z, previousSteeringInputs.z))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }
        }


        // Called when the engine's boost inputs change.
        protected virtual void OnBoostInputsChanged()
        {
            if (controlType != EngineControlType.Boost)
            {
                UpdateStoredValues();
                return;
            }


            if (axis == Axis.X || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedBoostInputs.x, previousBoostInputs.x))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }


            if (axis == Axis.Y || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedBoostInputs.y, previousBoostInputs.y))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }


            if (axis == Axis.Z || axis == Axis.Any)
            {
                if (CheckThreshold(engines.ModulatedBoostInputs.z, previousBoostInputs.z))
                {
                    UpdateStoredValues();
                    Trigger();
                    return;
                }
            }
        }        
    }
}

