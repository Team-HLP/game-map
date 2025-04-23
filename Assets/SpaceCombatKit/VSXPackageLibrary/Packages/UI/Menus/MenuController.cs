using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace VSX.UI
{
    /// <summary>
    /// Controls a menu in the game.
    /// </summary>
    public class MenuController : MonoBehaviour
    {

        protected List<MenuState> states = new List<MenuState>();

        [Tooltip("Whether the menu is exitable, e.g. a main menu may not be exitable within its own scene.")]
        [SerializeField]
        protected bool exitable = true;
        public bool Exitable
        {
            get => exitable;
            set => exitable = value;
        }

        [Tooltip("The gameobject that represents the menu. It is activated/deactivated to open/close the menu.")]
        [SerializeField]
        protected GameObject menuHandle;

        [Tooltip("The default state of the menu - the state that it opens with when it is opened with no state specified.")]
        [SerializeField]
        protected MenuState defaultState;

        protected MenuState currentState;
        public MenuState CurrentState { get { return currentState; } }

        [Tooltip("Whether to open the menu when the scene starts.")]
        [SerializeField]
        protected bool openOnStart = false;

        [Tooltip("Whether to select a first UI object when the menu opens. Useful for e.g. gamepads.")]
        [SerializeField]
        protected bool selectFirstUIObject;
        public virtual bool SelectFirstUIObject
        {
            get { return selectFirstUIObject; }
            set { selectFirstUIObject = value; }
        }

        protected bool menuOpen = false;
        public bool MenuOpen { get { return menuOpen; } }

        public UnityEvent onMenuOpened;
        public UnityEvent onMenuClosed;



        protected virtual void Awake()
        {
            states = new List<MenuState>(GetComponentsInChildren<MenuState>(true));
            foreach (MenuState menuState in states)
            {
                menuState.MenuController = this;
            }

            if (menuHandle != null) menuHandle.SetActive(false);

        }


        protected virtual void Start()
        {
            if (openOnStart)
            {
                OpenMenu();
            }
        }


        /// <summary>
        /// Set the first UI object selected for the current menu state.
        /// </summary>
        public virtual void SetFirstUIObjectSelected()
        {
            if (currentState != null && currentState.FirstSelectedUIObject != null)
            {
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(currentState.FirstSelectedUIObject);
                }
            }
        }


        /// <summary>
        /// Clear the selected UIObject 
        /// </summary>
        public virtual void ClearSelectedUIObject()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }


        /// <summary>
        /// Open the menu to the default state.
        /// </summary>
        public virtual void OpenMenu()
        {
            OpenMenu(defaultState);
        }


        /// <summary>
        /// Open the menu to a specific state.
        /// </summary>
        /// <param name="state">The menu state to open to.</param>
        public virtual void OpenMenu(MenuState state)
        {
            menuOpen = true;

            if (menuHandle != null)
            {
                menuHandle.gameObject.SetActive(true);
            }

            EnterState(state);

            onMenuOpened.Invoke();
        }


        /// <summary>
        /// Close the menu.
        /// </summary>
        public virtual void CloseMenu()
        {
            if (!exitable) return;

            menuOpen = false;

            if (currentState != null)
            {
                currentState.OnExit();
            }

            if (menuHandle != null)
            {
                menuHandle.gameObject.SetActive(false);
            }

            ClearSelectedUIObject();

            onMenuClosed.Invoke();
        }


        /// <summary>
        /// Enter a menu state.
        /// </summary>
        /// <param name="menuState">The menu state to enter.</param>
        public virtual void EnterState(MenuState menuState)
        {
            if (menuState == null)
            {
                CloseMenu();
            }
            else
            {
                if (currentState != null) currentState.OnExit();

                currentState = menuState;

                menuState.OnEnter();

                if (selectFirstUIObject)
                {
                    SetFirstUIObjectSelected();
                }
            }
        }
    }
}

