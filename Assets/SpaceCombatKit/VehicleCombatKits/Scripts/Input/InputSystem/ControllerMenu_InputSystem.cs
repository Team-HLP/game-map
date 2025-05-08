using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using VSX.UI;
using VSX.VehicleCombatKits;
using VSX.Vehicles;


namespace VSX.Controls.UnityInputSystem
{
    /// <summary>
    /// Displays a controls menu for a gamepad or joystick.
    /// </summary>
    public class ControllerMenu_InputSystem : MonoBehaviour
    {
        [Tooltip("The input action assets to get bindings from.")]
        [SerializeField]
        protected List<InputActionAsset> inputActionAssets = new List<InputActionAsset>();

        [Tooltip("The input groups (control schemes) to display.")]
        [SerializeField]
        protected List<string> inputGroups = new List<string>();

        [Tooltip("The input maps associated with different vehicle classes.")]
        [SerializeField]
        protected List<VehicleInputDisplaySettings> vehicleInputDisplaySettings = new List<VehicleInputDisplaySettings>();

        // The UI display items in the menu 
        protected List<InputControlDisplayItem> controlDisplayItems = new List<InputControlDisplayItem>();

        [Tooltip("The toggle button prefab for displaying controls for a vehicle class/type.")]
        [SerializeField]
        protected ButtonController vehicleClassMenuToggle;

        [Tooltip("The parent for the toggle buttons for selecting a vehicle class to display controls for.")]
        [SerializeField]
        protected Transform vehicleClassMenuItemsParent;
        protected List<ButtonController> vehicleClassButtons = new List<ButtonController>();

        [Tooltip("Here you can override the suffixes for displaying different axes of an action.")]
        [SerializeField]
        protected List<InputActionAxisLabels> actionAxisLabelOverrides = new List<InputActionAxisLabels>();



        protected virtual void Awake()
        {
            controlDisplayItems = new List<InputControlDisplayItem>(GetComponentsInChildren<InputControlDisplayItem>(true));

            foreach (VehicleInputDisplaySettings vehicleInputMapsItem in vehicleInputDisplaySettings)
            {
                ButtonController vehicleClassButton = Instantiate(vehicleClassMenuToggle, vehicleClassMenuItemsParent);
                vehicleClassButton.GetComponentInChildren<UVCText>().text = AddSpacesToCamelCase(vehicleInputMapsItem.vehicleClass.name);
                vehicleClassButton.onClick.AddListener(() => { Display(vehicleInputMapsItem.vehicleClass); });
                vehicleClassButtons.Add(vehicleClassButton);
            }
        }


        protected virtual void Start()
        {
            Initialize();
        }


        /// <summary>
        /// Initialize the menu to display input for the first vehicle class it has information for.
        /// </summary>
        public virtual void Initialize()
        {
            
            // Display the input for the vehicle the player is currently in.
            bool found = false;
            if (GameAgentManager.Instance != null && GameAgentManager.Instance.FocusedGameAgent != null && GameAgentManager.Instance.FocusedGameAgent.Vehicle != null)
            {
                VehicleClass vehicleClass = GameAgentManager.Instance.FocusedGameAgent.Vehicle.VehicleClass;
                foreach(VehicleInputDisplaySettings settings in vehicleInputDisplaySettings)
                {
                    if (settings.vehicleClass == vehicleClass)
                    {
                        Display(settings.vehicleClass);
                        found = true;
                        break;
                    }
                }
            }

            // Display input for the first vehicle class.
            if (!found) Display(vehicleInputDisplaySettings.Count > 0 ? vehicleInputDisplaySettings[0].vehicleClass : null);
        }


        // Add spaces to a camel case string.
        protected virtual string AddSpacesToCamelCase(string s)
        {
            return Regex.Replace(s, "([a-z])([A-Z])", "$1 $2");
        }


        // Get the name of a control from an input binding path.
        protected virtual string GetControlName(string path)
        {
            string[] temp = path.Split('/');
            foreach (string s in temp)
            {
                if (s.Contains('<')) continue;

                return s;
            }

            return "";
        }


