using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Utilities;
using VSX.Vehicles;


namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Base class for a player input script that controls a vehicle with a main gimbal (e.g. a turret).
    /// </summary>
    public class PlayerInput_Base_GimballedVehicleControls : VehicleInput
    {
        [Tooltip("How fast the gimbal rotates in response to input.")]
        [SerializeField]
        protected float lookSensitivity = 0.1f;

        [Tooltip("Settings for whether to invert the vertical gimbal rotation input for specific devices.")]
        [SerializeField]
        protected InputInvertSettings invertVerticalRotation;

        protected Vector2 lookInputValue;

        protected GimbalController gimbalController;



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            gimbalController = vehicle.GetComponent<GimbalController>();
            if (gimbalController == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + gimbalController.GetType().Name + " component was not found on the vehicle.");
                }
                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " successfully initialized.");
            }

            return true;
        }


        protected virtual InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.None;
        }


        // Called every frame that this input is running.
        protected override void OnInputUpdate()
        {           
            bool invertVertical = false;
            switch (GetLookInputDeviceType())
            {
                case InputDeviceType.Mouse:

                    if (invertVerticalRotation.InvertMouse) invertVertical = true;

                    break;

                case InputDeviceType.Keyboard:

                    if (invertVerticalRotation.InvertKeyboard) invertVertical = true;

                    break;

                case InputDeviceType.Gamepad:

                    if (invertVerticalRotation.InvertGamepad) invertVertical = true;

                    break;

                case InputDeviceType.Joystick:

                    if (invertVerticalRotation.InvertJoystick) invertVertical = true;

                    break;

            }

            gimbalController.Rotate(lookInputValue.x * lookSensitivity, -lookInputValue.y * lookSensitivity * (invertVertical ? -1 : 1));

        }
    }
}