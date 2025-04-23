using System;
using UnityEngine;
using VSX.Controls.UnityInputManager;
using VSX.VehicleCombatKits;

namespace VSX.Characters
{
    /// <summary>
    /// Player input script for controlling a first person character, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_FirstPersonCharacterControls : PlayerInput_Base_FirstPersonCharacterControls
    {

        [Header("Inputs")]

        [Tooltip("Input for making the character look up/down.")]
        [SerializeField]
        protected CustomInput lookVerticalInput = new CustomInput("Characters", "Look Vertical", "Mouse Y");

        [Tooltip("Input for making the character look left/right.")]
        [SerializeField]
        protected CustomInput lookHorizontalInput = new CustomInput("Characters", "Look Horizontal", "Mouse X");

        [Tooltip("Input for making the character walk forward or backward.")]
        [SerializeField]
        protected CustomInput walkForwardBackwardAxisInput = new CustomInput("Characters", "Walk", "Vertical");

        [Tooltip("Input for making the character strafe left and right.")]
        [SerializeField]
        protected CustomInput strafeHorizontalAxisInput = new CustomInput("Characters", "Strafe Horizontal", "Horizontal");

        [Tooltip("Input for making the character strafe vertically.")]
        [SerializeField]
        protected CustomInput strafeVerticalInput = new CustomInput("Characters", "Strafe Vertical", "Vertical");

        [Tooltip("Input for making the character run.")]
        [SerializeField]
        protected CustomInput runInput = new CustomInput("Characters", "Run", KeyCode.LeftShift);

        [Tooltip("Input for making the character jump.")]
        [SerializeField]
        protected CustomInput jumpInput = new CustomInput("Characters", "Jump", KeyCode.Space);


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.Mouse;
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            lookInputValue.x = lookHorizontalInput.FloatValue();
            lookInputValue.y = lookVerticalInput.FloatValue();

            movementInputValue.x = strafeHorizontalAxisInput.FloatValue();
            movementInputValue.y = strafeVerticalInput.FloatValue();
            movementInputValue.z = walkForwardBackwardAxisInput.FloatValue();


            if (jumpInput.Down())
            {
                Jump();
            }


            if (runInput.Down())
            {
                StartRunning();
            }


            if (runInput.Up())
            {
                StopRunning();
            }

            base.OnInputUpdate();
        }
    }
}
