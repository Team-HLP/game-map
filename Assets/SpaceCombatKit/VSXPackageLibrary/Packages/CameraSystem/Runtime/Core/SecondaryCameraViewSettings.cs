using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.CameraSystem
{
    /// <summary>
    /// This class contains the settings for a secondary camera (a camera that must follow certain aspects of the main camera).
    /// </summary>
    [System.Serializable]
    public class SecondaryCameraViewSettings
    {
        [Tooltip("The camera view that these settings will be applied to.")]
        public CameraView view;

        [Tooltip("Whether to copy the field of view of the main camera.")]
        public bool copyFieldOfView;

        [Tooltip("Whether to copy the rotation of the main camera.")]
        public bool copyRotation;
    }
}
