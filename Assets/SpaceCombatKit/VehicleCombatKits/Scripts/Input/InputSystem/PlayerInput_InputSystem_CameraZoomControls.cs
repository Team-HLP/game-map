using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling camera zoom controls on a vehicle, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_CameraZoomControls : PlayerInput_Base_CameraZoomControls
    {

        protected GeneralInputAsset input;


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }


        protected override void Awake()
        {

            base.Awake();

            input = new GeneralInputAsset();

            input.CameraControls.Zoom.performed += ctx => Zoom(ctx.ReadValue<float>());

        }


        protected virtual void Zoom(float zoom)
        {
            zoomInputValue = zoom;
        }
    }
}

