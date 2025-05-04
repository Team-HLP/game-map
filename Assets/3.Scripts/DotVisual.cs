using UnityEngine;
using System.Collections;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;
using System.Collections.Generic;

public class DotVisual : MonoBehaviour
{
    private List<EyePupilData> eyePupilDatas;
    private DotSpawner spawner;

    private Renderer _renderer;
    private Color _originalColor;
    private Color _targetColor;

    private Color highlightColor = Color.red;
    private float highlightLerpSpeed = 10f;

    private float gazeActiveUntil = 0f;
    private float gazeTimer = 0f;
    private float continuousGazeTime = 5f;

    private bool hasCompleted = false;

    private float pupilSampleInterval = 1f;
    private float nextSampleTime = 0f;

    public void Init(DotSpawner parentSpawner)
    {
        eyePupilDatas = new List<EyePupilData>();
        spawner = parentSpawner;

        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
            _targetColor = _originalColor;
        }
    }

    void Update()
    {
        if (hasCompleted) return;

        bool isGazedNow = Time.time <= gazeActiveUntil;

        if (isGazedNow)
        {
            spawner.DisActiveRetryFocusNoticeText();
            gazeTimer += Time.deltaTime;
            _targetColor = highlightColor;

            if (Time.time >= nextSampleTime)
            {
                SamplePupilSize();
                nextSampleTime = Time.time + pupilSampleInterval;
            }

            if (gazeTimer >= continuousGazeTime)
            {
                OnGazeComplete();
            }
        }
        else
        {
            gazeTimer = 0f;
            eyePupilDatas.Clear();
            nextSampleTime = Time.time + pupilSampleInterval;
            _targetColor = _originalColor;
            spawner.ActiveRetryFocusNoticeText();
        }

        if (_renderer != null)
        {
            if (_renderer.material.HasProperty("_BaseColor"))
            {
                _renderer.material.SetColor("_BaseColor",
                    Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor,
                               Time.deltaTime * highlightLerpSpeed));
            }
            else
            {
                _renderer.material.color =
                    Color.Lerp(_renderer.material.color, _targetColor,
                               Time.deltaTime * highlightLerpSpeed);
            }
        }
    }

    public void OnGazeEnter()
    {
        gazeActiveUntil = Time.time + 0.1f;
    }

    private void OnGazeComplete()
    {
        if (hasCompleted) return;
        hasCompleted = true;

        spawner.OnDotCompleted();
        EyePupilDataManager.Instance.SaveEyePupilData(eyePupilDatas);
        Destroy(gameObject);
    }

    private void SamplePupilSize()
    {
        if (XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupils) &&
            pupils != null && pupils.Length >= 2)
        {
            var left = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var right = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

            if (left.isDiameterValid && right.isDiameterValid)
            {
                eyePupilDatas.Add(new EyePupilData(left.pupilDiameter, right.pupilDiameter));
            }
        }
    }
}
