using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Utilities;
using VSX.Vehicles;
using UnityEngine.InputSystem;
using VSX.VehicleCombatKits;

namespace VSX.Characters
{

    /// <summary>
    /// Player input script for controlling a first person character, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_FirstPersonCharacterControls : PlayerInput_Base_FirstPersonCharacterControls
    {

        protected CharacterInputAsset input;

        protected GeneralInputAsset generalInput;

        protected InputDeviceType lastLookInputDeviceType = InputDeviceType.None;


        protected override void Awake()
        {
            base.Awake();

            input = new CharacterInputAsset();
            generalInput = new GeneralInputAsset();

            input.CharacterControls.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
            input.CharacterControls.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
            input.CharacterControls.Jump.performed += ctx => Jump();
            input.CharacterControls.Run.performed += ctx => StartRunning();
            input.CharacterControls.Run.canceled += ctx => StopRunning();
        }


        protected virtual void OnEnable()
        {
            input.Enable();
            generalInput.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
            generalInput.Disable();
        }


        protected virtual void Look(Vector2 look)
        {
            lookInputValue = look;

            if (input.CharacterControls.Look.activeControl.device is Mouse)
            {
                lastLookInputDeviceType = InputDeviceType.Mouse;
            }
            else if (input.CharacterControls.Look.activeControl.device is Gamepad)
            {
                lastLookInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (input.CharacterControls.Look.activeControl.device is Keyboard)
            {
                lastLookInputDeviceType = InputDeviceType.Keyboard;
            }
            else if (input.CharacterControls.Look.activeControl.device is Joystick)
            {
                lastLookInputDeviceType = InputDeviceType.Joystick;
            }
        }


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return lastLookInputDeviceType;
        }


        protected virtual void Move(Vector2 inputValue)
        {
            movementInputValue.x = inputValue.x;
            movementInputValue.z = inputValue.y;
        }
    }
}