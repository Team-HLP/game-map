using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.CameraSystem;
using VSX.Vehicles;

namespace VSX.VehicleCombatKits
{

    /// <summary>
    /// A camera target represents an object (such as a character or a vehicle) that a camera entity can follow.
    /// </summary>
    public class UVCCameraTarget : CameraTarget, IModulesUser
    {
        public virtual void OnModuleAdded(Module module) { }

        public virtual void OnModuleRemoved(Module module) { }

        public virtual void OnModuleMounted(Module module)
        {            
            ICameraEntityUser[] moduleCameraEntityUsers = module.GetComponentsInChildren<ICameraEntityUser>();

            foreach(ICameraEntityUser moduleCameraEntityUser in moduleCameraEntityUsers)
            {
                // Check if it's already stored
                bool found = false;
                foreach(ICameraEntityUser storedCameraEntityUser in cameraEntityUsers)
                {
                    if (storedCameraEntityUser == moduleCameraEntityUser)
                    {
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                cameraEntityUsers.Add(moduleCameraEntityUser);

                moduleCameraEntityUser.SetCameraEntity(cameraEntity);
            }
        }

        public virtual void OnModuleUnmounted(Module module)
        {
            ICameraEntityUser[] moduleCameraEntityUsers = module.GetComponentsInChildren<ICameraEntityUser>();

            foreach (ICameraEntityUser moduleCameraEntityUser in moduleCameraEntityUsers)
            {
                foreach (ICameraEntityUser storedCameraEntityUser in cameraEntityUsers)
                {
                    if (storedCameraEntityUser.Equals(moduleCameraEntityUser))
                    {
                        cameraEntityUsers.Remove(storedCameraEntityUser);

                        moduleCameraEntityUser.SetCameraEntity(cameraEntity);

                        break;
                    }
                }
            }
        }
    }
}

