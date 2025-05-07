using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.Controls
{
    /// <summary>
    /// Stores unique suffixes for the horizontal and vertical axes for specific action names.
    /// </summary>
    [System.Serializable]
    public class InputActionAxisLabels
    {
        [Tooltip("The action to display labels for.")]
        public string action;

        [Tooltip("The name (suffix) for the horizontal axis of this action.")]
        public string horizontalAxisName = "Horizontal";

        [Tooltip("The name (suffix) for the vertical axis of this action.")]
        public string verticalAxisName = "Vertical";
    }
}
