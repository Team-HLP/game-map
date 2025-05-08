using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling a vehicle's auto aim functionality, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_AutoAimControls : PlayerInput_Base_AutoAimControls
    {

        protected GeneralInputAsset input;

        
        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            input.TargetingControls.AutoAimToggle.performed += ctx => ToggleAutoAim();
        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }
    }
}

