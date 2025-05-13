using UnityEngine;
using System.Collections.Generic;
using Tobii.XR;
using System.IO;

public class GazeRaycaster : MonoBehaviour
{
    public static List<UserStatus> userStatus = new List<UserStatus>();
    public ClickableObject currentObject = null;

    // 최대 사정거리
    private float maxRayDistance = 1000f;

    // 추가: 감지할 레이어 정의
    public LayerMask gazeTargetLayer;

    void Update()
    {
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (!eyeData.GazeRay.IsValid)
        {
            ExitCurrentObject();
            return;
        }

        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

        // 지정된 레이어만 감지
        if (Physics.Raycast(gazeRay, out RaycastHit hit, maxRayDistance, gazeTargetLayer))
        {
            Debug.DrawRay(gazeRay.origin, gazeRay.direction * hit.distance, Color.green);

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
