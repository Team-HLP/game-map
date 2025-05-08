using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling triggerables (e.g. weapons) on a vehicle, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_TriggerablesControls : PlayerInput_Base_TriggerablesControls
    {
        [Header("Inputs")]

        [Tooltip("Input for firing the primary weapon.")]
        [SerializeField]
        protected CustomInput primaryWeaponInput = new CustomInput("Weapons", "Fire Primary", 0);

        [Tooltip("Input for firing the secondary weapon.")]
        [SerializeField]
        protected CustomInput secondaryWeaponInput = new CustomInput("Weapons", "Fire Secondary", 1);


        /// <summary>
        /// Stop receivng input.
        /// </summary>
        public override void StopInput()
        {
            base.StopInput();

            if (triggerablesManager != null)
            {
                triggerablesManager.StopTriggeringAll();
            }
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            // Primary

            if (primaryWeaponInput.Down())
            {
                StartFiring(primaryWeaponTriggerIndex);
            }
            else if (primaryWeaponInput.Up())
            {
                StopFiring(primaryWeaponTriggerIndex);
            }

            // Secondary

            if (secondaryWeaponInput.Down())
            {
                StartFiring(secondaryWeaponTriggerIndex);
            }
            else if (secondaryWeaponInput.Up())
            {
                StopFiring(secondaryWeaponTriggerIndex);
            }
        }
    }
}