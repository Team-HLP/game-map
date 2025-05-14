using UnityEngine;
using System.Collections;
using Tobii.XR;

public class EyesBlinkCountManager2 : MonoBehaviour
{
    private int blinkCount;
    private bool wasBlinking;
    private Coroutine measureCoroutine;

    void Start()
    {
        blinkCount = 0;
        wasBlinking = false;
        StartMeasuring();
    }

    IEnumerator MeasurEyeBlinkCount()
    {
        WaitForSeconds interval = new WaitForSeconds(0.0f);

        while (true)
        {
            var tobiiData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
            bool isBlinking = tobiiData.IsLeftEyeBlinking && tobiiData.IsRightEyeBlinking;

            if (!wasBlinking && isBlinking)
            {
                blinkCount++;
            }

            wasBlinking = isBlinking;

            yield return interval;
        }
    }

    public void StartMeasuring()
    {
        if (measureCoroutine == null)
        {
            measureCoroutine = StartCoroutine(MeasurEyeBlinkCount());
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
        blinkCount = 0;
        wasBlinking = false;
    }

    public int GetEyeBlinkCount()
    {
        return blinkCount;
    }
}