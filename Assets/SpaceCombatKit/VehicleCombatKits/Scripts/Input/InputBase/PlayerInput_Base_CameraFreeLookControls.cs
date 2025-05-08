using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;
using VSX.GameStates;
using VSX.Utilities;
using VSX.UI;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls camera free look mode on a vehicle.
    /// </summary>
    public class PlayerInput_Base_CameraFreeLookControls : VehicleInput
    {

        [Tooltip("How fast the camera rotates in free look mode.")]
        [SerializeField]
        protected float freeLookSpeed = 0.1f;

        [Tooltip("Whether to invert the vertical axis of the camera rotation in free look mode for each input device.")]
        [SerializeField]
        protected InputInvertSettings invertFreeLookVertical;

        [Tooltip("Whether to move the cursor back to the center of the screen in free look mode.")]
        [SerializeField]
        protected bool centerCursorOnFreeLookMode = true;

        [Tooltip("The camera views in which free look mode can be used.")]
        [SerializeField]
        protected List<CameraView> freeLookModeCameraViews = new List<CameraView>();

        [Tooltip("The game state to enter when free look mode is entered.")]
        [SerializeField]
        protected GameState freeLookEnterGameState;

        [Tooltip("The game state to enter when free look mode is exited.")]
        [SerializeField]
        protected GameState freeLookExitGameState;


        protected bool isFreeLookMode = false;

        protected Vector2 lookInputValue;

        protected CameraTarget cameraTarget;

        protected CameraEntity cameraEntity;

        protected GimbalController cameraGimbalController;

        protected CustomCursor hudCursor;



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            isFreeLookMode = false;
            if (cameraGimbalController != null)
            {
                cameraGimbalController.ResetGimbal(true);
            }

            if (!base.Initialize(vehicle)) return false;

            // Unlink from previous camera target
            if (cameraTarget != null)
            {
                cameraTarget.onCameraEntityTargeting.RemoveListener(OnCameraFollowingVehicle);
            }

            // Link to camera following camera target
            cameraTarget = vehicle.GetComponent<CameraTarget>();

            hudCursor = vehicle.GetComponentInChildren<CustomCursor>();

            if (cameraTarget != null)
            {
                // Link to camera already following
                if (cameraTarget.CameraEntity != null)
                {
                    OnCameraFollowingVehicle(cameraTarget.CameraEntity);
                }

                // Link to any new camera following
                cameraTarget.onCameraEntityTargeting.AddListener(OnCameraFollowingVehicle);
            }

            return true;
        }


        // Called when the camera starts following a vehicle.
        protected virtual void OnCameraFollowingVehicle(CameraEntity cameraEntity)
        {
            ExitFreeLookMode();

            cameraGimbalController = null;

            if (cameraEntity != null)
            {
                cameraEntity.onCameraViewTargetChanged.RemoveListener(OnCameraViewChanged);
            }

            this.cameraEntity = cameraEntity;

            if (cameraEntity != null)
            {
                cameraEntity.onCameraViewTargetChanged.AddListener(OnCameraViewChanged);
                cameraGimbalController = cameraEntity.GetComponent<GimbalController>();
            }
        }


        // Called when the camera view changes.
        protected virtual void OnCameraViewChanged(CameraViewTarget cameraViewTarget)
        {
            ExitFreeLookMode();
        }


        // Called every frame that the camera is in free look mode.
        protected virtual void FreeLookModeUpdate()
        {
            // Free look mode
            if (cameraGimbalController != null)
            {
                float freeLookVertical = lookInputValue.y;

                switch (GetLookInputDeviceType())
                {
                    case InputDeviceType.Mouse:

                        if (invertFreeLookVertical.InvertMouse) freeLookVertical *= -1;

                        break;

                    case InputDeviceType.Keyboard:

                        if (invertFreeLookVertical.InvertKeyboard) freeLookVertical *= -1;

                        break;

                    case InputDeviceType.Gamepad:

                        if (invertFreeLookVertical.InvertGamepad) freeLookVertical *= -1;

                        break;

                    case InputDeviceType.Joystick:

                        if (invertFreeLookVertical.InvertJoystick) freeLookVertical *= -1;

                        break;
                }

                cameraGimbalController.Rotate(lookInputValue.x * freeLookSpeed, -freeLookVertical * freeLookSpeed);
            }
        }


        protected virtual InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.None;
        }


        protected virtual void EnterFreeLookMode()
        {
            if (!CanRunInput()) return;

            if (isFreeLookMode) return;

            if (cameraEntity == null) return;

            if (freeLookModeCameraViews.Count != 0 && freeLookModeCameraViews.IndexOf(cameraEntity.CurrentView) == -1) return;

            isFreeLookMode = true;

            if (centerCursorOnFreeLookMode && hudCursor != null) hudCursor.CenterCursor();

            if (GameStateManager.Instance != null && freeLookEnterGameState != null)
            {
                GameStateManager.Instance.EnterGameState(freeLookEnterGameState);
            }
        }


        protected virtual void ExitFreeLookMode()
        {
            if (!CanRunInput()) return;

            if (!isFreeLookMode) return;

            isFreeLookMode = false;
            if (cameraGimbalController != null)
            {
                cameraGimbalController.ResetGimbal(true);
            }

            if (GameStateManager.Instance != null && freeLookExitGameState != null)
            {
                GameStateManager.Instance.EnterGameState(freeLookExitGameState);
            }
        }


        // Called every frame that this input script is running.
        protected override void OnInputUpdate()
        {
            if (isFreeLookMode)
            {
                FreeLookModeUpdate();
            }
        }
    }
}