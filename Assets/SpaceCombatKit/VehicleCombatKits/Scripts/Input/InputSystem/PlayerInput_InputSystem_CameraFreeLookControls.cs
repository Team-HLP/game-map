using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling camera free look mode on a vehicle, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_CameraFreeLookControls : PlayerInput_Base_CameraFreeLookControls
    {
        
        protected GeneralInputAsset input;

        protected InputDeviceType lastLookInputDeviceType = InputDeviceType.None;



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

            input.VehicleControls.FreeLook.performed += ctx => Look(ctx.ReadValue<Vector2>());

            input.VehicleControls.FreeLookMode.started += ctx => EnterFreeLookMode();

            input.VehicleControls.FreeLookMode.canceled += ctx => ExitFreeLookMode();

        }


        protected virtual void Look(Vector2 look)
        {
            lookInputValue = look;

            if (input.VehicleControls.FreeLook.activeControl.device is Mouse)
            {
                lastLookInputDeviceType = InputDeviceType.Mouse;
            }
            else if (input.VehicleControls.FreeLook.activeControl.device is Gamepad)
            {
                lastLookInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (input.VehicleControls.FreeLook.activeControl.device is Keyboard)
            {
                lastLookInputDeviceType = InputDeviceType.Keyboard;
            }
            else if (input.VehicleControls.FreeLook.activeControl.device is Joystick)
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