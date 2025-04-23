using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for interacting with a vehicle loadout menu, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_LoadoutControls : PlayerInput_Base_LoadoutControls
    {
        [Header("Inputs")]

        [Tooltip("Input for engaging the view rotation.")]
        [SerializeField]
        protected CustomInput engageViewRotationInput;

        [Tooltip("Input for navigating to the main menu.")]
        [SerializeField]
        protected CustomInput mainMenuInput;

        [Tooltip("Input for going back through the menu.")]
        [SerializeField]
        protected CustomInput backInput;

        [Tooltip("Input for starting the mission/next scene after loadout is complete.")]
        [SerializeField]
        protected CustomInput startActionInput;

        [Tooltip("Input for accepting the currently highlighted UI element or contextual action.")]
        [SerializeField]
        protected CustomInput acceptInput;

        [Tooltip("Input for deleting the currently selected configuration.")]
        [SerializeField]
        protected CustomInput deleteInput;

        [Tooltip("Input for cycling forward through the vehicle options.")]
        [SerializeField]
        protected CustomInput cycleVehicleSelectionForwardInput;

        [Tooltip("Input for cycling backward through the vehicle options.")]
        [SerializeField]
        protected CustomInput cycleVehicleSelectionBackInput;

        [Tooltip("Input for cycling forward through the module options.")]
        [SerializeField]
        protected CustomInput cycleModuleSelectionForwardInput;

        [Tooltip("Input for cycling backward through the module options.")]
        [SerializeField]
        protected CustomInput cycleModuleSelectionBackInput;

        [Tooltip("Input for cycling forward through the vehicle's module mounts.")]
        [SerializeField]
        protected CustomInput cycleModuleMountSelectionForwardInput;

        [Tooltip("Input for cycling backward through the vehicle's module mounts.")]
        [SerializeField]
        protected CustomInput cycleModuleMountSelectionBackInput;

        [Tooltip("Input for rotating the view horizontally.")]
        [SerializeField]
        protected CustomInput rotateViewHorizontalInput;

        [Tooltip("Input for rotating the view vertically.")]
        [SerializeField]
        protected CustomInput rotateViewVerticalInput;


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            if (engageViewRotationInput.Pressed())
            {
                viewRotationInputValue.x = rotateViewHorizontalInput.FloatValue();
                viewRotationInputValue.y = rotateViewVerticalInput.FloatValue();
            }            

            if (mainMenuInput.Down()) Menu();

            if (backInput.Down()) Back();

            if (startActionInput.Down()) StartAction();

            if (acceptInput.Down()) Accept();

            if (deleteInput.Down()) Delete();

            if (cycleVehicleSelectionForwardInput.Down()) CycleVehicleSelection(true);

            if (cycleVehicleSelectionBackInput.Down()) CycleVehicleSelection(false);

            if (cycleModuleSelectionForwardInput.Down()) CycleModuleSelection(true);

            if (cycleModuleSelectionBackInput.Down()) CycleModuleSelection(false);

            if (cycleModuleMountSelectionForwardInput.Down()) CycleModuleMountSelection(true);

            if (cycleModuleMountSelectionBackInput.Down()) CycleModuleMountSelection(false);

            base.OnInputUpdate();
        }
    }
}


