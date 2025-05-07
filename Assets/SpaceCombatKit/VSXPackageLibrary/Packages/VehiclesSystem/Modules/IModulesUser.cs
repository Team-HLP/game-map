using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VSX.Vehicles
{
    public interface IModulesUser
    {
        void OnModuleAdded(Module module);

        void OnModuleRemoved(Module module);

        void OnModuleMounted(Module module);

        void OnModuleUnmounted(Module module);
    }
}

