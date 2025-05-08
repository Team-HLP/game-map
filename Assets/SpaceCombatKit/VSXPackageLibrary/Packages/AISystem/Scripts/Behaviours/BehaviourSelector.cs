using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.AI
{
    /// <summary>
    /// This class holds a list of AI vehicle behaviours, and every frame, it tries each of the behaviours until one of them succeeds in updating, at which point
    /// it finishes for that frame. This way, the list represents a prioritized list where only one item in the list is updated each frame.
    /// 
    /// This component is basically a standalone Behaviour Tree Selector node.
    /// </summary>
    public class BehaviourSelector : VehicleInput
    {

        [Tooltip("The list of behaviours to select from. Will try each one in succession until one successfully runs.")]
        [SerializeField]
        protected List<AIVehicleBehaviour> behaviours = new List<AIVehicleBehaviour>();
        protected int currentBehaviourIndex = -1;


        protected override void Start()
        {
            base.Start();

            foreach (AIVehicleBehaviour behaviour in behaviours)
            {
                if (behaviour.UpdateEveryFrame)
                {
                    Debug.LogError("Behaviour " + behaviour.name + " is set to 'Update Every Frame' while managed by a Behaviour Selector. Remove it from the Behaviour Selector's Behaviours list or uncheck this setting in the inspector.");
                }
            }
        }


        protected override bool Initialize(Vehicle vehicle)
        {

            // Success
            for (int i = 0; i < behaviours.Count; ++i)
            {
                behaviours[i].SetVehicle(vehicle);
            }

            return true;
        }


        protected virtual void SetSelection(int selectionIndex)
        {
            for (int i = 0; i < behaviours.Count; ++i)
            {
                if (i == selectionIndex)
                {
                    behaviours[i].StartBehaviour();
                    currentBehaviourIndex = i;
                }
                else
                {
                    behaviours[i].StopBehaviour();
                }
            }
        }


        protected override void OnInputUpdate()
        {
            for (int i = 0; i < behaviours.Count; ++i)
            {
                if (behaviours[i].BehaviourUpdate())
                {
                    if (currentBehaviourIndex != i)
                    {
                        SetSelection(i);
                    }
                    break;
                }
            }
        }
    }
}