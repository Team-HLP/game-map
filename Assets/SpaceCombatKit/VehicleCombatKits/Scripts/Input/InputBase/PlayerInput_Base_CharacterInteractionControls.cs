using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;
using VSX.Vehicles;

namespace VSX.Characters
{
    /// <summary>
    /// Base class for a player input script that controls character interactions.
    /// </summary>
    public class PlayerInput_Base_CharacterInteractionControls : VehicleInput
    {

        [Tooltip("The game agent that controls the character that this input is for.")]
        [SerializeField]
        protected GameAgent gameAgent;
        
        [Tooltip("The interaction UI prompt handle.")]
        [SerializeField]
        protected GameObject promptHandle;

        [Tooltip("The interaction prompt text.")]
        [SerializeField]
        protected UVCText promptText;

        [Tooltip("The interaction prompt contents.")]
        [SerializeField]
        protected string prompt = "Press {control} To {action}";

        protected Vehicle vehicle;

        protected bool interactionsPaused = false;



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

            this.vehicle = vehicle;

            return true;
        }


        // Called every frame that this input script is running.
        protected override void OnInputUpdate()
        {
            if (gameAgent.Character == null) return;

            if (promptHandle != null) promptHandle.SetActive(false);

            if (!interactionsPaused)
            {
                if (gameAgent.Character.Interactable != null)
                {
                    if (promptText != null)
                    {
                        promptText.text = prompt;
                        promptText.text = promptText.text.Replace("{control}", GetControlDisplayString());
                        promptText.text = promptText.text.Replace("{action}", gameAgent.Character.Interactable.PromptText);
                    }

                    if (promptHandle != null) promptHandle.SetActive(true);
                }
            }
        }


        protected virtual void Interact()
        {
            if (!initialized) return;
            if (interactionsPaused) return;

            if (vehicle == gameAgent.Character)
            {
                if (gameAgent.Character.Interactable != null)
                {

                    gameAgent.Character.Interact();
                    PauseInteractions(1);
                }
            }
        }


        // Pause interactions for a specified duration.
        protected virtual void PauseInteractions(float duration)
        {
            StartCoroutine(PauseInteractionsCoroutine(duration));
        }


        // Get the string for the enter/exit control on the UI.
        protected virtual string GetControlDisplayString()
        {
            return "";
        }



        protected IEnumerator PauseInteractionsCoroutine(float duration)
        {
            interactionsPaused = true;
            yield return new WaitForSeconds(duration);
            interactionsPaused = false;
        }
    }
}

