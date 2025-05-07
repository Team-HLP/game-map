using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.ResourceSystem;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Display UI for a resource container found on a module mounted on a module mount.
    /// </summary>
    public class ModuleResourceContainerUIController : ResourceContainerUIController
    {

        [Tooltip("The module mount to display the resource container UI for.")]
        [SerializeField]
        protected ModuleMount moduleMount;


        protected virtual void Awake()
        {
            if (moduleMount != null)
            {
                moduleMount.onModuleMounted.AddListener(OnModuleMounted);
                moduleMount.onModuleUnmounted.AddListener(OnModuleUnmounted);

                if (moduleMount.MountedModule() != null) OnModuleMounted(moduleMount.MountedModule());
            }
        }


        protected virtual void OnModuleMounted(Module module)
        {
            ResourceContainer[] resourceContainers = module.GetComponentsInChildren<ResourceContainer>();
            foreach(ResourceContainer resourceContainer in resourceContainers)
            {
                if (CanDisplay(resourceContainer)) Display(resourceContainer);
            }
        }


        protected virtual void OnModuleUnmounted(Module module)
        {
            Display(null);
        }
    }
}
