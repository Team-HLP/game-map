using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Controls
{
    /// <summary>
    /// Used instead of an enum as an ID for a specific control on an input device.
    /// </summary>
    [CreateAssetMenu]
    public class InputControl : ScriptableObject
    {
        [Tooltip("The ID for this control (can be an input path or anything that designates a control for the input solution you're using).")]
        public string ID = "Control";
    }
}
