using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Controls.UnityInputManager
{
    /// <summary>
    /// Set up input to drive functions using Unity Events.
    /// </summary>
    public class CustomInputGeneric : GeneralInput
    {
        [Tooltip("The input items.")]
        [SerializeField]
        protected List<CustomInputEventItem> inputItems = new List<CustomInputEventItem>();

        
        protected override void OnInputUpdate()
        {
            // Run the input items
            for (int i = 0; i < inputItems.Count; ++i)
            {
                inputItems[i].ProcessEvents();
            }
        }
    }
}