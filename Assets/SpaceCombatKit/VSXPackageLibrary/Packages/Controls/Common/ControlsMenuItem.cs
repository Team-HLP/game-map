using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;

namespace VSX.Controls
{
    /// <summary>
    /// Manages a single item on the controls menu.
    /// </summary>
    public class ControlsMenuItem : MonoBehaviour
    {

        [Tooltip("The text for the action name of the control.")]
        [SerializeField]
        protected UVCText actionText;


        [Tooltip("The texts for the displaying the bindings assigned to the action.")]
        [SerializeField]
        protected List<UVCText> bindingTexts = new List<UVCText>();



        /// <summary>
        /// Set the parameters for this controls menu item. 
        /// </summary>
        /// <param name="action">The action for this control input.</param>
        /// <param name="input">The control input.</param>
        public virtual void Set(string action, params string[] bindingDisplayValues)
        {
            foreach (UVCText text in bindingTexts)
            {
                text.text = "";
            }

            actionText.text = action;

            for(int i = 0; i < bindingDisplayValues.Length; ++i)
            {
                if (i < bindingTexts.Count)
                {
                    bindingTexts[i].text = bindingDisplayValues[i];
                }
                else
                {
                    break;
                }
            }
        }
    }
}