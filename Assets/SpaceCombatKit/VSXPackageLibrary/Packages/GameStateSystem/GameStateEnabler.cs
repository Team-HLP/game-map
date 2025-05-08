using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.GameStates
{
    /// <summary>
    /// Run events based upon the game state the game is in.
    /// </summary>
    public class GameStateEnabler : MonoBehaviour
    {

        [Tooltip("The game states that trigger the events when entered and exited.")]
        [SerializeField]
        protected List<GameState> compatibleGameStates = new List<GameState>();

        [Tooltip("Event called when one of the compatible game states is entered.")]
        public UnityEvent onCompatibleGameStateEntered;

        [Tooltip("Event called when one of the compatible game states is exited.")]
        public UnityEvent onIncompatibleGameStateEntered;


        protected virtual void Awake()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.onEnteredGameState.AddListener(OnEnteredGameState);
            }
        }


        // Called every time a new game state is entered.
        protected virtual void OnEnteredGameState(GameState gameState)
        {
            if (compatibleGameStates.Count == 0 || compatibleGameStates.IndexOf(gameState) != -1)
            {
                OnCompatibleGameStateEntered();
                onCompatibleGameStateEntered.Invoke();
            }
            else
            {
                OnIncompatibleGameStateEntered();
                onIncompatibleGameStateEntered.Invoke();
            }
        }


        // Called when one of the specified game states is entered.
        protected virtual void OnCompatibleGameStateEntered() { }


        // Called when a game state is entered that is not one of those specified.
        protected virtual void OnIncompatibleGameStateEntered() { }
    }
}

