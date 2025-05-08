using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using VSX.VehicleCombatKits;


namespace VSX.SpaceCombatKit
{

    /// <summary>
    /// Player input script for controlling a capital ship, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_CapitalShipControls : PlayerInput_Base_CapitalShipControls
    {

        protected SCKInputAsset SCKInput;
        protected GeneralInputAsset generalInput;

        protected InputDeviceType lastLookInputDeviceType = InputDeviceType.None;


        protected virtual void OnEnable()
        {
            SCKInput.Enable();
            generalInput.Enable();
        }


        protected virtual void OnDisable()
        {
            SCKInput.Disable();
            generalInput.Disable();
        }


        protected virtual void Look(Vector2 look)
        {
            lookInputValue = look;

            if (SCKInput.CapitalShipControls.Look.activeControl.device is Mouse)
            {
                lastLookInputDeviceType = InputDeviceType.Mouse;
            }
            else if (SCKInput.CapitalShipControls.Look.activeControl.device is Gamepad)
            {
                lastLookInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (SCKInput.CapitalShipControls.Look.activeControl.device is Keyboard)
            {
                lastLookInputDeviceType = InputDeviceType.Keyboard;
            }
            else if (SCKInput.CapitalShipControls.Look.activeControl.device is Joystick)
            {
                lastLookInputDeviceType = InputDeviceType.Joystick;
            }
        }


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return lastLookInputDeviceType;
        }


        protected override void Awake()
        {
            base.Awake();

            SCKInput = new SCKInputAsset();

            // Gimbal rotation
            SCKInput.CapitalShipControls.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());

            // Steering
            SCKInput.CapitalShipControls.Steer.performed += ctx => { steeringInputValue.x = ctx.ReadValue<Vector2>().y; steeringInputValue.y = ctx.ReadValue<Vector2>().x; };

            // Strafing
            SCKInput.CapitalShipControls.Strafe.performed += ctx => { movementInputValue.x = ctx.ReadValue<Vector2>().x; movementInputValue.y = ctx.ReadValue<Vector2>().y; };

            // Acceleration
            SCKInput.CapitalShipControls.Throttle.performed += ctx => movementInputValue.z = ctx.ReadValue<float>();

            // Boost
            SCKInput.CapitalShipControls.Boost.performed += ctx => boostInputValue.z = ctx.ReadValue<float>();

            generalInput = new GeneralInputAsset();

            // Zoom
            generalInput.CameraControls.Zoom.performed += ctx => zoomInputValue = ctx.ReadValue<float>();

        }
    }
}
