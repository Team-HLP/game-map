using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Loadouts;
using VSX.Weapons;


namespace VSX.VehicleCombatKits
{
    public class WeaponRangeStatController : ModuleStatController
    {
        /// <summary>
        /// Get whether an object is relevant to display the stat for.
        /// </summary>
        /// <param name="statTarget">The object to display the stat for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            Weapon weapon = statTarget.GetComponent<Weapon>();
            if (weapon == null) return false;

            return true;
        }


        /// <summary>
        /// Get the float value of the stat for an object.
        /// </summary>
        /// <param name="statTarget">The object to get the stat value for.</param>
        /// <returns>The stat value.</returns>
        protected override float GetStatValue(GameObject statTarget)
        {
            Weapon weapon = statTarget.GetComponent<Weapon>();
            if (weapon == null) return 0f;

            return weapon.Range;
        }
    }
}
