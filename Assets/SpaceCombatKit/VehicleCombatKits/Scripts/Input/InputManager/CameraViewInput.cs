using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Controls.UnityInputManager;

namespace VSX.CameraSystem
{
    /// <summary>
    /// Provides an inspector setting for selecting a camera view with a specified input.
    /// </summary>
    [System.Serializable]
    public class CameraViewInput
    {
        public CameraView view;
        public CustomInput input;

        public CameraViewInput(CameraView view, CustomInput input)
        {
            this.view = view;
            this.input = input;
        }
    }
}
