using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Vehicles;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that controls characters entering and exiting vehicles.
    /// </summary>
    public class PlayerInput_Base_EnterExitControls : VehicleInput
    {

        [Tooltip("The game agent that is controlling the characters/vehicles involved in the enter/exit interaction.")]
        [SerializeField]
        protected GameAgent gameAgent;

        [Tooltip("Whether to prioritize exiting when both entering and exiting is possible in the same frame.")]
        [SerializeField]
        protected bool prioritizeExiting = true;

        [Tooltip("Whether this script should set the enter/exit prompts on the vehicle HUD.")]
        [SerializeField]
        protected bool setEnterExitPrompts = true;

        [Tooltip("The enter prompt (note that {control} is automatically replaced with the control currently bound to the enter/exit action).")]
        [SerializeField]
        protected string enterVehiclePrompt = "Press {control} to enter";

        [Tooltip("The exit prompt (note that {control} is automatically replaced with the control currently bound to the enter/exit action).")]
        [SerializeField]
        protected string exitVehiclePrompt = "Press {control} to exit";

        // The enter exit manager for the vehicle the player is currently in.
        protected VehicleEnterExitManager vehicleEnterExitManager;


        protected override void Reset()
        {
            base.Reset();

            gameAgent = transform.root.GetComponentInChildren<GameAgent>();
        }


        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            // Update the dependencies
            vehicleEnterExitManager = vehicle.GetComponentInChildren<VehicleEnterExitManager>();
            if (vehicleEnterExitManager == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + vehicleEnterExitManager.GetType().Name + " component was not found on the vehicle.");
                }
                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " successfully initialized.");
            }

            return true;

        }


        protected override void OnInitialized(GameObject targetObject)
        {
            base.OnInitialized(targetObject);

            if (gameAgent.Character != null && targetObject != gameAgent.Character.gameObject)
            {
                vehicleEnterExitManager.SetChild(gameAgent.Character.GetComponent<VehicleEnterExitManager>());
            }
        }


        // Called when the enter/exit input is pressed.
        protected void EnterExit()
        {
            if (!CanRunInput()) return;

            if (setEnterExitPrompts)
            {
                vehicleEnterExitManager.SetPrompts(enterVehiclePrompt.Replace("{control}", GetControlDisplayString()),
                                                    exitVehiclePrompt.Replace("{control}", GetControlDisplayString()));
            }

            if (prioritizeExiting)
            {
                if (vehicleEnterExitManager.CanExitToChild())
                {
                    Vehicle child = vehicleEnterExitManager.Child.Vehicle;
                    vehicleEnterExitManager.ExitToChild();
                    gameAgent.EnterVehicle(child);
                }
                else if (vehicleEnterExitManager.EnterableVehicles.Count > 0)
                {
                    Vehicle parent = vehicleEnterExitManager.EnterableVehicles[0].Vehicle;
                    vehicleEnterExitManager.EnterParent(0);
                    gameAgent.EnterVehicle(parent);
                }
            }
            else
            {
                if (vehicleEnterExitManager.EnterableVehicles.Count > 0)
                {
                    // Check for input
                    Vehicle parent = vehicleEnterExitManager.EnterableVehicles[0].Vehicle;
                    vehicleEnterExitManager.EnterParent(0);
                    gameAgent.EnterVehicle(parent);
                }
                else if (vehicleEnterExitManager.CanExitToChild())
                {
                    // Check for input
                    Vehicle child = vehicleEnterExitManager.Child.Vehicle;
                    vehicleEnterExitManager.ExitToChild();
                    gameAgent.EnterVehicle(child);
                }
            }
        }


        // Get the string for the enter/exit control on the UI.
        protected virtual string GetControlDisplayString()
        {
            return "";
        }


        // Called every frame that this input script is running.
        protected override void OnInputUpdate()
        {
            if (setEnterExitPrompts)
            {
                vehicleEnterExitManager.SetPrompts(enterVehiclePrompt.Replace("{control}", GetControlDisplayString()),
                                                    exitVehiclePrompt.Replace("{control}", GetControlDisplayString()));
            }
        }
    }
}