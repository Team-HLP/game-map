using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;

namespace VSX.SpaceCombatKit
{
    /// <summary>
    /// Player input script for controlling landing/takeoff of a vehicle, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_ShipLanderControls : PlayerInput_Base_ShipLanderControls
    {

        [Header("Inputs")]

        [Tooltip("Input for launching or landing a spaceship.")]
        [SerializeField]
        protected CustomInput landingInput = new CustomInput("Spaceships", "Land/Take Off", KeyCode.L);


        // Get the string to display the input on the UI.
        protected override string GetControlDisplayString()
        {
            return landingInput.GetInputAsString();
        }


        protected override void OnInputUpdate()
        {
            if (landingInput.Down())
            {
                LaunchLand();
            }

            base.OnInputUpdate();
        }
    }
}