using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.GameStates;
using VSX.Vehicles;
using VSX.HUD;


namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Enables the HUD based on the game state. Useful for switching off the HUD e.g. when menus are activated.
    /// </summary>
    public class GameStateHUDEnabler : MonoBehaviour
    {
        protected HUDManager hudManager;

        [Tooltip("The Game States in which the HUD should be visible/active.")]
        [SerializeField]
        protected List<GameState> HUDActiveGameStates = new List<GameState>();


        protected virtual void Awake()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.onEnteredGameState.AddListener(OnEnteredGameState);
            }
        }


        /// <summary>
        /// Set the vehicle from which to get the HUD reference.
        /// </summary>
        /// <param name="vehicle">The vehicle from which to get the HUD.</param>
        public virtual void SetVehicle(Vehicle vehicle)
        {
            hudManager = vehicle == null ? null : vehicle.GetComponentInChildren<HUDManager>();
        }


        /// <summary>
        /// Clear the reference to the HUD.
        /// </summary>
        public virtual void ClearReferences()
        {
            hudManager = null;
        }


        // Called when a new Game State is entered.
        protected virtual void OnEnteredGameState(GameState gameState)
        {
            if (hudManager != null)
            {
                if (HUDActiveGameStates.IndexOf(gameState) != -1)
                {
                    hudManager.ActivateHUD();
                }
                else
                {
                    hudManager.DeactivateHUD();
                }
            }
        }
    }

}
