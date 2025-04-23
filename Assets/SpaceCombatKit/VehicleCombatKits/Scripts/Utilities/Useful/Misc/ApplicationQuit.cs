using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Class exposing a function to quit the application, can be added to a gameobject and called from a Unity Event or button click events.
    /// </summary>
    public class ApplicationQuit : MonoBehaviour
    {
        /// <summary>
        /// Quit the application.
        /// </summary>
        public virtual void Quit()
        {
            Application.Quit();
        }
    }
}
