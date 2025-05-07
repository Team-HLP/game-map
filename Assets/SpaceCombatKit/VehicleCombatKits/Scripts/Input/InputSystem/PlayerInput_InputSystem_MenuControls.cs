using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for interacting with a generic menu (e.g. pause menu), using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_MenuControls : PlayerInput_Base_MenuControls
    {
        
        protected GeneralInputAsset input;


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            input.CoreControls.Back.performed += ctx => Back();
            input.GeneralControls.Pause.performed += ctx => ToggleMenu();
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