        // Display an input action on the UI.
        protected virtual void Display(InputAction action)
        {

            foreach (InputBinding binding in action.bindings)
            {
                if (binding.isPartOfComposite) continue;

                if (binding.isComposite)
                {
                    // Get all the composite parts
                    int index = action.bindings.IndexOf((value) => value.id == binding.id);

                    List<InputBinding> compositeParts = new List<InputBinding>();
                    while (true)
                    {
                        index++;
                        if (action.bindings.Count <= index) break;
                        if (!action.bindings[index].isPartOfComposite) break;
                        if (!IsValidGroup(action.bindings[index])) continue;

                        compositeParts.Add(action.bindings[index]);

                    }

                    string horizontalPath = "";
                    string verticalPath = "";
                    List<int> horizontalAndVerticalIndexes = new List<int>();

                    for (int i = 0; i < compositeParts.Count; ++i)
                    {
                        for (int j = 0; j < compositeParts.Count; ++j)
                        {
                            if (i == j) continue;

                            if (GetControlName(compositeParts[i].path) == GetControlName(compositeParts[j].path))
                            {
                                if (compositeParts[i].path.EndsWith("up"))
                                {
                                    verticalPath = compositeParts[i].path.Substring(0, compositeParts[i].path.Length - 2) + "vertical";
                                    horizontalAndVerticalIndexes.Add(i);
                                    horizontalAndVerticalIndexes.Add(j);
                                }
                                if (compositeParts[i].path.EndsWith("left"))
                                {
                                    horizontalPath = compositeParts[i].path.Substring(0, compositeParts[i].path.Length - 4) + "horizontal";
                                    horizontalAndVerticalIndexes.Add(i);
                                    horizontalAndVerticalIndexes.Add(j);
                                }
                            }
                        }
                    }

                    if (horizontalPath != "")
                    {
                        string axisLabel = "horizontal";
                        foreach (InputActionAxisLabels actionAxisLabelsOverride in actionAxisLabelOverrides)
                        {
                            if (actionAxisLabelsOverride.action == action.name)
                            {
                                axisLabel = actionAxisLabelsOverride.horizontalAxisName;
                            }
                        }
                        Display(horizontalPath, axisLabel == "" ? action.name : action.name + " " + axisLabel);
                    }

                    if (verticalPath != "")
                    {
                        string axisLabel = "vertical";
                        foreach (InputActionAxisLabels actionAxisLabelsOverride in actionAxisLabelOverrides)
                        {
                            if (actionAxisLabelsOverride.action == action.name)
                            {
                                axisLabel = actionAxisLabelsOverride.verticalAxisName;
                            }
                        }
                        Display(verticalPath, axisLabel == "" ? action.name : action.name + " " + axisLabel);
                    }

                    for (int i = 0; i < compositeParts.Count; ++i)
                    {
                        if (horizontalAndVerticalIndexes.IndexOf(i) == -1)
                        {
                            Display(compositeParts[i].path, action.name + " " + compositeParts[i].name);
                        }
                    }
                }
                else
                {
                    if (IsValidGroup(binding))
                    {
                        Display(binding.path, binding.action);
                    }
                }
            }
        }


        // Check if an input binding is part of a group displayed by this menu.
        protected virtual bool IsValidGroup(InputBinding binding)
        {
            if (inputGroups.Count > 0)
            {
                bool found = false;
                foreach (string group in inputGroups)
                {
                    if (binding.groups.Contains(group))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
            }

            return true;
        }


        // Display an input action on the UI via its binding path.
        protected virtual void Display(string path, string displayValue)
        {
            foreach (InputControlDisplayItem item in controlDisplayItems)
            {
                if (path.EndsWith(item.InputControl.ID))
                {
                    item.Set(displayValue);
                }
            }
        }


        /// <summary>
        /// Display the input for a vehicle class.
        /// </summary>
        /// <param name="vehicleClass">The vehicle class to display input for.</param>
        public virtual void Display(VehicleClass vehicleClass)
        {
            foreach (InputControlDisplayItem displayItem in controlDisplayItems)
            {
                displayItem.Hide();
            }

            for (int i = 0; i < vehicleClassButtons.Count; ++i)
            {
                if (vehicleInputDisplaySettings[i].vehicleClass == vehicleClass)
                {
                    vehicleClassButtons[i].OnSelect(null);
                }
                else
                {
                    vehicleClassButtons[i].OnDeselect(null);
                }
            }

            foreach (VehicleInputDisplaySettings settings in vehicleInputDisplaySettings)
            {
                if (settings.vehicleClass == vehicleClass)
                {
                    foreach (InputActionAsset inputActions in inputActionAssets)
                    {
                        foreach (InputActionMap map in inputActions.actionMaps)
                        {
                            if (settings.maps.Count > 0 && settings.maps.IndexOf(map.name) == -1)
                            {
                                continue;
                            }

                            foreach (InputAction action in map.actions)
                            {
                                Display(action);
                            }
                        }
                    }
                }
            }
        }
    }
}

