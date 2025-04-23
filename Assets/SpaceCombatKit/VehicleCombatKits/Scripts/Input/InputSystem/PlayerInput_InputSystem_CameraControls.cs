using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling camera functionality on a vehicle, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_CameraControls : PlayerInput_Base_CameraControls
    {
        
        protected GeneralInputAsset input;


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            input.CameraControls.NextCameraView.performed += ctx => CycleCameraView(true);

            input.CameraControls.PreviousCameraView.performed += ctx => CycleCameraView(false);
        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }
    }
}