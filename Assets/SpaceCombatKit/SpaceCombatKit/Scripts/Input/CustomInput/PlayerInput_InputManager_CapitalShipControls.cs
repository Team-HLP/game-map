using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Controls.UnityInputManager;
using VSX.VehicleCombatKits;


namespace VSX.SpaceCombatKit
{

    /// <summary>
    /// Player input script for controlling a capital ship, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_CapitalShipControls : PlayerInput_Base_CapitalShipControls
    {
        [Header("Inputs")]

        [Tooltip("Input for yawing the ship left and right.")]
        [SerializeField]
        protected CustomInput yawInput = new CustomInput("Capital Ships", "Rotate Horizontally", "Roll");

        [Tooltip("Input for rotating the camera horizontally.")]
        [SerializeField]
        protected CustomInput lookHorizontalInput = new CustomInput("Capital Ships", "Look Horizontally", "Mouse X");

        [Tooltip("Input for rotating the camera vertically.")]
        [SerializeField]
        protected CustomInput lookVerticalInput = new CustomInput("Capital Ships", "Look Vertically", "Mouse Y");

        [Tooltip("Throttle input axis.")]
        [SerializeField]
        protected CustomInput throttleInput = new CustomInput("Capital Ships", "Throttle Up", "Vertical");

        [Tooltip("Input for moving left/right.")]
        [SerializeField]
        protected CustomInput strafeHorizontalInput = new CustomInput("Capital Ships", "Strafe Horizontal", "Strafe Horizontal");

        [Tooltip("Input for moving up/down.")]
        [SerializeField]
        protected CustomInput strafeVerticalInput = new CustomInput("Capital Ships", "Strafe Vertical", "Strafe Vertical");     

        [Tooltip("Input for boosting.")]
        [SerializeField]
        protected CustomInput boostInput = new CustomInput("Capital Ships", "Boost", KeyCode.Tab);


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.Mouse;
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            lookInputValue.x = lookHorizontalInput.FloatValue();
            lookInputValue.y = lookVerticalInput.FloatValue();

            steeringInputValue.y = -yawInput.FloatValue();
            movementInputValue.z = throttleInput.FloatValue();

            movementInputValue.x = strafeHorizontalInput.FloatValue();
            movementInputValue.y = strafeVerticalInput.FloatValue();

            boostInputValue.z = boostInput.FloatValue();

            base.OnInputUpdate();
        }
    }
}
