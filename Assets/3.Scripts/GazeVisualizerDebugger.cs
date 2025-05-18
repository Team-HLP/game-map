using UnityEngine;
using Tobii.XR;

public class GazeVisualizerDebugger : MonoBehaviour
{
    SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1) TobiiXR로부터 매 프레임 GazeRay 가져오기
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        var gazeRay = eyeData.GazeRay;

        // 2) 유효성, origin, direction 찍기

        // (원한다면) 화면에 Ray도 그려볼 수 있습니다.
    }
}
