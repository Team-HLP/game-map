using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;

namespace VSX.Loadouts
{
    /// <summary>
    /// Displays a stat (text and/or bar) on the UI.
    /// </summary>
    public class StatController : MonoBehaviour
    {
        [Tooltip("The gameobject that is activated/deactivated to show/hide the stat on the UI.")]
        [SerializeField]
        protected GameObject activationObject;

        [Tooltip("Whether to hide the stat completely if the value is zero.")]
        [SerializeField]
        protected bool hideIfValueIsZero = true;

        [Tooltip("Whether to hide the stat completely if the value is infinity.")]
        [SerializeField]
        protected bool hideIfValueIsInfinity = true;

        [Tooltip("Whether to show the text value of the stat on the UI.")]
        [SerializeField]
        protected bool showText = true;

        [Tooltip("The text showing the stat value on the UI.")]
        [SerializeField]
        protected UVCText valueText;

        [Tooltip("A custom value that is multiplied to the stat value before displaying it.")]
        [SerializeField]
        protected float valueMultiplier = 1;

        [Tooltip("The number of decimals that the stat text value is displayed with.")]
        [SerializeField]
        protected int valueDecimals = 0;

        [Tooltip("The suffix that is appended onto the end of the stat value before displaying on the UI.")]
        [SerializeField]
        protected string suffix = "units";

        [Tooltip("The value to display for an infinite stat value.")]
        [SerializeField]
        protected string infiniteValueDisplay = "-";

        [Tooltip("Whether to show the stat bar on the UI.")]
        [SerializeField]
        protected bool showBar = true;

        [Tooltip("The loadout stat bar.")]
        [SerializeField] protected UIFillBar bar;


        protected virtual void Reset()
        {
            activationObject = gameObject;

            valueText = GetComponentInChildren<UVCText>();
            bar = GetComponentInChildren<UIFillBar>();
        }


        /// <summary>
        /// Get whether an object is relevant to display the stat for.
        /// </summary>
        /// <param name="statTarget">The object to display the stat for.</param>
        /// <returns>Whether the object is compatible with the stat type.</returns>
        public virtual bool IsCompatible(GameObject statTarget)
        {
            return statTarget != null;
        }


        /// <summary>
        /// Provide the stat controller with relevant loadout information to use when displaying the stat.
        /// </summary>
        /// <param name="loadoutItems">The loadout items.</param>
        /// <param name="itemIndex">The list index of the loadout item whose stats are being displayed.</param>
        public virtual void Set(LoadoutItems loadoutItems, int itemIndex) { }


        /// <summary>
        /// Set the list of objects to gather stat information from, and the index of the object that the stats are being displayed for.
        /// </summary>
        /// <param name="statTargets">The list of objects to gather stat information from (including comparison information).</param>
        /// <param name="statTargetIndex">The list index of the object that the stat is being displayed for.</param>
        protected virtual void Set(List<GameObject> statTargets, int statTargetIndex)
        {
            if (!IsCompatible(statTargets[statTargetIndex]))
            {
                gameObject.SetActive(false);
                return;
            }

            float statValue = GetStatValue(statTargets[statTargetIndex]);
            if (hideIfValueIsZero && Mathf.Approximately(statValue, 0))
            {
                gameObject.SetActive(false);
                return;
            }

            if (hideIfValueIsInfinity && statValue == Mathf.Infinity)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            UpdateText(statTargets, statTargetIndex);

            UpdateBar(statTargets, statTargetIndex);
        }


        /// <summary>
        /// Update the UI text display for the stat.
        /// </summary>
        /// <param name="statTargets">The list of objects to gather stat information from (including comparison information).</param>
        /// <param name="statTargetIndex">The list index of the object that the stat is being displayed for.</param>
        protected virtual void UpdateText(List<GameObject> statTargets, int statTargetIndex)
        {
            if (valueText == null) return;

            valueText.gameObject.SetActive(showText);

            if (!showText) return;

            float statValue = GetStatValue(statTargets[statTargetIndex]);

            valueText.text = FormatTextDisplay(statValue);
        }


        /// <summary>
        /// Update the UI bar display for the stat.
        /// </summary>
        /// <param name="statTargets">The list of objects to gather stat information from (including comparison information).</param>
        /// <param name="statTargetIndex">The list index of the object that the stat is being displayed for.</param>
        protected virtual void UpdateBar(List<GameObject> statTargets, int statTargetIndex)
        {
            if (bar == null) return;

            bar.gameObject.SetActive(showBar);

            if (!showBar) return;

            float statValue = GetStatValue(statTargets[statTargetIndex]);

            float maxStatValue = 0;
            foreach (GameObject peerStatTarget in statTargets)
            {
                if (!IsCompatible(peerStatTarget)) continue;

                float peerStatValue = GetStatValue(peerStatTarget);
                if (peerStatValue != Mathf.Infinity) maxStatValue = Mathf.Max(maxStatValue, peerStatValue);
            }

            if (!Mathf.Approximately(maxStatValue, 0))
            {
                bar.SetFill(statValue / maxStatValue);
            }
        }


        /// <summary>
        /// Get the float value of the stat for an object.
        /// </summary>
        /// <param name="statTarget">The object to get the stat value for.</param>
        /// <returns>The stat value.</returns>
        protected virtual float GetStatValue(GameObject statTarget)
        {
            return 0;
        }


        /// <summary>
        /// Format the text display for the stat.
        /// </summary>
        /// <param name="statValue">The raw stat value.</param>
        /// <returns>The formatted display text.</returns>
        protected virtual string FormatTextDisplay(float statValue)
        {
            string result;
            if (statValue == Mathf.Infinity)
            {
                result = infiniteValueDisplay;
            }
            else
            {
                result = (statValue * valueMultiplier).ToString("F" + valueDecimals.ToString());
            }

            return result + (suffix == "" ? "" : " " + suffix);
        }
    }
}

