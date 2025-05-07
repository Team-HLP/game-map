using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling camera free look mode on a vehicle, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_CameraFreeLookControls : PlayerInput_Base_CameraFreeLookControls
    {

        [Tooltip("Input for toggling Free Look Mode.")]
        [SerializeField]
        protected CustomInput freeLookModeInput = new CustomInput("Camera", "Free look mode.", 2);

        [Tooltip("Input for rotating the camera horizontally in Free Look Mode.")]
        [SerializeField]
        protected CustomInput lookHorizontalInput = new CustomInput("Camera", "Free look horizontal.", "Mouse X");

        [Tooltip("Input for rotating the camera vertically in Free Look Mode.")]
        [SerializeField]
        protected CustomInput lookVerticalInput = new CustomInput("Camera", "Free look vertical.", "Mouse Y");



        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            if (freeLookModeInput.Down())
            {
                EnterFreeLookMode();
            }

            if (freeLookModeInput.Up())
            {
                ExitFreeLookMode();
            }

            if (isFreeLookMode && cameraGimbalController != null)
            {
                lookInputValue = new Vector2(lookHorizontalInput.FloatValue(), lookVerticalInput.FloatValue());
            }
        }


        protected override InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.Mouse;
        }
    }
}
