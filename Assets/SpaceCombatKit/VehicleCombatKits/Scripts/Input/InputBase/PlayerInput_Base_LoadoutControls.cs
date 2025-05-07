using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Controls;
using VSX.Loadouts;


namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Base class for a player input script that interacts with a vehicle loadout menu.
    /// </summary>
    public class PlayerInput_Base_LoadoutControls : GeneralInput
    {
        [Tooltip("The component controlling the loadout UI.")]
        [SerializeField]
        protected LoadoutUIController loadoutUIController;

        [Tooltip("The loadout camera controller.")]
        [SerializeField]
        protected LoadoutCameraController loadoutCameraController;

        protected Vector3 viewRotationInputValue;
        protected bool viewRotationEngaged = false;

        [SerializeField]
        protected InputInvertSettings invertViewRotationVertical;


        // Called when the Back input is pressed.
        protected virtual void Back()
        {
            if (!CanRunInput()) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.ModuleSelection)
            {
                loadoutUIController.EnterVehicleSelection();
            }
            else
            {
                loadoutUIController.MainMenu();
            }
        }


        // Called when the Menu input is pressed.
        public virtual void Menu()
        {
            if (!CanRunInput()) return;

            loadoutUIController.MainMenu();
        }


        // Called to start the game/mission after loadout is completed.
        public virtual void StartAction()
        {
            if (!CanRunInput()) return;

            loadoutUIController.StartMission(0);
        }


        // Called to accept the current action or click on the currently highlighted UI element.
        public virtual void Accept()
        {
            if (!CanRunInput()) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.VehicleSelection)
            {
                loadoutUIController.EnterModuleSelection();
            }
            else
            {
                loadoutUIController.EquipModule();
            }
        }


        // Cycle vehicle selection forward or back.
        protected virtual void CycleVehicleSelection(bool forward)
        {
            if (!CanRunInput()) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.VehicleSelection) loadoutUIController.CycleVehicleSelection(forward);
        }


        // Cycle module selection forward or back.
        protected virtual void CycleModuleSelection(bool forward)
        {
            if (!CanRunInput()) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.ModuleSelection) loadoutUIController.CycleModuleSelection(forward);
        }


        // Cycle module mount selection forward or back.
        protected virtual void CycleModuleMountSelection(bool forward)
        {
            if (!CanRunInput()) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.ModuleSelection) loadoutUIController.CycleModuleMountSelection(forward);
        }


        // Delete the current configuration.
        protected virtual void Delete()
        {
            if (!CanRunInput()) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.ModuleSelection) loadoutUIController.ClearSelectedModuleMount();
        }


        protected virtual InputDeviceType GetViewRotationInputDeviceType()
        {
            return InputDeviceType.None;
        }


        // Called every frame this input script is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            float viewRotationVertical = -viewRotationInputValue.y;

            switch (GetViewRotationInputDeviceType())
            {
                case InputDeviceType.Mouse:

                    if (invertViewRotationVertical.InvertMouse) viewRotationVertical *= -1;

                    break;

                case InputDeviceType.Keyboard:

                    if (invertViewRotationVertical.InvertKeyboard) viewRotationVertical *= -1;

                    break;

                case InputDeviceType.Gamepad:

                    if (invertViewRotationVertical.InvertGamepad) viewRotationVertical *= -1;

                    break;

                case InputDeviceType.Joystick:

                    if (invertViewRotationVertical.InvertJoystick) viewRotationVertical *= -1;

                    break;

            }

            loadoutCameraController.SetViewRotationInputs(new Vector2(viewRotationInputValue.x, viewRotationVertical));         
        }
    }
}
