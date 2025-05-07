using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.GameStates
{
    /// <summary>
    /// Enter a game state. This component is useful for entering a game state from a Unity Event.
    /// </summary>
    public class GameStateEnter : MonoBehaviour
    {
        [Tooltip("The default game state to enter.")]
        [SerializeField]
        protected GameState defaultGameState;


        /// <summary>
        /// Enter the default game state.
        /// </summary>
        public virtual void EnterGameState()
        {
            GameStateManager.Instance.EnterGameState(defaultGameState);
        }


        /// <summary>
        /// Enter a specified game state.
        /// </summary>
        /// <param name="newGameState">The game state to enter.</param>
        public virtual void EnterGameState(GameState newGameState)
        {
            GameStateManager.Instance.EnterGameState(newGameState);
        }
    }
}
