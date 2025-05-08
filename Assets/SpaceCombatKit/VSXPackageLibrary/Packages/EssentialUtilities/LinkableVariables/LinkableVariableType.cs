using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// The type of a linkable variable so that it can be identified efficiently.
    /// </summary>
    public enum LinkableVariableType
    {
        Object,
        Bool,
        Int,
        Float,
        String,
        Vector3        
    }
}
