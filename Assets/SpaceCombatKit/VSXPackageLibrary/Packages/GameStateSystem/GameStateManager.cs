using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VSX.GameStates
{

    /// <summary>
    /// Unity event for running functions when the game state changes.
    /// </summary>
    [System.Serializable]
    public class OnEnteredGameStateEventHandler : UnityEvent <GameState> { }


    /// <summary>
    /// Provides a way for the user to set parameters for each of the game states.
    /// </summary>
    [System.Serializable]
    public class GameStateInstance
    {
        [Tooltip("The game state for which these settings apply.")]
        public GameState gameState;

        [Tooltip("Whether to set the Time.timeScale to 0 upon entering this state (e.g. for menus).")]
        public bool freezeTimeOnEntry;

        [Tooltip("The seconds to pause before entering the game state.")]
        public float pauseBeforeEntry = 0f;

        [Tooltip("Whether to show the mouse cursor in this game state.")]
        public bool showCursor = true;

        [Tooltip("Whether to return the mouse cursor to the center of the screen upon entering this game state.")]
        public bool centerCursorAtStart = true;

        [Tooltip("Whether to lock the mouse cursor upon entering this game state.")]
        public bool lockCursor = false;

        [Tooltip("The game states from which this state can be entered. If empty, can enter from any other game state.")]
        public List<GameState> allowedEntryStates = new List<GameState>();

    }

    /// <summary>
    /// This class provides a single location to store the current state of the game.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {

        [SerializeField]
        protected GameState startingGameState;

        protected GameState currentGameState;
        public GameState CurrentGameState { get { return currentGameState; } }

        [Header("Game States")]

        // A list that stores the parameters associated with each of the game state
        [SerializeField]
        protected List<GameStateInstance> gameStates = new List<GameStateInstance>();

        // The singleton instance for this component
        public static GameStateManager Instance;

        // Enter game state with a delay using a coroutine
        protected Coroutine enterGameStateCoroutine;
        
        [Header("Events")]

        // Event
        public OnEnteredGameStateEventHandler onEnteredGameState;

        protected bool enteringState = false;


        protected void Awake()
        {
            // Enforce the singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Add the function to be called when the scene is exited
            SceneManager.sceneUnloaded += OnSceneUnloaded;

        }


        // Called at start of scene
        protected virtual void Start()
        {
            if (currentGameState == null) EnterGameState(startingGameState);
        }


        /// <summary>
        /// Enter a game state.
        /// </summary>
        /// <param name="newGameState">The new game state.</param>
        public void EnterGameState(GameState newGameState)
        {
            if (enteringState)
            {
                return;
            }
            
            for (int i = 0; i < gameStates.Count; ++i)
            {
                if (gameStates[i].gameState == newGameState)
                {
                    
                    if (gameStates[i].allowedEntryStates.Count > 0)
                    {
                        bool allow = false;
                        foreach (GameState allowedOriginState in gameStates[i].allowedEntryStates)
                        {
                            if (currentGameState == allowedOriginState)
                            {
                                allow = true;
                                break;
                            }
                        }

                        if (!allow) return;
                    }
                    
                    if (!Mathf.Approximately(gameStates[i].pauseBeforeEntry, 0))
                    {
                        enterGameStateCoroutine = StartCoroutine(PauseBeforeEntryCoroutine(gameStates[i].pauseBeforeEntry, gameStates[i]));
                    }
                    else
                    {
                        EnterGameState(gameStates[i]);
                    }
                }
            }
        }


        /// <summary>
        /// Enter a new game state.
        /// </summary>
        /// <param name="newGameStateInstance">The new game state instance.</param>
        protected void EnterGameState(GameStateInstance gameStateInstance)
        {
            // Stop game state change coroutine
            if (enterGameStateCoroutine != null) StopCoroutine(enterGameStateCoroutine);
            
            // Update the game state
            currentGameState = gameStateInstance.gameState;

            // Freeze time if applicable
            if (gameStateInstance.freezeTimeOnEntry)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
            }
            else
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
            }

            SetCursorVisible(gameStateInstance.showCursor);
            
            if (gameStateInstance.centerCursorAtStart)
            {
                CenterCursor();
            }

            SetCursorLock(gameStateInstance.lockCursor);

            // Call event
            onEnteredGameState.Invoke(gameStateInstance.gameState);
        }


        /// <summary>
        /// Center the cursor
        /// </summary>
        public void CenterCursor()
        {
            CursorLockMode initialState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = initialState;
        }


        /// <summary>
        /// Change cursor visibility.
        /// </summary>
	    public void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
        }


        /// <summary>
        /// Set the cursor locked or not.
        /// </summary>
        /// <param name="locked">Whether to lock the cursor.</param>
        public void SetCursorLock(bool locked)
        {
            if (locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }


        // Coroutine to pause before entering the game state.
        IEnumerator PauseBeforeEntryCoroutine(float pause, GameStateInstance nextState)
        {
            enteringState = true;
            yield return new WaitForSeconds(pause);
            enteringState = false;
            EnterGameState(nextState);
        }


        // Called when the scene manager exits a scene. Disable any cursor lock and show cursor.
        protected void OnSceneUnloaded(Scene scene)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
    }
}
