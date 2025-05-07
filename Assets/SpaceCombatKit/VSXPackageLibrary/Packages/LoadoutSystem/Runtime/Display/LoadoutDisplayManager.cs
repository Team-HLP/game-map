using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.Pooling;
using VSX.Vehicles;
using VSX.Engines3D;

namespace VSX.Loadouts
{
    /// <summary>
    /// This class manages the display of real vehicle and modules in the loadout menu.
    /// </summary>
	public class LoadoutDisplayManager : MonoBehaviour
    {

        [Tooltip("The loadout manager.")]
        [SerializeField]
        protected LoadoutManager loadoutManager;

        [Tooltip("The transform that display vehicles are parented to.")]
        [SerializeField]
        protected Transform vehicleHolder;
        public Transform VehicleHolder { get { return vehicleHolder; } }

        [Tooltip("Whether the vehicle should be positioned such that its bounds center point (rather than its transform position) is at the same position as the Vehicle Holder transform.")]
        [SerializeField]
        protected bool positionVehicleAtBoundsCenter = true;

        // The list of added display vehicles
        protected List<Vehicle> displayVehicles = new List<Vehicle>();
        public virtual List<Vehicle> DisplayVehicles { get { return displayVehicles; } }

        protected List<Module> displayModules = new List<Module>();

        
        protected virtual void Reset()
        {
            loadoutManager = FindAnyObjectByType<LoadoutManager>();
            vehicleHolder = transform;
        }


        protected virtual void Awake()
        {
            loadoutManager.onDataLoad.AddListener(AddDisplayVehicles);
            loadoutManager.onLoadoutChanged.AddListener(ShowVehicle);
        }


        protected virtual void AddDisplayVehicles()
        {

            RemoveDisplayVehicles();

            if (loadoutManager.Items == null)
            {
                return;
            }

            List<LoadoutVehicleItem> vehicleItems = loadoutManager.Items.vehicles;

            foreach (LoadoutVehicleItem vehicleItem in vehicleItems)
            {
                Vehicle vehicle = GetDisplayVehicle(vehicleItem.vehiclePrefab);

                vehicle.gameObject.SetActive(false);
            }
        }


        protected virtual void RemoveModules(Vehicle vehicle)
        {
            foreach (ModuleMount moduleMount in vehicle.ModuleMounts)
            {
                moduleMount.RemoveAllModules();
            }

            for (int i = 0; i < displayModules.Count; ++i)
            {
                if (PoolManager.Instance != null)
                {
                    displayModules[i].gameObject.SetActive(false);
                }
                else
                {
                    Destroy(displayModules[i].gameObject);
                }

                displayModules.RemoveAt(i);
                i--;
            }
        }


        protected virtual void RemoveDisplayVehicles()
        {
            Vehicle[] displayVehiclesArray = displayVehicles.ToArray();

            foreach (Vehicle displayVehicle in displayVehiclesArray)
            {
                RemoveModules(displayVehicle);

                displayVehicles.Remove(displayVehicle);

                if (PoolManager.Instance != null)
                {
                    displayVehicle.transform.SetParent(null);
                    displayVehicle.gameObject.SetActive(false);
                }
                else
                {
                    Destroy(displayVehicle.gameObject);
                }
            }
        }


        protected virtual Vehicle GetDisplayVehicle(Vehicle vehiclePrefab)
        {
            Vehicle vehicle;

            if (PoolManager.Instance != null)
            {
                vehicle = PoolManager.Instance.Get(vehiclePrefab.gameObject, vehicleHolder).GetComponent<Vehicle>();
            }
            else
            {
                vehicle = Instantiate(vehiclePrefab, vehicleHolder.position, vehicleHolder.rotation, vehicleHolder);
            }

            if (positionVehicleAtBoundsCenter)
            {
                
                vehicle.transform.localPosition = -vehicle.Bounds.center;
            }

            // Make rigidbody kinematic

            Rigidbody r = vehicle.GetComponent<Rigidbody>();
            if (r != null) r.isKinematic = true;

            VehicleEngines3D engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines != null)
            {
                engines.ActivateEnginesAtStart = false;
                engines.SetEngineActivation(false);
            }

            // Prevent creation of default modules

            foreach (ModuleMount moduleMount in vehicle.ModuleMounts)
            {
                moduleMount.createDefaultModulesAtStart = false;
            }

            // Add to list

            displayVehicles.Add(vehicle);

            return vehicle;
        }


        protected virtual Module GetModule(Module modulePrefab)
        {
            if (PoolManager.Instance != null)
            {
                return PoolManager.Instance.Get(modulePrefab.gameObject, vehicleHolder).GetComponent<Module>();
            }
            else
            {
                Module module = Instantiate(modulePrefab, vehicleHolder.position, vehicleHolder.rotation);
                return module;
            }
        }


        protected virtual void ShowVehicle()
        {           
            if (loadoutManager.Items == null) return;

            int vehicleIndex = loadoutManager.WorkingSlot.selectedVehicleIndex;


            if (vehicleIndex == -1 || !displayVehicles[vehicleIndex].gameObject.activeSelf)
            {
                for (int i = 0; i < displayVehicles.Count; ++i)
                {
                    displayVehicles[i].gameObject.SetActive(false);
                }
            }

            List<LoadoutVehicleItem> vehicleItems = loadoutManager.Items.vehicles;
            List<LoadoutModuleItem> moduleItems = loadoutManager.Items.modules;

            if (vehicleIndex < 0 || vehicleIndex >= vehicleItems.Count) return;

            displayVehicles[vehicleIndex].gameObject.SetActive(true);

            // Remove modules

            RemoveModules(displayVehicles[vehicleIndex]);

            // Add modules
            for (int i = 0; i < displayVehicles[vehicleIndex].ModuleMounts.Count; ++i)
            {
                if (loadoutManager.WorkingSlot.selectedModules.Count <= i) break;

                int moduleIndex = loadoutManager.WorkingSlot.selectedModules[i];
                if (moduleIndex != -1)
                {
                    if (displayVehicles[vehicleIndex].ModuleMounts[i].IsCompatible(moduleItems[moduleIndex].modulePrefab))
                    {
                        Module module = GetModule(moduleItems[moduleIndex].modulePrefab);
                        displayVehicles[vehicleIndex].ModuleMounts[i].AddModule(module, true);
                        displayModules.Add(module);
                    }
                    else
                    {
                        Debug.LogWarning(moduleItems[moduleIndex].modulePrefab.name + " Module is not compatible with the " +
                                            displayVehicles[vehicleIndex].ModuleMounts[i].name + " Module Mount on the " + displayVehicles[vehicleIndex].name + " vehicle.");
                    }
                }
                else
                {
                    displayVehicles[vehicleIndex].ModuleMounts[i].UnmountActiveModule();
                }
            }
        }
    }
}
