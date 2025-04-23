using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VSX.VehicleCombatKits;

namespace VSX.SpaceCombatKit
{
    /// <summary>
    /// Player input script for controlling a spacefighter style ship, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_SpaceshipControls : PlayerInput_Base_SpaceshipControls
    {

        protected SCKInputAsset input;

        protected InputDeviceType lastSteeringInputDeviceType = InputDeviceType.None;


        protected override void Awake()
        {
            base.Awake();

            input = new SCKInputAsset();
            
            InputSystem.onDeviceChange += OnDeviceChange;

            // Steering
            input.SpacefighterControls.Steer.performed += ctx => Steer(ctx.ReadValue<Vector2>());

            // Strafing
            input.SpacefighterControls.Strafe.performed += ctx => Strafe(ctx.ReadValue<Vector2>());

            // Roll
            input.SpacefighterControls.Roll.performed += ctx => Roll(ctx.ReadValue<float>());

            // Acceleration
            input.SpacefighterControls.Throttle.performed += ctx => Forward(ctx.ReadValue<float>());

            // Boost
            input.SpacefighterControls.Boost.performed += ctx => Boost(ctx.ReadValue<float>());

        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }


        protected virtual void Steer(Vector2 steer)
        {
            if (!CanRunInput() || !steeringEnabled) return;

            steeringInputs.x = steer.y;
            steeringInputs.y = steer.x;

            if (input.SpacefighterControls.Steer.activeControl.device is Mouse)
            {
                lastSteeringInputDeviceType = InputDeviceType.Mouse;
            }
            else if (input.SpacefighterControls.Steer.activeControl.device is Gamepad)
            {
                lastSteeringInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (input.SpacefighterControls.Steer.activeControl.device is Keyboard)
            {
                lastSteeringInputDeviceType = InputDeviceType.Keyboard;
            }
            else if (input.SpacefighterControls.Steer.activeControl.device is Joystick)
            {
                lastSteeringInputDeviceType = InputDeviceType.Joystick;
            }
        }

        
        protected override InputDeviceType GetSteeringInputDeviceType()
        {
            return lastSteeringInputDeviceType;
        }


        protected virtual void Strafe(Vector2 strafe)
        {
            if (!CanRunInput() || !movementEnabled) return;

            movementInputs.x = strafe.x;
            movementInputs.y = strafe.y;
        }


        protected virtual void Forward(float throttle)
        {
            if (!CanRunInput() || !movementEnabled) return;
            movementInputs.z = throttle;
        }


        protected virtual void Boost(float boost)
        {
            if (!CanRunInput() || !movementEnabled) return;

            boostInputs.z = boost;
        }


        protected virtual void Roll(float roll)
        {
            if (!CanRunInput() || !steeringEnabled) return;

            steeringInputs.z = roll;

            if (input.SpacefighterControls.Roll.activeControl.device is Mouse)
            {
                lastSteeringInputDeviceType = InputDeviceType.Mouse;
            }
            else if (input.SpacefighterControls.Roll.activeControl.device is Gamepad)
            {
                lastSteeringInputDeviceType = InputDeviceType.Gamepad;
            }
            else if (input.SpacefighterControls.Roll.activeControl.device is Joystick)
            {
                lastSteeringInputDeviceType = InputDeviceType.Joystick;
            }
        }


        protected virtual void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (Mouse.current == null || !mouseEnabled || Gamepad.current != null)
            {
                if (controlHUDCursor && hudCursor != null) hudCursor.CenterCursor();
                reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);
            }
        }


        protected override Vector3 GetMouseViewportPosition()
        {
            return new Vector3(Mouse.current.position.ReadValue().x / Screen.width, Mouse.current.position.ReadValue().y / Screen.height, 0);
        }
    }
}