using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Player input script for controlling characters entering and exiting vehicles, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_EnterExitControls : PlayerInput_Base_EnterExitControls
    {

        protected GeneralInputAsset input;


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            // Steering
            input.GeneralControls.Use.performed += ctx => EnterExit();
        }


        // Get the string to display the input on the UI.
        protected override string GetControlDisplayString()
        {
            return input.GeneralControls.Use.GetBindingDisplayString();
        }
    }
}