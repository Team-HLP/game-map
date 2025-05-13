using UnityEngine;
using Tobii.XR;

public class GazeVisualizerDebugger : MonoBehaviour
{
    SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) Debug.LogError("[GazeVizDebug] SpriteRenderer가 없음!");
    }

    void Update()
    {
        // 1) TobiiXR로부터 매 프레임 GazeRay 가져오기
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        var gazeRay = eyeData.GazeRay;

        // 2) 유효성, origin, direction 찍기
        Debug.Log($"[GazeVizDebug] IsValid={gazeRay.IsValid} | " +
                  $"Origin={gazeRay.Origin:F2} | Dir={gazeRay.Direction:F2} | " +
                  $"Renderer.enabled={_sr.enabled} | Pos={transform.position:F2}");

        // (원한다면) 화면에 Ray도 그려볼 수 있습니다.
        if (gazeRay.IsValid)
            Debug.DrawRay(gazeRay.Origin, gazeRay.Direction * 5f, Color.cyan);
    }
}
