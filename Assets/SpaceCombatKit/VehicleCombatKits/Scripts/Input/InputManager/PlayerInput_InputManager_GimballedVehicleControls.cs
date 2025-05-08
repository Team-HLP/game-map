using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling a vehicle with a main gimbal (e.g. turret), using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_GimballedVehicleControls : PlayerInput_Base_GimballedVehicleControls
    {
        [Header("Inputs")]

        [Tooltip("Input for rotating the vehicle's gimbal horizontally.")]
        [SerializeField]
        protected CustomInput horizontalRotationInputAxis = new CustomInput("Turrets", "Look Horizontal", "Mouse X");

        [Tooltip("Input for rotating the vehicle's gimbal vertically.")]
        [SerializeField]
        protected CustomInput verticalRotationInputAxis = new CustomInput("Turrets", "Look Vertical", "Mouse Y");


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.Mouse;
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            lookInputValue.x = horizontalRotationInputAxis.FloatValue();
            lookInputValue.y = verticalRotationInputAxis.FloatValue();
        }
    }
}