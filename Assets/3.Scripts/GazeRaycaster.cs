using UnityEngine;
using System.Collections.Generic;
using Tobii.XR;
using System.IO;

public class GazeRaycaster : MonoBehaviour
{
    public static List<UserStatus> userStatus = new List<UserStatus>();
    public ClickableObject currentObject = null;

    // 최대 사정거리
    private float maxRayDistance = 100f;

    void Update()
    {
        // TobiiXR로부터 시선 정보 받아오기
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (!eyeData.GazeRay.IsValid)
        {
            ExitCurrentObject();
            return;
        }

        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

        // Raycast 결과에 따라 색상 및 디버그 시각화
        if (Physics.Raycast(gazeRay, out RaycastHit hit, maxRayDistance))
        {
            // 맞은 거리만큼 초록색으로
            Debug.DrawRay(gazeRay.origin, gazeRay.direction * hit.distance, Color.green);
            // 히트 지점에 작은 노란 구체 표시
            Debug.DrawLine(hit.point + Vector3.up * 0.1f,
                          hit.point - Vector3.up * 0.1f, Color.yellow);
            Debug.DrawLine(hit.point + Vector3.right * 0.1f,
                          hit.point - Vector3.right * 0.1f, Color.yellow);

            Debug.Log($"[GazeRaycaster] Ray hit object: {hit.collider.gameObject.name}");

            var hitObject = hit.collider.GetComponent<ClickableObject>();
            if (hitObject != null)
            {
                if (hitObject != currentObject)
                {
                    ExitCurrentObject();
                    currentObject = hitObject;
                }
                currentObject.OnGazeEnter();
            }
            else
            {
                ExitCurrentObject();
            }
        }
        else
        {
            // 아무것도 못 맞추면 레이 전체를 빨간색으로
            Debug.DrawRay(gazeRay.origin, gazeRay.direction * maxRayDistance, Color.red);
            ExitCurrentObject();
        }
    }


    void ExitCurrentObject()
    {
        if (currentObject != null)
        {
            currentObject.OnGazeExit(); // 응시 종료 알림 추가
            currentObject = null;
        }
    }

    public static void SaveUserDestoryStatus(string object_name)
    {
        userStatus.Add(new UserStatus(Time.time, Status.DESTROY, object_name));
    }

    public static void SaveUserStatusToJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "behavior_series.json");

        UserStatusListWrapper wrapper = new UserStatusListWrapper();
        wrapper.behavior_series = userStatus;

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(filePath, json);
        Debug.Log("User status saved to: " + filePath);
        userStatus = new List<UserStatus>();
    }

    [System.Serializable]
    public class UserStatus
    {
        public float time_stamp;
        public string status;
        public string object_name;

        public UserStatus(float time_stamp, Status status, string object_name)
        {
            this.time_stamp = time_stamp;
            this.status = status.ToString();
            this.object_name = object_name;
        }
    }

    [System.Serializable]
    public enum Status { LOCKED, NOT_LOCKED, DESTROY }

    [System.Serializable]
    public class UserStatusListWrapper
    {
        public List<UserStatus> behavior_series;
    }
}
