using UnityEngine;
using Tobii.XR;

public class GazeRaycaster : MonoBehaviour
{
    public ClickableObject currentObject = null;

    void Update()
    {
        // TobiiXR로부터 시선 정보 받아오기
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        // 시선이 유효하지 않으면 무시
        if (!eyeData.GazeRay.IsValid) {
            ExitCurrentObject();
            return;
        }

        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

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

                currentObject.OnGazeEnter(); // 시선이 닿았다고 알림
            }
            else
            {
                ExitCurrentObject();
            }
        }
        else
        {
            ExitCurrentObject();
        }
    }

    void ExitCurrentObject()
    {
        if (currentObject != null)
        {
            currentObject = null;
        }
    }
}
