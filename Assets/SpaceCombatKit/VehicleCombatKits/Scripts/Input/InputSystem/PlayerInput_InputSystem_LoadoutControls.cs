using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Player input script for interacting with a vehicle loadout menu, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_LoadoutControls : PlayerInput_Base_LoadoutControls
    {
        
        protected LoadoutInputAsset input;

        protected InputDeviceType lastViewRotationInputDeviceType = InputDeviceType.None;



        protected override void Awake()
        {
            base.Awake();
            
            input = new LoadoutInputAsset();

            input.LoadoutControls.RotateView.performed += ctx => RotateView(ctx.ReadValue<Vector2>());

            input.LoadoutControls.Menu.performed += ctx => Menu();

            input.LoadoutControls.Back.performed += ctx => Back();

            input.LoadoutControls.Start.performed += ctx => StartAction();

            input.LoadoutControls.Accept.performed += ctx => Accept();

            input.LoadoutControls.Delete.performed += ctx => Delete();

            input.LoadoutControls.CycleVehicleSelection.performed += ctx => { if (ctx.ReadValue<float>() > 0.5f) CycleVehicleSelection(true); else if (ctx.ReadValue<float>() < -0.5f) CycleVehicleSelection(false); };
            
            input.LoadoutControls.CycleModuleSelection.performed += ctx => { if (ctx.ReadValue<float>() > 0.5f) CycleModuleSelection(true); else if (ctx.ReadValue<float>() < -0.5f) CycleModuleSelection(false); };

            input.LoadoutControls.CycleModuleMountSelection.performed += ctx => { if (ctx.ReadValue<float>() > 0.5f) CycleModuleMountSelection(true); else if (ctx.ReadValue<float>() < -0.5f) CycleModuleMountSelection(false); };

        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }



        protected virtual void RotateView(Vector2 look)
        {
            viewRotationInputValue = look;

            if (input.LoadoutControls.RotateView.activeControl.device is Mouse)
            {
                lastViewRotationInputDeviceType = InputDeviceType.Mouse;
            }
            else if (input.LoadoutControls.RotateView.activeControl.device is Gamepad)
            {
                lastViewRotationInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (input.LoadoutControls.RotateView.activeControl.device is Keyboard)
            {
                lastViewRotationInputDeviceType = InputDeviceType.Keyboard;
            }
            else if (input.LoadoutControls.RotateView.activeControl.device is Joystick)
            {
                lastViewRotationInputDeviceType = InputDeviceType.Joystick;
            }
        }


        protected override InputDeviceType GetViewRotationInputDeviceType()
        {
            return lastViewRotationInputDeviceType;
        }
    }
}
