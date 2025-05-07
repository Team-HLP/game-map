using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.Loadouts
{
    /// <summary>
    /// Displays a stat on the UI for a module.
    /// </summary>
    public class ModuleStatController : StatController
    {
        [Tooltip("The module types that are compatible with this stat. If empty, all module types are compatible.")]
        [SerializeField]
        protected List<ModuleType> moduleTypes = new List<ModuleType>();


        /// <summary>
        /// Get whether an object is relevant to display stats for.
        /// </summary>
        /// <param name="statTarget">The object to display stats for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public override bool IsCompatible(GameObject statTarget)
        {
            if (!base.IsCompatible(statTarget)) return false;

            Module module = statTarget.GetComponent<Module>();
            if (module == null) return false;

            if (moduleTypes.Count > 0 && moduleTypes.IndexOf(module.ModuleType) == -1) return false;

            return true;
        }


        /// <summary>
        /// Provide the stat controller with relevant loadout information to use when displaying the stat.
        /// </summary>
        /// <param name="loadoutItems">The loadout items.</param>
        /// <param name="itemIndex">The list index of the loadout item whose stats are being displayed.</param>
        public override void Set(LoadoutItems loadoutItems, int itemIndex)
        {
            Set(loadoutItems.modules.ConvertAll(x => x.modulePrefab.gameObject), itemIndex);
        }
    }
}

