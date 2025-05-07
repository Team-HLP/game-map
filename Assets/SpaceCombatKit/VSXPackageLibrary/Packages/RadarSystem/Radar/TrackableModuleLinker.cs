using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;

namespace VSX.RadarSystem
{
    public class TrackableModuleLinker : ModuleManager
    {
        [SerializeField]
        protected Trackable parentTrackable;

        [SerializeField]
        protected TrackableType moduleTrackableTypeOverride;

        [SerializeField]
        protected string labelOverride;


        protected override void OnModuleMounted(Module module)
        {
            base.OnModuleMounted(module);

            Trackable moduleTrackable = module.GetComponent<Trackable>();
            if (parentTrackable != null && moduleTrackable != null)
            {
                if (moduleTrackableTypeOverride != null)
                {
                    moduleTrackable.TrackableType = moduleTrackableTypeOverride;
                }

                if (labelOverride != "")
                {
                    moduleTrackable.Label = labelOverride;
                }

                parentTrackable.AddChildTrackable(moduleTrackable);
            }
        }
    }
}

