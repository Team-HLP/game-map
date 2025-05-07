using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace VSX.SpaceCombatKit
{

    /// <summary>
    /// Player input script for controlling takeoff/landing of a vehicle, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_ShipLanderControls : PlayerInput_Base_ShipLanderControls
    { 
    
        protected SCKInputAsset input;


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

            input = new SCKInputAsset();

            // Steering
            input.SpacefighterControls.LaunchLand.performed += ctx => LaunchLand();
        }


        // Get string to display the input on the UI.
        protected override string GetControlDisplayString()
        {
            return input.SpacefighterControls.LaunchLand.GetBindingDisplayString();
        }
    }
}
