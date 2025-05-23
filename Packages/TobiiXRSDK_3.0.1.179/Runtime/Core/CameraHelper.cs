﻿//Copyright © 2018 – Property of Tobii AB(publ) - All Rights Reserved

using UnityEngine;

namespace Tobii.XR
{
    /// <summary>
    /// Helper class to find the main camera, cache it and invalidate the cache when the camera
    /// is destroyed or disabled.
    /// </summary>
    public static class CameraHelper
    {
        private static Camera _cachedCamera;

        public static Camera GetMainCamera()
        {
            if (_cachedCamera == null || _cachedCamera.gameObject.activeInHierarchy == false)
            {
                _cachedCamera = Internal_GetMainCamera();
            }

            return _cachedCamera;
        }

        public static Transform GetCameraTransform()
        {
            var camera = GetMainCamera();
            return camera != null ? camera.transform : null;
        }

        private static Camera Internal_GetMainCamera()
        {
            var mainCameras = GameObject.FindGameObjectsWithTag("MainCamera");

            if (mainCameras.Length > 1)
            {
                Debug.LogWarning("There are " + mainCameras.Length +
                                 " main cameras in the scene. Please ensure there is always exactly one main camera in the scene.");
            }

            if (mainCameras.Length > 0)
            {
                var camera = mainCameras[0].GetComponent<Camera>();
                if (camera != null && camera.gameObject.activeInHierarchy)
                {
                    return camera;
                }
            }

            if (Camera.allCameras.Length > 1)
            {
                Debug.LogWarning("No main camera found in scene. There are " + Camera.allCameras.Length +
                                   " other cameras in the scene, using the first camera found.");
            }

            foreach (var camera in Camera.allCameras)
            {
                if (camera.gameObject.activeInHierarchy)
                {
                    return camera;
                }
            }

            // Debug.LogError("No active camera found in scene. Add a camera before running TobiiXR.");
            return null;
        }
    }
}