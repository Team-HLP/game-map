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
        Debug.Log("[DEBUG] XR & Tobii ����� �ڷ�ƾ ����");

        #if UNITY_EDITOR
                bool simulateXR = true;
        #else
            bool simulateXR = false;
        #endif

        // XR ����̽��� Ȱ��ȭ�� ������ ���
        while (!XRSettings.isDeviceActive && !simulateXR)
        {
            Debug.Log("[DEBUG] XR ����̽� ��� ��...");
            yield return null;
        }

        Debug.Log("[DEBUG] XR ����̽� Ȱ��ȭ��: " + XRSettings.loadedDeviceName);

        // Player ������Ʈ�� ������ ������ ���
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
            yield return null;
        }

        Debug.Log("[DEBUG] Player ������Ʈ�� ������: " + player.name);

        // Main Camera ã��
        Camera cam = player.GetComponentInChildren<Camera>();
        if (cam != null)
        {
            playerCamera = cam.transform;
            Debug.Log("[DEBUG] Main Camera �����: " + cam.name);
        }
        else
        {
            Debug.LogWarning("[DEBUG] Player ������ Camera ����");
        }

        // ����� ����
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
                Debug.LogWarning("[DEBUG] Gaze ���� �Ұ� (��ȿ���� ����)");
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
