using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.Utilities
{
    public class TriggerCondition : Condition
    {
        bool triggered = false;
        public bool triggeredValue = true;
        public LayerMask triggerLayers;


        public override bool Value()
        {
            return triggered ? triggeredValue : !triggeredValue;
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if ((triggerLayers & (1 << other.gameObject.layer)) != 0)
            {
                triggered = true;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if ((triggerLayers & (1 << other.gameObject.layer)) != 0)
            {
                triggered = false;
            }
        }
    }
}

