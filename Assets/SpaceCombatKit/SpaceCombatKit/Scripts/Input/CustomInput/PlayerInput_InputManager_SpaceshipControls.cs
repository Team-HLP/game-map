using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;
using VSX.VehicleCombatKits;

namespace VSX.SpaceCombatKit
{
    /// <summary>
    /// Player input script for controlling the steering and movement of a space fighter, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_SpaceshipControls : PlayerInput_Base_SpaceshipControls
    {

        [Header("Inputs")]

        [Tooltip("Input for the pitch (local X axis rotation) steering.")]
        [SerializeField]
        protected CustomInput pitchAxisInput = new CustomInput("Space Fighters", "Pitch", "Mouse Y");

        [Tooltip("Input for the yaw (local Y axis rotation) steering.")]
        [SerializeField]
        protected CustomInput yawAxisInput = new CustomInput("Space Fighters", "Yaw", "Mouse X");

        [Tooltip("Input for the roll (local Z axis rotation) steering.")]
        [SerializeField]
        protected CustomInput rollAxisInput = new CustomInput("Space Fighters", "Roll", "Roll");

        [Tooltip("Input for controlling the throttle.")]
        [SerializeField]
        protected CustomInput throttleAxisInput = new CustomInput("Space Fighters", "Move Forward/Back", "Vertical");

        [Tooltip("Input for strafing the ship vertically.")]
        [SerializeField]
        protected CustomInput strafeVerticalInput = new CustomInput("Space Fighters", "Strafe Vertical", "Strafe Vertical");

        [Tooltip("Input for strafing the ship horizontally.")]
        [SerializeField]
        protected CustomInput strafeHorizontalInput = new CustomInput("Space Fighters", "Strafe Horizontal", "Strafe Horizontal");

        [Tooltip("Input for boosting.")]
        [SerializeField]
        protected CustomInput boostInput = new CustomInput("Space Fighters", "Boost", KeyCode.Tab);


        protected override InputDeviceType GetSteeringInputDeviceType()
        {
            return InputDeviceType.Mouse;
        }


        protected override void OnInputUpdate()
        {           
            movementInputs.x = strafeHorizontalInput.FloatValue();
            movementInputs.y = strafeVerticalInput.FloatValue();
            movementInputs.z = throttleAxisInput.FloatValue();

            steeringInputs.x = pitchAxisInput.FloatValue();
            steeringInputs.y = yawAxisInput.FloatValue();
            steeringInputs.z = -rollAxisInput.FloatValue();

            boostInputs.z = boostInput.FloatValue();

            base.OnInputUpdate();
        }
    }
}