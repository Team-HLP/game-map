using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UI
{
    /// <summary>
    /// A menu state for a menu that is governed by a menu controller. Typically represents a sub-menu.
    /// </summary>
    public class MenuState : MonoBehaviour
    {

        [Tooltip("The gameobject (e.g. button) that the event system should select when this menu state is entered.")]
        [SerializeField]
        protected GameObject firstSelectedUIObject;
        public GameObject FirstSelectedUIObject
        {
            get { return firstSelectedUIObject; }
            set { firstSelectedUIObject = value; }
        }

        [Tooltip("The object to activate when this menu state is entered (also deactivated when exited).")]
        [SerializeField]
        protected GameObject activationObject;

        [Tooltip("The parent state to exit to when backing out of this menu state.")]
        [SerializeField]
        protected MenuState parentState;

        public UnityEvent onStateEntered;
        public UnityEvent onStateExited;

        

        protected MenuController menuController;
        public MenuController MenuController
        {
            get { return menuController; }
            set { menuController = value; }
        }


        protected virtual void Awake()
        {
            if (activationObject != null)
            {
                activationObject.SetActive(false);
            }
        }


        /// <summary>
        /// Make the menu enter this menu state.
        /// </summary>
        public virtual void OnEnter()
        {
            if (activationObject != null) activationObject.SetActive(true);
            onStateEntered.Invoke();
        }


        /// <summary>
        /// Exit this menu state.
        /// </summary>
        public virtual void OnExit()
        {
            if (activationObject != null) activationObject.SetActive(false);
            onStateExited.Invoke();
        }


        public virtual void EnterState(bool enter)
        {
            if (enter)
            {
                menuController.EnterState(this);
            }
        }


        /// <summary>
        /// Exit to the parent menu state.
        /// </summary>
        public virtual void ExitToParent()
        {
            if (menuController != null)
            {
                menuController.EnterState(parentState);
            }
        }
    }
}

