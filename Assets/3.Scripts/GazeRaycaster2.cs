using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Tobii.XR;
using System.IO;

public class GazeRaycaster2 : MonoBehaviour
{
    private static List<UserStatus> userStatus = new List<UserStatus>();
    public ClickableObject2 currentObject = null;
    private bool isLookingAtObject = false;
    private string currentObjectType = "";
    private Coroutine coroutine;

    [Header("Gaze Settings")]
    public LayerMask gazeTargetLayer;  // GazeTarget 레이어 마스크

    void Start()
    {
        coroutine = StartCoroutine(LogUserStatusCoroutine());
    }

    void Update()
    {
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        if (!eyeData.GazeRay.IsValid)
        {
            isLookingAtObject = false;
            ExitCurrentObject();
            return;
        }

        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

        // 지정된 GazeTarget 레이어만 감지
        if (Physics.Raycast(gazeRay, out RaycastHit hit, 100f, gazeTargetLayer))
        {
            ClickableObject2 hitObject = hit.collider.GetComponent<ClickableObject2>();

            if (hitObject != null)
            {
                if (hitObject != currentObject)
                {
                    ExitCurrentObject();
                    currentObject = hitObject;
                }

                currentObject.OnGazeEnter();
                isLookingAtObject = true;
                currentObjectType = currentObject.GetObjectTypeAsString();
            }
            else
            {
                isLookingAtObject = false;
                currentObjectType = "";
                ExitCurrentObject();
            }
        }
        else
        {
            isLookingAtObject = false;
            currentObjectType = "";
            ExitCurrentObject();
        }
    }

    void ExitCurrentObject()
    {
        if (currentObject != null)
        {
            currentObject.OnGazeExit();
            currentObject = null;
        }
    }

    IEnumerator LogUserStatusCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (isLookingAtObject)
            {
                userStatus.Add(new UserStatus(GameManager2.Instance.getFrameTime(), Status.GAZE, currentObjectType));
            }
            else
            {
                userStatus.Add(new UserStatus(GameManager2.Instance.getFrameTime(), Status.NOT_GAZE, ""));
            }
        }
    }

    public static void SaveUserDestoryStatus(string object_name)
    {
        userStatus.Add(new UserStatus(GameManager2.Instance.getFrameTime(), Status.USER_DESTROY, object_name));
    }

    public static void SaveAutoDestoryStatus(string object_name)
    {
        userStatus.Add(new UserStatus(GameManager2.Instance.getFrameTime(), Status.AUTO_DESTROY, object_name));
    }

    public void SaveUserStatusToJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "behavior_data.json");

        UserStatusListWrapper wrapper = new UserStatusListWrapper();
        wrapper.behavior_data = userStatus;

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(filePath, json);
        userStatus = new List<UserStatus>();

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    [System.Serializable]
    public enum Status { GAZE, NOT_GAZE, USER_DESTROY, AUTO_DESTROY }

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
    public class UserStatusListWrapper
    {
        public List<UserStatus> behavior_data;
    }
}
