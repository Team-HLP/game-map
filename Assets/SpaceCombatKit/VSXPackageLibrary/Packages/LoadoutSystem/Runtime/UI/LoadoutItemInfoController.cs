using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VSX.UI;

namespace VSX.Loadouts
{
    /// <summary>
    /// Base class to manage the display of info for a loadout item.
    /// </summary>
    public class LoadoutItemInfoController : MonoBehaviour
    {
        [Tooltip("The loadout manager to display info for.")]
        [SerializeField]
        protected LoadoutManager loadoutManager;

        [Tooltip("The UI handle to activate (to show the info UI) or deactivate (to hide the info UI).")]
        [SerializeField]
        protected GameObject UIHandle;

        [Tooltip("The loadout item label.")]
        [SerializeField]
        protected UVCText labelText;
        public UVCText LabelText
        {
            get { return labelText; }
            set { labelText = value; }
        }

        [Tooltip("The loadout item description.")]
        [SerializeField]
        protected UVCText descriptionText;
        public UVCText DescriptionText
        {
            get { return descriptionText; }
            set { descriptionText = value; }
        }

        [Tooltip("The icon image for the loadout item.")]
        [SerializeField]
        protected Image iconImage;
        public Image IconImage
        {
            get { return iconImage; }
            set { iconImage = value; }
        }

        [Tooltip("The list index of the sprite in the loadout item's Sprites list to show as the icon.")]
        [SerializeField]
        protected int iconSpriteIndex = 0;

        protected List<StatController> statControllers = new List<StatController>();


        protected virtual void Reset()
        {
            loadoutManager = FindObjectOfType<LoadoutManager>();
        }


        protected virtual void Awake()
        {
            statControllers = new List<StatController>(GetComponentsInChildren<StatController>());

            loadoutManager.onLoadoutChanged.AddListener(UpdateInfo);

            UpdateInfo();
        }


        protected virtual void Show()
        {
            UIHandle.SetActive(true);
        }


        protected virtual void Hide()
        {
            UIHandle.SetActive(false);
        }


        /// <summary>
        /// Update the loadout item info.
        /// </summary>
        public virtual void UpdateInfo()
        {
            Show();
        }


        /// <summary>
        /// Set the label for the loadout item.
        /// </summary>
        /// <param name="text">The label content.</param>
        public virtual void SetLabel(string text)
        {
            if (labelText != null) labelText.text = text;
        }


        /// <summary>
        /// Set the description for the loadout item.
        /// </summary>
        /// <param name="text">The description content.</param>
        public virtual void SetDescription(string text)
        {
            if (descriptionText != null) descriptionText.text = text;
        }


        /// <summary>
        /// Set the icon sprite for the loadout item.
        /// </summary>
        /// <param name="icon">The icon sprite.</param>
        public virtual void SetIcon(Sprite icon)
        {
            if (iconImage != null) iconImage.sprite = icon;
        }
    }
}

