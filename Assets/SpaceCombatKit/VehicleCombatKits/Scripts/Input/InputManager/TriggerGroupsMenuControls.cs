using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;
using VSX.Weapons;
using VSX.Controls;


namespace VSX.VehicleCombatKits
{

    [System.Serializable]
    public class TriggerIndexInput
    {
        public int triggerIndex;
        public CustomInput input;
    }

    public class TriggerGroupsMenuControls : GeneralInput
    {

        [Header("Settings")]

        [SerializeField]
        protected TriggerableGroupsMenuController triggerGroupsMenuController;

        [SerializeField]
        protected List<TriggerIndexInput> triggerIndexInputs = new List<TriggerIndexInput>();


        protected override bool Initialize(GameObject inputTargetObject)
        {
            return (triggerGroupsMenuController != null);
        }

        protected override void OnInputUpdate()
        {
            for(int i = 0; i < triggerIndexInputs.Count; ++i)
            {
                if (triggerIndexInputs[i].input.Down())
                {
                    triggerGroupsMenuController.SetTriggerGroupTriggerValue(triggerIndexInputs[i].triggerIndex);
                }
            }
        }
    }
}

