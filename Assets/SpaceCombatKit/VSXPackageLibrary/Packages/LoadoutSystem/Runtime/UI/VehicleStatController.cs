using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.Loadouts
{
    /// <summary>
    /// Displays a stat on the UI for a vehicle.
    /// </summary>
    public class VehicleStatController : StatController
    {
        [Tooltip("The vehicle classes that are compatible with this stat. If empty, all vehicle classes are compatible.")]
        [SerializeField]
        protected List<VehicleClass> vehicleClasses = new List<VehicleClass>();


        /// <summary>
        /// Get whether an object is relevant to display stats for.
        /// </summary>
        /// <param name="statTarget">The object to display stats for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            Vehicle vehicle = statTarget.GetComponent<Vehicle>();
            if (vehicle == null) return false;

            if (vehicleClasses.Count > 0 && vehicleClasses.IndexOf(vehicle.VehicleClass) == -1) return false;

            return true;
        }


        /// <summary>
        /// Provide the stat controller with relevant loadout information to use when displaying the stat.
        /// </summary>
        /// <param name="loadoutItems">The loadout items.</param>
        /// <param name="itemIndex">The list index of the loadout item whose stats are being displayed.</param>
        public override void Set(LoadoutItems loadoutItems, int itemIndex)
        {
            Set(loadoutItems.vehicles.ConvertAll(x => x.vehiclePrefab.gameObject), itemIndex);
        }
    }
}

