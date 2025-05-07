using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VSX.UI
{
    /// <summary>
    /// Contains settings for a button state.
    /// </summary>
    [System.Serializable]
    public class ButtonState
    {

        [Tooltip("The name of this button state.")]
        [SerializeField]
        protected string stateName = "Normal";
        public string StateName
        {
            get { return stateName; }
            set { stateName = value; }
        }

        /// <summary>
        /// Adds settings for an image for this button state.
        /// </summary>
        [System.Serializable]
        public class ImageSetting
        {
            public Image image;

            public bool modifyColor;
            public Color color;

            public bool modifySprite;
            public Sprite sprite;
        }

        /// <summary>
        /// Adds settings for a text for this button state.
        /// </summary>
        [System.Serializable]
        public class TextSetting
        {
            public UVCText text;

            public bool modifyColor;
            public Color color;

            public bool modifyContent;
            public string content;
        }

        [Tooltip("The state's priority. Buttons will maintain the highest priority active state at all times.")]
        public int priority = 0;

        protected bool isPriorityState;
        public bool IsPriorityState
        {
            get => isPriorityState;
            set
            {
                bool wasPriorityState = isPriorityState;
                isPriorityState = value;

                if (isPriorityState && !wasPriorityState)
                {
                    onIsPriorityState.Invoke();
                }
                if (!isPriorityState && wasPriorityState)
                {
                    onNotPriorityState.Invoke();
                }
            }
        }

        protected bool stateActive = false;
        public bool StateActive
        {
            get { return stateActive; }
            set
            {
                bool wasActive = stateActive;
                stateActive = value;

                if (stateActive && !wasActive)
                {
                    onStateActivated.Invoke();
                }
                if (!stateActive && wasActive)
                {
                    onStateDeactivated.Invoke();
                }
            }
        }

        [Tooltip("The image settings for this button state.")]
        public List<ImageSetting> imageSettings = new List<ImageSetting>();

        [Tooltip("The text settings for this button state.")]
        public List<TextSetting> textSettings = new List<TextSetting>();

        public UnityEvent onStateActivated;
        public UnityEvent onStateDeactivated;

        public UnityEvent onIsPriorityState;
        public UnityEvent onNotPriorityState;


        // Constructor
        public ButtonState(string stateName)
        {
            this.stateName = stateName;
        }
    }
}


