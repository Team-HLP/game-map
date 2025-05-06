using UnityEngine;
using Tobii.XR;

public class GazeDetection : MonoBehaviour
{
    public DotVisual currentObject;

    void Update()
    {
        // TobiiXR로부터 시선 정보 받아오기
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        // 시선이 유효하지 않으면 무시
        if (!eyeData.GazeRay.IsValid) {
            return;
        }

        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

        // 시선으로 Raycast 쏘기
        if (Physics.Raycast(gazeRay, out RaycastHit hit, 100f))
        {
            DotVisual hitObject = hit.collider.GetComponent<DotVisual>();

            if (hitObject != null)
            {
                if (hitObject != currentObject)
                {
                    currentObject = hitObject;
                }

                currentObject.OnGazeEnter();
            }
            else
            {
                
            }
        }
        else
        {

        }
    }
}
