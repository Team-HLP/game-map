using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Characters;
using VSX.Loadouts;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Display stat for character walk speed on the UI.
    /// </summary>
    public class WalkSpeedStatController : VehicleStatController
    {
        /// <summary>
        /// Get whether an object is relevant to display the stat for.
        /// </summary>
        /// <param name="statTarget">The object to display the stat for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            RigidbodyCharacterController character = statTarget.GetComponent<RigidbodyCharacterController>();
            if (character == null) return false;

            return true;
        }


        /// <summary>
        /// Get the float value of the stat for an object.
        /// </summary>
        /// <param name="statTarget">The object to get the stat value for.</param>
        /// <returns>The stat value.</returns>
        protected override float GetStatValue(GameObject statTarget)
        {
            RigidbodyCharacterController character = statTarget.GetComponent<RigidbodyCharacterController>();
            if (character == null) return 0f;

            return character.WalkSpeed;
        }
    }
}

