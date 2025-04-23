using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for interacting with a generic menu (e.g. pause menu), using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_MenuControls : PlayerInput_Base_MenuControls
    {
        [Header("Inputs")]

        [Tooltip("Input for navigating back through the menu.")]
        [SerializeField]
        protected CustomInput backInput;

        [Tooltip("Input for opening the menu from a different context.")]
        [SerializeField]
        protected CustomInput toggleMenuInput;


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            if (backInput.Down()) Back();

            if (toggleMenuInput.Down()) ToggleMenu();
        }
    }
}

