using UnityEngine;
using System.Collections.Generic;
using Tobii.XR;
using System.IO;

public class GazeRaycaster : MonoBehaviour
{

    [Header("Raycast")]
    public float maxDistance = 200f;
    public LayerMask gazeMask = ~0;        // 기본 Everything

    public static List<UserStatus> userStatus = new List<UserStatus>();
    public ClickableObject currentObject = null;

    void Update()
    {
        // TobiiXR로부터 시선 정보 받아오기
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        // 시선이 유효하지 않으면 무시
        if (!eyeData.GazeRay.IsValid)
        {
            ExitCurrentObject();
            return;
        }

        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);
        
        // 디버그용 레이 표시
        Debug.DrawRay(gazeRay.origin, gazeRay.direction * maxDistance, Color.green);

        // 시선으로 Raycast 쏘기
        if (Physics.Raycast(gazeRay, out RaycastHit hit, 100f))
        {
            ClickableObject hitObject = hit.collider.GetComponent<ClickableObject>();

            if (hitObject != null)
            {
                if (hitObject != currentObject)
                {
                    ExitCurrentObject();
                    currentObject = hitObject;
                }

                // 개인 로컬에서 돌리면 컴퓨터가 터질수도 있기 때문에 주석 처리
                //userStatus.Add(new UserStatus(GameManager.Instance.getFrameTime(), Status.LOCKED, currentObject.GetObjectTypeAsString()));
                currentObject.OnGazeEnter(); // 시선이 닿았다고 알림
            }
            else
            {
                //userStatus.Add(new UserStatus(GameManager.Instance.getFrameTime(), Status.NOT_LOCKED, ""));
                ExitCurrentObject();
            }
        }
        else
        {
            //userStatus.Add(new UserStatus(GameManager.Instance.getFrameTime(), Status.NOT_LOCKED, ""));
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
    public enum Status { GAZE, NOT_GAZE, DESTROY }

    [System.Serializable]
    public class UserStatusListWrapper
    {
        public List<UserStatus> behavior_series;
    }
}
