using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;

namespace VSX.Controls
{
    /// <summary>
    /// Displays a single input control for a device on the UI.
    /// </summary>
    public class InputControlDisplayItem : MonoBehaviour
    {

        [Tooltip("The control that this item displays on the UI.")]
        [SerializeField]
        protected InputControl inputControl;
        public virtual InputControl InputControl { get => inputControl; }


        [Tooltip("The control that this item displays on the UI.")]
        [SerializeField]
        protected UVCText m_Text;


        /// <summary>
        /// Hide this item on the UI.
        /// </summary>
        public virtual void Hide() { gameObject.SetActive(false); }


        /// <summary>
        /// Show this item on the UI.
        /// </summary>
        public virtual void Show() { gameObject.SetActive(true); }



        protected virtual void Reset()
        {
            m_Text = GetComponentInChildren<UVCText>();
        }


        /// <summary>
        /// Set this display item.
        /// </summary>
        /// <param name="displayText">The text to display for this input control.</param>
        /// <param name="show">Whether to show this item on the UI (or hide it).</param>
        public virtual void Set(string displayText, bool show = true)
        {
            m_Text.text = displayText;

            if (show)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}

