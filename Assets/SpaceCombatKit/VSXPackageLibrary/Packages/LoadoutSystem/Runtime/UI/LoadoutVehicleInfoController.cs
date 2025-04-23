using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;


namespace VSX.Loadouts
{
    /// <summary>
    /// Display info for a vehicle on the loadout UI.
    /// </summary>
    public class LoadoutVehicleInfoController : LoadoutItemInfoController
    {

        /// <summary>
        /// Update the loadout vehicle info.
        /// </summary>
        public override void UpdateInfo()
        {
            base.UpdateInfo();

            if (loadoutManager.Items == null) { Hide(); return; }

            if (loadoutManager.WorkingSlot.selectedVehicleIndex == -1) { Hide(); return; }

            LoadoutVehicleItem vehicleItem = loadoutManager.Items.vehicles[loadoutManager.WorkingSlot.selectedVehicleIndex];

            SetLabel(vehicleItem.Label + (vehicleItem.locked ? " (LOCKED)" : ""));
            SetDescription(vehicleItem.description);
            if (iconSpriteIndex >= 0 && vehicleItem.sprites.Count > iconSpriteIndex) SetIcon(vehicleItem.sprites[iconSpriteIndex]);

            foreach (StatController statController in statControllers)
            {
                statController.Set(loadoutManager.Items, loadoutManager.WorkingSlot.selectedVehicleIndex);
            }
        }
    }
}

