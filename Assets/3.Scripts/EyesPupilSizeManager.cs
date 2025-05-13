using UnityEngine;
using System.Collections;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;
using System.Collections.Generic;

public class EyesPupilSizeManager : MonoBehaviour
{
    private List<EyeData> eyeDatas;
    private Coroutine measureCoroutine;

    void Start()
    {
        eyeDatas = new List<EyeData>();
        StartMeasuring();
    }

    IEnumerator MeasurEyePupilSize()
    {
        WaitForSeconds interval = new WaitForSeconds(1.0f);

        while (true)
        {
            if (XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupils) && pupils != null && pupils.Length >= 2)
            {
                var left = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
                var right = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

                if (left.isDiameterValid && right.isDiameterValid)
                {
                    eyeDatas.Add(new EyeData
                    {
                        timestamp = GameManager.Instance.getFrameTime(),
                        leftPupilSize = left.pupilDiameter * 1000f,
                        rightPupilSize = right.pupilDiameter * 1000f
                    });
                }
            }

            yield return interval;
        }
    }

    public void StartMeasuring()
    {
        if (measureCoroutine == null)
        {
            measureCoroutine = StartCoroutine(MeasurEyePupilSize());
        }
    }

    public void StopMeasuring()
    {
        if (measureCoroutine != null)
        {
            StopCoroutine(measureCoroutine);
            measureCoroutine = null;
        }
    }

    public void ResetManager()
    {
        eyeDatas = new List<EyeData>();
    }

    public List<EyeData> GetEyeDatas()
    {
        return eyeDatas;
    }

    [System.Serializable]
    public class EyeData
    {
        public float timestamp;
        public float leftPupilSize;
        public float rightPupilSize;
    }
}