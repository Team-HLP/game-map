using UnityEngine;
using Tobii.XR;

[RequireComponent(typeof(LineRenderer))]
public class GazeRayLineRenderer : MonoBehaviour
{
    private LineRenderer _lr;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
        _lr.startWidth = 0.005f;
        _lr.endWidth   = 0.005f;
        _lr.material   = new Material(Shader.Find("Unlit/Color"));
        _lr.material.color = Color.red;
    }

    void Update()
    {
        var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (eyeData.GazeRay.IsValid)
        {
            Vector3 o = eyeData.GazeRay.Origin;
            Vector3 d = eyeData.GazeRay.Direction.normalized * 50f;

            _lr.enabled = true;
            _lr.SetPosition(0, o);
            _lr.SetPosition(1, o + d);
        }
        else
        {
            _lr.enabled = false;
        }
    }
}
