using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Utilities;
using VSX.VehicleCombatKits;
using VSX.Vehicles;

namespace VSX.Characters
{
    /// <summary>
    /// Base class for a player input script that controls a first person character.
    /// </summary>
    public class PlayerInput_Base_FirstPersonCharacterControls : VehicleInput
    {

        [Tooltip("How fast the character looks around in response to input.")]
        [SerializeField]
        protected float lookSensitivity = 1f;

        [Tooltip("Whether to invert the vertical look input for each input device type.")]
        [SerializeField]
        protected InputInvertSettings invertLookVertical;

        protected Vector3 movementInputValue;
        protected Vector2 lookInputValue;

        protected GimbalController gimbalController;
        protected FirstPersonCharacterController characterController;



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <returns>Whether initialization succeeded</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            characterController = vehicle.GetComponent<FirstPersonCharacterController>();
            gimbalController = vehicle.GetComponent<GimbalController>();
            if (characterController == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + typeof(CharacterController).Name + " component was not found on the vehicle.");
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


        protected virtual void Jump()
        {
            if (!CanRunInput()) return;

            characterController.Jump();
        }


        protected virtual void StartRunning()
        {
            if (!CanRunInput()) return;

            characterController.SetRunning(true);
        }


        protected virtual void StopRunning()
        {
            if (!CanRunInput()) return;

            characterController.SetRunning(false);
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            float horizontal = movementInputValue.x;
            float forward = movementInputValue.z;

            // Look

            float lookVertical = lookInputValue.y;
            switch (GetLookInputDeviceType())
            {
                case InputDeviceType.Mouse:

                    if (invertLookVertical.InvertMouse) lookVertical *= -1;

                    break;

                case InputDeviceType.Keyboard:

                    if (invertLookVertical.InvertKeyboard) lookVertical *= -1;

                    break;

                case InputDeviceType.Gamepad:

                    if (invertLookVertical.InvertGamepad) lookVertical *= -1;

                    break;

                case InputDeviceType.Joystick:

                    if (invertLookVertical.InvertJoystick) lookVertical *= -1;

                    break;
            }

            gimbalController.Rotate(lookInputValue.x * lookSensitivity, -lookVertical * lookSensitivity);

            // Move

            characterController.SetMovementInputs(horizontal, 0, forward);
        }
    }
}
