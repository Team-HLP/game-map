using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Utilities;

namespace VSX.GameStates
{
    /// <summary>
    /// A condition that returns True only if the current game state is one of those specified.
    /// </summary>
    public class GameStateCondition : Condition
    {
        [Tooltip("The game states for which this condition returns True.")]
        [SerializeField]
        protected List<GameState> gameStates = new List<GameState>();
        public List<GameState> GameStates { get => gameStates; }


        /// <summary>
        /// Get the current value of the condition.
        /// </summary>
        /// <returns>The current value of the condition.</returns>
        public override bool Value()
        {
            if (GameStateManager.Instance == null) return true;

            for(int i = 0; i < gameStates.Count; ++i)
            {
                if (GameStateManager.Instance.CurrentGameState == gameStates[i]) return true;
            }

            return false;
        }
    }
}
