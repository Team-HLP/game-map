using UnityEngine;
using Tobii.XR;
using UnityEngine.XR;
using System.Collections;

public class TobiiXrDebugLogger : MonoBehaviour
{
    public string playerTag = "Player";

    private Transform playerCamera;

    IEnumerator Start()
    {
        Debug.Log("[DEBUG] XR & Tobii 디버깅 코루틴 시작");

        #if UNITY_EDITOR
                bool simulateXR = true;
        #else
            bool simulateXR = false;
        #endif

        // XR 디바이스가 활성화될 때까지 대기
        while (!XRSettings.isDeviceActive && !simulateXR)
        {
            Debug.Log("[DEBUG] XR 디바이스 대기 중...");
            yield return null;
        }

        Debug.Log("[DEBUG] XR 디바이스 활성화됨: " + XRSettings.loadedDeviceName);

        // Player 오브젝트가 스폰될 때까지 대기
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
            yield return null;
        }

        Debug.Log("[DEBUG] Player 오브젝트가 등장함: " + player.name);

        // Main Camera 찾기
        Camera cam = player.GetComponentInChildren<Camera>();
        if (cam != null)
        {
            playerCamera = cam.transform;
            Debug.Log("[DEBUG] Main Camera 연결됨: " + cam.name);
        }
        else
        {
            Debug.LogWarning("[DEBUG] Player 하위에 Camera 없음");
        }

        // 디버깅 시작
        StartCoroutine(LogGaze());
    }

    IEnumerator LogGaze()
    {
        while (true)
        {
            var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);


            if (eyeData.GazeRay.IsValid)
            {
                Debug.DrawRay(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction * 10f, Color.green);
                Debug.Log("[DEBUG] Gaze Origin: " + eyeData.GazeRay.Origin);
                Debug.Log("[DEBUG] Gaze Direction: " + eyeData.GazeRay.Direction);

            }
            else
            {
                Debug.LogWarning("[DEBUG] Gaze 감지 불가 (유효하지 않음)");
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
