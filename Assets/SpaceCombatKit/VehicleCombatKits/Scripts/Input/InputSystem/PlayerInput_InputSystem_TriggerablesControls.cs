using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling triggerables (e.g. weapons) on a vehicle, using Unity's Input System.
    /// </summary>
    public class PlayerInput_InputSystem_TriggerablesControls : PlayerInput_Base_TriggerablesControls
    {

        protected GeneralInputAsset input;


        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            // Fire primary
            input.WeaponControls.FirePrimary.performed += ctx => StartFiring(primaryWeaponTriggerIndex);
            input.WeaponControls.FirePrimary.canceled += ctx => StopFiring(primaryWeaponTriggerIndex);


            // Fire secondary
            input.WeaponControls.FireSecondary.performed += ctx => StartFiring(secondaryWeaponTriggerIndex);
            input.WeaponControls.FireSecondary.canceled += ctx => StopFiring(secondaryWeaponTriggerIndex);

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