using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;
using VSX.Controls.UnityInputManager;
using VSX.Engines3D;

namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// Player input script for controlling camera functionality on a vehicle, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_CameraControls : PlayerInput_Base_CameraControls
    {
        [Header("Inputs")]

        [Tooltip("Input for cycling the camera view forward.")]
        [SerializeField]
        protected CustomInput cycleViewForwardInput = new CustomInput("Camera", "Next camera view.", KeyCode.RightBracket);

        [Tooltip("Input for cycling the camera view backward.")]
        [SerializeField]
        protected CustomInput cycleViewBackwardInput = new CustomInput("Camera", "Previous camera view.", KeyCode.LeftBracket);

        [Tooltip("Input for selecting specific camera views.")]
        [SerializeField]
        protected List<CameraViewInput> cameraViewInputs = new List<CameraViewInput>();

        protected VehicleEngines3D engines;


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            // Cycle camera view
            if (cameraEntity != null)
            {
                if (cycleViewForwardInput.Down())
                {
                    cameraEntity.CycleCameraView(true);
                }
                else if (cycleViewBackwardInput.Down())
                {
                    cameraEntity.CycleCameraView(false);
                }

                // Select camera view
                for (int i = 0; i < cameraViewInputs.Count; ++i)
                {
                    if (cameraViewInputs[i].input.Down())
                    {
                        cameraEntity.SetView(cameraViewInputs[i].view);
                    }
                }
            }
        }
    }
}