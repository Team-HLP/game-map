using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling a vehicle's radar functionality, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_RadarControls : PlayerInput_Base_RadarControls
    {

        protected GeneralInputAsset input;


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            // Link input to functions
            input.TargetingControls.TargetNext.performed += ctx => TargetNext();
            input.TargetingControls.TargetPrevious.performed += ctx => TargetPrevious();
            input.TargetingControls.TargetNearest.performed += ctx => TargetNearest();
            input.TargetingControls.TargetFront.performed += ctx => TargetFront();
            input.TargetingControls.TargetUnderCursor.performed += ctx => TargetUnderCursor();
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

