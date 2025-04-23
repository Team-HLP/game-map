using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    public class Condition : MonoBehaviour
    {
        public virtual bool Value()
        {
            return false;
        }
    }
}

