using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.Characters
{
    /// <summary>
    /// Player input script for controlling character interactions, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_CharacterInteractionControls : PlayerInput_Base_CharacterInteractionControls
    {
        [Header("Inputs")]

        [Tooltip("Input for interacting with an interactable object.")]
        [SerializeField]
        protected CustomInput interactInput = new CustomInput("Characters", "Interact", KeyCode.F);


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            if (interactInput.Down())
            {
                Interact();
            }
        }


        // Get the string for the enter/exit control on the UI.
        protected override string GetControlDisplayString()
        {
            return interactInput.GetInputAsString();
        }
    }
}

