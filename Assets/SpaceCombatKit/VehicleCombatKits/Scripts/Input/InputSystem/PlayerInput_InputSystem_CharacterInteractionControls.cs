using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VSX.VehicleCombatKits;


namespace VSX.Characters
{
    /// <summary>
    /// Player input script for controlling character interactions, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_CharacterInteractionControls : PlayerInput_Base_CharacterInteractionControls
    {

        protected GeneralInputAsset input;


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();
            input.GeneralControls.Use.performed += ctx => Interact();
        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }


        // Get the string to display the input on the UI.
        protected override string GetControlDisplayString()
        {
            return input.GeneralControls.Use.GetBindingDisplayString();
        }
    }
}

