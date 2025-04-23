using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling a vehicle's auto aim functionality, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_AutoAimControls : PlayerInput_Base_AutoAimControls
    {
        [Header("Inputs")]

        [Tooltip("Input for toggling auto aim.")]
        [SerializeField]
        protected CustomInput toggleAutoAimInput = new CustomInput("Vehicle Controls", "Toggle Auto Aim", KeyCode.T);


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            if (toggleAutoAimInput.Down())
            {
                ToggleAutoAim();
            }

            base.OnInputUpdate();
        }
    }
}

