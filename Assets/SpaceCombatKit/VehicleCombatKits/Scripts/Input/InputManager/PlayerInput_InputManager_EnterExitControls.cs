using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling characters entering and exiting vehicles, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_EnterExitControls : PlayerInput_Base_EnterExitControls
    {
        [Header("Inputs")]

        [Tooltip("Input for entering and exiting vehicles as a character.")]
        [SerializeField]
        protected CustomInput enterExitInput = new CustomInput("Vehicles", "Enter/Exit Vehicle", KeyCode.F);


        // Get the string to display the input on the UI.
        protected override string GetControlDisplayString()
        {
            return enterExitInput.GetInputAsString();
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            if (enterExitInput.Down())
            {
                EnterExit();
            }
        }
    }
}
