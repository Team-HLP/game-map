using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.CameraSystem
{
    /// <summary>
    /// Unity Event for functions with a Camera View Target parameter.
    /// </summary>
    [System.Serializable]
    public class CameraViewTargetEventHandler : UnityEvent<CameraViewTarget> { }
}
