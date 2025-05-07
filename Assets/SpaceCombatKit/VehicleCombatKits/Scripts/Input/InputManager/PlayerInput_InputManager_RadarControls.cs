using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Player input script for controlling a vehicle's radar functionality, using Unity's Input Manager.
    /// </summary>
    public class PlayerInput_InputManager_RadarControls : PlayerInput_Base_RadarControls
    {
        [Header("Inputs")]

        [Tooltip("Input for selecting the next target.")]
        [SerializeField]
        protected CustomInput nextTargetInput = new CustomInput("Target Selection", "Next", KeyCode.Greater);

        [Tooltip("Input for selecting the previous target.")]
        [SerializeField]
        protected CustomInput previousTargetInput = new CustomInput("Target Selection", "Previous", KeyCode.Less);

        [Tooltip("Input for selecting the nearest target.")]
        [SerializeField]
        protected CustomInput nearestTargetInput = new CustomInput("Target Selection", "Nearest", KeyCode.N);

        [Tooltip("Input for selecting the front-most target.")]
        [SerializeField]
        protected CustomInput frontTargetInput = new CustomInput("Target Selection", "Front", KeyCode.M);


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {

            base.OnInputUpdate();

            // Select next target
            if (nextTargetInput.Down())
            {
                TargetNext();
            }

            // Select previous target
            if (previousTargetInput.Down())
            {
                TargetPrevious();
            }

            // Select nearest target
            if (nearestTargetInput.Down())
            {
                TargetNearest();
            }

            // Select front target
            if (frontTargetInput.Down())
            {
                TargetFront();
            }
        }
    }
}