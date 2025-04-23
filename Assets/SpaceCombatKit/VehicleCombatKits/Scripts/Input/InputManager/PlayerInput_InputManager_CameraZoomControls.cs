using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling camera zoom controls on a vehicle, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_CameraZoomControls : PlayerInput_Base_CameraZoomControls
    {

        [Tooltip("Input for zooming in and out with the camera.")]
        [SerializeField]
        protected CustomInput zoomInput = new CustomInput("Camera", "Camera zoom.", "Mouse ScrollWheel");


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            zoomInputValue = zoomInput.FloatValue();

            base.OnInputUpdate();
        }
    }
}

