using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;

namespace VSX.Controls
{
    /// <summary>
    /// Controls a control group item in the controls menu.
    /// </summary>
    public class ControlsMenuGroupItem : MonoBehaviour
    {
        [Tooltip("The name of the group of controls in the menu.")]
        [SerializeField]
        protected UVCText groupLabelText;


        [Tooltip("Whether to make the label upper case.")]
        [SerializeField]
        protected bool toUpper = true;


        /// <summary>
        /// Set the parameters of this group item.
        /// </summary>
        /// <param name="groupLabel">The label for this group item.</param>
        public virtual void Set(string groupLabel)
        {
            groupLabelText.text = toUpper ? groupLabel.ToUpper() : groupLabel;
        }
    }
}