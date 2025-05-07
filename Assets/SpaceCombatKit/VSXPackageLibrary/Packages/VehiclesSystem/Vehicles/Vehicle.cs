using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VSX.Vehicles
{

    /// <summary>
    /// Unity event for running functions when the vehicle is entered by a game agent.
    /// </summary>
    [System.Serializable]
    public class OnVehicleEnteredEventHandler : UnityEvent <GameAgent> { };

    /// <summary>
    /// Unity event for running functions when the vehicle is exited by a game agent.
    /// </summary>
    [System.Serializable]
    public class OnVehicleExitedEventHandler : UnityEvent <GameAgent> { };


    /// <summary> 
    /// This class is a base class for all kinds of vehicles. It exposes a function for entering and exiting 
    /// the vehicle, and deals with all kinds of vehicle events.
    /// </summary>
    public class Vehicle : MonoBehaviour
	{

        [Header("General")]

        // The identifying label for this vehicle, used by the loadout menu etc. 
        [SerializeField]
        protected string label = "Vehicle";
		public virtual string Label { get { return label; } }

        // The identifying label for this vehicle, used by the loadout menu etc. 
        [TextArea]
        [SerializeField]
        protected string description = "Vehicle.";
        public virtual string Description { get { return description; } }

        // The class of vehicle
        [SerializeField]
        protected VehicleClass vehicleClass;
        public VehicleClass VehicleClass { get { return vehicleClass; } }

        [Tooltip("A box that represents the height, length, and width of the vehicle, used for target box sizing, loadout zoom, etc.")]
        [SerializeField]
        protected Bounds bounds;
        public Bounds Bounds { get { return bounds; } }

        // A list of all the occupants currently in the vehicle
        protected List<GameAgent> occupants = new List<GameAgent>();
        public List<GameAgent> Occupants { get { return occupants; } }

        // Efficiently get the game object
        protected GameObject cachedGameObject;
        public GameObject CachedGameObject { get { return cachedGameObject; } }

        // Efficiently get the rigidbody
        protected Rigidbody cachedRigidbody;
        public Rigidbody CachedRigidbody { get { return cachedRigidbody; } }

        protected List<ModuleMount> moduleMounts = new List<ModuleMount>();
        public List<ModuleMount> ModuleMounts
        {
            get
            {
                // If prefab, search the hierarchy
                if (gameObject.scene.rootCount == 0)
                {
                    return GetModuleMounts();
                }
                // If not prefab, use cached list
                else
                {
                    return moduleMounts;
                }
            }
        }

        protected List<ModuleManager> moduleManagers = new List<ModuleManager>();

        protected bool destroyed;
        public bool Destroyed { get { return destroyed; } }

        
        [SerializeField]
        protected Transform characterExitSpawn;
        public Transform CharacterExitSpawn { get { return characterExitSpawn; } }


        [Tooltip("Whether to restore the vehicle when it is enabled in the scene (if it was destroyed).")]
        [SerializeField]
        protected bool restoreOnEnable = true;
        public bool RestoreOnEnable { get => restoreOnEnable; set => restoreOnEnable = value; }


        [Header("Vehicle State Events")]
        
        // Vehicle destroyed event
        public UnityEvent onDestroyed;

        // Vehicle restored event
        public UnityEvent onRestored;

        [Header("Vehicle Owner Events")]

        public UnityEvent onEnteredByPlayer;

        public UnityEvent onEnteredByAI;

        public UnityEvent onExitedAll;

        // Game agent entered event
        [HideInInspector]
        public OnVehicleEnteredEventHandler onGameAgentEntered;

        // Game Agent exited event
        [HideInInspector]
        public OnVehicleEnteredEventHandler onGameAgentExited;

        protected List<IModulesUser> modulesUsers = new List<IModulesUser>();



        // Called when the component is first added to a gameobject or is reset in the inspector
        protected virtual void Reset()
        {
            label = "Vehicle";

            bounds = new Bounds(Vector3.zero, new Vector3(30, 30, 30));
        }


        protected virtual void Awake()
		{		
            cachedGameObject = gameObject;
            cachedRigidbody = GetComponent<Rigidbody>();

            moduleManagers = new List<ModuleManager>(GetComponentsInChildren<ModuleManager>());

            List<ModuleMount> sortedModuleMountsList = GetModuleMounts();
            foreach (ModuleMount moduleMount in sortedModuleMountsList)
            {
                AddModuleMount(moduleMount);
            }
        }


        protected virtual void OnEnable()
        {
            if (restoreOnEnable && destroyed)
            {
                Restore();
            }
        }


        protected virtual void OnModuleAdded(Module module)
        {
            if (occupants.Count != 0)
            {
                module.SetOwner(occupants[0]);
            }

            for(int i = 0; i < modulesUsers.Count; ++i)
            {
                modulesUsers[i].OnModuleAdded(module);
            }
        }


        protected virtual void OnModuleRemoved(Module module)
        {
            if (occupants.Count != 0)
            {
                module.SetOwner(null);
            }

            for (int i = 0; i < modulesUsers.Count; ++i)
            {
                modulesUsers[i].OnModuleRemoved(module);
            }
        }


        protected virtual void OnModuleMounted(Module module)
        {
            for (int i = 0; i < modulesUsers.Count; ++i)
            {
                modulesUsers[i].OnModuleMounted(module);
            }
        }


        protected virtual void OnModuleUnmounted(Module module)
        {
            for (int i = 0; i < modulesUsers.Count; ++i)
            {
                modulesUsers[i].OnModuleUnmounted(module);
            }
        }


        protected virtual List<ModuleMount> GetModuleMounts()
        {
            List<ModuleMount> _moduleMounts = new List<ModuleMount>();

            GetModuleMountsRecursive(transform, ref _moduleMounts);

            return _moduleMounts;
        }


        protected virtual void GetModuleMountsRecursive(Transform t, ref List<ModuleMount> moduleMountsList)
        {
            ModuleMount[] moduleMountsOnTransform = t.GetComponents<ModuleMount>();
            foreach (ModuleMount moduleMount in moduleMountsOnTransform)
            {
                moduleMountsList.Add(moduleMount);
            }

            foreach(Transform child in t)
            {
                GetModuleMountsRecursive(child, ref moduleMountsList);
            }
        }


        /// <summary>
        /// Add a module mount to the vehicle.
        /// </summary>
        /// <param name="moduleMount">The module mount to be added.</param>
        public virtual void AddModuleMount(ModuleMount moduleMount)
        {

            moduleMounts.Add(moduleMount);

            moduleMount.RootTransform = transform;

            for (int i = 0; i < moduleManagers.Count; ++i)
            {
                moduleManagers[i].OnModuleMountAdded(moduleMount);
            }

            moduleMount.onModuleAdded.AddListener(OnModuleAdded);
            moduleMount.onModuleRemoved.AddListener(OnModuleRemoved);
            moduleMount.onModuleMounted.AddListener(OnModuleMounted);
            moduleMount.onModuleUnmounted.AddListener(OnModuleUnmounted);

        }


        /// <summary>
        /// Remove a module mount from the vehicle.
        /// </summary>
        /// <param name="moduleMount">The module mount to be removed.</param>
        public virtual void RemoveModuleMount(ModuleMount moduleMount)
        {

            int index = moduleMounts.IndexOf(moduleMount);
            if (index == -1) return;

            moduleMounts.Remove(moduleMount);

            for (int i = 0; i < moduleManagers.Count; ++i)
            {
                moduleManagers[i].OnModuleMountRemoved(moduleMount);
            }

            moduleMount.onModuleAdded.RemoveListener(OnModuleAdded);
            moduleMount.onModuleRemoved.RemoveListener(OnModuleRemoved);
            moduleMount.onModuleMounted.RemoveListener(OnModuleMounted);
            moduleMount.onModuleUnmounted.RemoveListener(OnModuleUnmounted);

        }

        
        /// <summary>
        /// Called when a game agent enters the vehicle.
        /// </summary>
        /// <param name="newOccupant">The game agent that entered the vehicle.</param>
        public virtual void OnEntered (GameAgent newOccupant)
        {
            if (newOccupant == null) return;

            // Check if the game agent is already in the vehicle
            for (int i = 0; i < occupants.Count; ++i)
            {
                if (occupants[i] == newOccupant)
                {
                    return;
                }
            }

            // Add the new occupant
            occupants.Add(newOccupant);

            if (occupants.Count != 0)
            {
                for (int i = 0; i < moduleManagers.Count; ++i)
                {
                    moduleManagers[i].ActivateModuleManager();
                }
            }

            // Update owner for modules
            for(int i = 0; i < moduleMounts.Count; ++i)
            {
                for(int j = 0; j < moduleMounts[i].Modules.Count; ++j)
                {
                    moduleMounts[i].Modules[j].SetOwner(newOccupant);
                }
            }

            // Call the events
            onGameAgentEntered.Invoke(newOccupant);
            if (newOccupant.IsPlayer)
            {
                onEnteredByPlayer.Invoke();
            }
            else
            {
                onEnteredByAI.Invoke();
            }
        }


        /// <summary>
        /// Called when a game agent exits a vehicle.
        /// </summary>
        /// <param name="exitingOccupant">The game agent exiting.</param>
        public virtual void OnExited (GameAgent exitingOccupant)
        {

            if (exitingOccupant == null) return;

            // Find the occupant in the list and remove
            for (int i = 0; i < occupants.Count; ++i)
            {
                if (occupants[i] == exitingOccupant)
                {
                    // Remove the occupant
                    occupants.RemoveAt(i);

                    // Call the event
                    onGameAgentExited.Invoke(exitingOccupant);
                    if (occupants.Count == 0)
                    {
                        onExitedAll.Invoke();
                    }

                    break;
                }
            }

            if (occupants.Count == 0)
            {
                for(int i = 0; i < moduleManagers.Count; ++i)
                {
                    moduleManagers[i].DeactivateModuleManager();
                }

                // Set owner to null for modules
                for (int i = 0; i < moduleMounts.Count; ++i)
                {
                    for (int j = 0; j < moduleMounts[i].Modules.Count; ++j)
                    {
                        moduleMounts[i].Modules[j].SetOwner(null);
                    }
                }
            }
        }


        /// <summary>
        /// Called to destroy the vehicle (e.g. when health reaches zero).
        /// </summary>
        public virtual void Destroy()
        {
            if (!destroyed)
            {
                destroyed = true;

                // Call event
                onDestroyed.Invoke();
            }          
        }


        /// <summary>
        /// Restore the vehicle after it has been destroyed.
        /// </summary>
        public virtual void Restore()
		{
            if (destroyed)
            {
                destroyed = false;

                // Call event
                onRestored.Invoke();
            }         
		}


        // Show the bounding box visually in the editor scene view
        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(bounds.center), transform.rotation, transform.lossyScale);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, bounds.size);
        }
    }
}
