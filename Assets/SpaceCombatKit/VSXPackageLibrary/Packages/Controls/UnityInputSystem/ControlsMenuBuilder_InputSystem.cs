using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.Collections.Specialized.BitVector32;


namespace VSX.Controls.UnityInputSystem
{
    /// <summary>
    /// Builds a controls menu for Unity's Input System.
    /// </summary>
    public class ControlsMenuBuilder_InputSystem : MonoBehaviour
    {

        [Tooltip("The input action assets to be included in the menu.")]
        [SerializeField]
        protected List<InputActionAsset> inputAssets = new List<InputActionAsset>();


        [Tooltip("The keywords for control schemes to be included in the menu. Leave empty for all control schemes to be included.")]
        [SerializeField]
        protected List<string> controlSchemeKeywords = new List<string>();


        [Tooltip("Exclude these maps from the menu.")]
        [SerializeField]
        protected List<string> excludedMaps = new List<string>();


        /// <summary>
        /// Stores a binding display string for a specific input binding path.
        /// </summary>
        [System.Serializable]
        public class BindingDisplayItem
        {
            [Tooltip("The input binding path to display for.")]
            public string path;

            [Tooltip("The UI display value.")]
            public string displayValue;
        }


        [Tooltip("A list of display strings for specific input binding paths.")]
        [SerializeField]
        protected List<BindingDisplayItem> bindingDisplayItems = new List<BindingDisplayItem>();


        [Tooltip("Actions that are excluded from being displayed on the menu.")]
        [SerializeField]
        protected List<string> excludedActions = new List<string>();


        [Tooltip("The input binding display options used when getting the display name from Unity's Input System.")]
        [SerializeField]
        protected InputBinding.DisplayStringOptions inputBindingDisplayOptions;


        [Tooltip("The prefab to show a control group in the menu.")]
        [SerializeField]
        protected ControlsMenuGroupItem controlsMenuGroupItemPrefab;


        [Tooltip("The prefab to show a single item in the controls menu.")]
        [SerializeField]
        protected ControlsMenuItem controlsMenuItemPrefab;


        [Tooltip("The parent for items in the controls menu.")]
        [SerializeField]
        protected Transform controlsMenuItemsParent;



        // Start is called before the first frame update
        protected virtual void Start()
        {
            BuildMenu();
        }


        // Get the string display value for a binding.
        protected virtual string GetDisplayValue(InputBinding binding)
        {
            foreach (BindingDisplayItem item in bindingDisplayItems)
            {
                if (item.path == binding.path)
                {
                    return item.displayValue;
                }
            }

            return binding.ToDisplayString(inputBindingDisplayOptions);
        }


        /// <summary>
        /// Build the menu.
        /// </summary>
        public virtual void BuildMenu()
        {
            foreach (InputActionAsset inputAsset in inputAssets)
            {
                foreach (InputActionMap map in inputAsset.actionMaps)
                {
                    bool excluded = false;
                    foreach (string excludedMap in excludedMaps)
                    {
                        if (map.name == excludedMap) excluded = true; break;
                    }
                    if (excluded) continue;

                    // Add a group
                    ControlsMenuGroupItem groupItemController = (ControlsMenuGroupItem)Instantiate(controlsMenuGroupItemPrefab, controlsMenuItemsParent);
                    groupItemController.Set(map.name);

                    foreach (InputAction action in map.actions)
                    {
                        List<string> bindingDisplayValues = new List<string>();
                        string actionName = action.name;

                        for (int i = 0; i < action.bindings.Count; ++i)
                        {
                            if (IsRelevantControlScheme(action.bindings[i]))
                            {
                                if (action.bindings[i].isPartOfComposite)
                                {
                                    // Display individually
                                    string compositeActionName = action.name + " " + TidyBindingName(action, action.bindings[i].name);
                                    Display(compositeActionName, GetDisplayValue(action.bindings[i]));
                                } 
                                else if (!action.bindings[i].isComposite)
                                {
                                    bindingDisplayValues.Add(GetDisplayValue(action.bindings[i]));
                                }
                            }
                        }

                        if (bindingDisplayValues.Count > 0)
                        {
                            Display(actionName, bindingDisplayValues.ToArray());
                        }
                    }
                }
            }
        }


        protected virtual void Display(string actionDisplayName, params string[] bindingDisplayValues)
        {
            ControlsMenuItem itemController = Instantiate(controlsMenuItemPrefab, controlsMenuItemsParent);
            itemController.Set(actionDisplayName, bindingDisplayValues);
        }


        protected virtual bool IsRelevantControlScheme(InputBinding binding)
        {
            if (controlSchemeKeywords.Count == 0) return true;

            foreach (string keyword in controlSchemeKeywords)
            {
                if (binding.groups.Contains(keyword))
                {
                    return true;
                }
            }

            return false;
        }


        protected string TidyBindingName(InputAction action, string bindingName)
        {

            bindingName = bindingName.Replace("up", "Up");
            bindingName = bindingName.Replace("down", "Down");
            bindingName = bindingName.Replace("left", "Left");
            bindingName = bindingName.Replace("right", "Right");

            if (action.name.Contains("Roll"))
            {
                bindingName = bindingName.Replace("positive", "Left");
                bindingName = bindingName.Replace("negative", "Right");
            }
            else if (action.name.Contains("Throttle"))
            {
                bindingName = bindingName.Replace("positive", "Up");
                bindingName = bindingName.Replace("negative", "Down");
            }
            else if (action.name.Contains("Move"))
            {
                bindingName = bindingName.Replace("Up", "Forward");
                bindingName = bindingName.Replace("Down", "Back");
            }

            return bindingName;
        }
    }
}

