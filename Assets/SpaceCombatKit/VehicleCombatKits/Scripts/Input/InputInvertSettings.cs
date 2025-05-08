using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.VehicleCombatKits
{
    [System.Serializable]
    public class InputInvertSettings
    {
        [SerializeField]
        protected bool invertMouse;
        public virtual bool InvertMouse { get => invertMouse; set => invertMouse = value; }


        [SerializeField]
        protected bool invertKeyboard;
        public virtual bool InvertKeyboard { get => invertKeyboard; set => invertKeyboard = value; }


        [SerializeField]
        protected bool invertGamepad;
        public virtual bool InvertGamepad { get => invertGamepad; set => invertGamepad = value; }


        [SerializeField]
        protected bool invertJoystick;
        public virtual bool InvertJoystick { get => invertJoystick; set => invertJoystick = value; }
    }
}

