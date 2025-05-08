using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.Characters
{
    /// <summary>
    /// Controls a character interactable that can be toggled to control an animated object such as a door or ramp.
    /// </summary>
    public class ToggleInteractableAnimated : CharacterInteractable
    {

        [Tooltip("The state animator controller that manages the animated object.")]
        [SerializeField]
        protected StateAnimatorController stateAnimatorController;


        /// <summary>
        /// Called when a character interacts with this interactable.
        /// </summary>
        public override void Interact()
        {
            base.Interact();

            if (stateAnimatorController.CurrentState == 0)
            {
                stateAnimatorController.TransitionToState(1);
            }
            else
            {
                stateAnimatorController.TransitionToState(0);
            }
        }
    }
}

