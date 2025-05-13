// 파일이름: wak_GazeRaycaster.cs
using UnityEngine;
using Tobii.XR;

/// <summary>
/// Tobii XR 시선 정보를 받아서 가장 먼저 맞은 wak_ClickableObject에
/// OnGazeEnter / OnGazeExit를 전달한다.
/// </summary>
public class wak_GazeRaycaster : MonoBehaviour
{
    /// 현재 시선이 머무는 객체
    public wak_ClickableObject currentObject = null;

    void Update ()
    {
        /* 1) 시선 데이터 가져오기 */
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        if (!eyeData.GazeRay.IsValid)
        {
            ExitCurrentObject();
            return;
        }

        /* 2) Raycast */
        Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

        if (Physics.Raycast(gazeRay, out RaycastHit hit, 100f))
        {
            wak_ClickableObject hitObject = hit.collider.GetComponent<wak_ClickableObject>();

            if (hitObject != null)
            {
                /* 새 오브젝트로 이동했는가? */
                if (hitObject != currentObject)
                {
                    ExitCurrentObject();
                    currentObject = hitObject;
                    Debug.Log(currentObject);
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
            ExitCurrentObject();
        }
    }

    /* ──────────────────────────────── 내부 메서드 ─────────────────────────────── */
    void ExitCurrentObject ()
    {
        if (currentObject != null)
        {
            currentObject.OnGazeExit();
            currentObject = null;
        }
    }
}
