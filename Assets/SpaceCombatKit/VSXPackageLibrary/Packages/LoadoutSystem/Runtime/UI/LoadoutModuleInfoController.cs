using System.Collections;
using System.Collections.Generic;


namespace VSX.Loadouts
{
    /// <summary>
    /// Display info for a module on the loadout UI.
    /// </summary>
    public class LoadoutModuleInfoController : LoadoutItemInfoController
    {

        /// <summary>
        /// Update the loadout module info.
        /// </summary>
        public override void UpdateInfo()
        {

            base.UpdateInfo();

            if (loadoutManager.Items == null) { Hide(); return; }

            if (loadoutManager.SelectedModuleMountIndex == -1) { Hide(); return; }

            if (loadoutManager.WorkingSlot.selectedModules[loadoutManager.SelectedModuleMountIndex] == -1) { Hide(); return; }

            LoadoutModuleItem moduleItem = loadoutManager.Items.modules[loadoutManager.WorkingSlot.selectedModules[loadoutManager.SelectedModuleMountIndex]];

            SetLabel(moduleItem.Label);
            SetDescription(moduleItem.description);
            if (iconSpriteIndex >= 0 && moduleItem.sprites.Count > iconSpriteIndex) SetIcon(moduleItem.sprites[iconSpriteIndex]);

            foreach (StatController statController in statControllers)
            {
                statController.Set(loadoutManager.Items, loadoutManager.WorkingSlot.selectedModules[loadoutManager.SelectedModuleMountIndex]);
            }
        }
    }
}

