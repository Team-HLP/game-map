using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;


namespace VSX.Weapons
{
    /// <summary>
    /// Changes the color of the UI when aim assist is active.
    /// </summary>
    public class AimAssistUIController : MonoBehaviour
    {
        [Tooltip("The weapons controller to get aim assist information from.")]
        [SerializeField]
        protected WeaponsController weapons;

        [Tooltip("The UI to apply the color to.")]
        [SerializeField]
        protected UIColorManager colorManager;

        [Tooltip("The color to apply to the UI when aim assist is not active.")]
        [SerializeField]
        protected Color noAimAssistColor = Color.white;

        [Tooltip("The color to apply to the UI when aim assist is active.")]
        [SerializeField]
        protected Color aimAssistColor = Color.red;

        

        protected virtual void Awake()
        {
            if (weapons != null) weapons.onAimAssistUpdated += OnAimAssistUpdated;
        }


        /// <summary>
        /// Called when the aim assist state is updated.
        /// </summary>
        protected virtual void OnAimAssistUpdated()
        {
            colorManager.SetColor(weapons.AimAssistState ? aimAssistColor : noAimAssistColor);
        }
    }

}
