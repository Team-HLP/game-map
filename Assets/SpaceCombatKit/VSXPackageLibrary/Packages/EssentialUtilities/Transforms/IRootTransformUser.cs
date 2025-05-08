using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    public interface IRootTransformUser
    {
        Transform RootTransform{ set; }
    }
}