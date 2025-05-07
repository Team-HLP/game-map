using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls;
using UnityEngine.Events;
using VSX.UI;

namespace VSX.VehicleCombatKits
{
    /// <summary>
    /// Base class for a player input script that interacts with a generic menu (e.g. pause menu).
    /// </summary>
    public class PlayerInput_Base_MenuControls : GeneralInput
    {

        [Tooltip("The menu to control.")]
        [SerializeField]
        protected MenuController menuController;


        // Unity Event called when Back is pressed
        public UnityEvent onBackPressed;


        protected bool menuOpenedThisFrame = false;



        protected override void Reset()
        {
            base.Reset();

            menuController = GetComponentInChildren<MenuController>();
        }


        protected override void Awake()
        {
            base.Awake();

            menuController.onMenuOpened.AddListener(OnMenuOpened);
        }


        protected virtual void ToggleMenu()
        {
            if (menuController.MenuOpen)
            {
                menuController.CloseMenu();
            }
            else
            {
                menuController.OpenMenu();
            }
        }


        protected virtual void OpenMenu()
        {
            if (menuController.MenuOpen) return;

            menuController.OpenMenu();
        }


        protected virtual void Back()
        {
            if (!menuController.MenuOpen) return;

            if (menuOpenedThisFrame) return;

            if (menuController.CurrentState == null) return;

            menuController.CurrentState.ExitToParent();

            onBackPressed.Invoke();
        }


        protected virtual void OnMenuOpened()
        {
            menuOpenedThisFrame = true;

            StartCoroutine(ResetFrameParameters());
        }


        IEnumerator ResetFrameParameters()
        {
            yield return new WaitForEndOfFrame();

            menuOpenedThisFrame = false;
        }
    }
}
