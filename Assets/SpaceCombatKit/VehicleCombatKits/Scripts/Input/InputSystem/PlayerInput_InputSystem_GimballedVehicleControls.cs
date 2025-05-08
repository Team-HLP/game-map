using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Player input script for controlling a vehicle with a main gimbal (e.g. turret), using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_GimballedVehicleControls : PlayerInput_Base_GimballedVehicleControls
    {

        protected GeneralInputAsset input;

        protected InputDeviceType lastLookInputDeviceType = InputDeviceType.None;



        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Enable();
        }


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            input.CoreControls.MoveCursor.performed += ctx => Look(ctx.ReadValue<Vector2>());

        }


        protected virtual void Look(Vector2 look)
        {
            lookInputValue = look;

            if (input.CoreControls.MoveCursor.activeControl.device is Mouse)
            {
                lastLookInputDeviceType = InputDeviceType.Mouse;
            }
            else if (input.CoreControls.MoveCursor.activeControl.device is Gamepad)
            {
                lastLookInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (input.CoreControls.MoveCursor.activeControl.device is Keyboard)
            {
                lastLookInputDeviceType = InputDeviceType.Keyboard;
            }
            else if (input.CoreControls.MoveCursor.activeControl.device is Joystick)
            {
                lastLookInputDeviceType = InputDeviceType.Joystick;
            }
        }


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return lastLookInputDeviceType;
        }
    }
}