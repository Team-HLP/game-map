using UnityEngine;
using System.Collections;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;
using Tobii.XR;
using System.IO;

public class EyesDataManager : MonoBehaviour
{
    private float leftPupilSum = 0f;
    private float rightPupilSum = 0f;
    private int leftPupilCount = 0;
    private int rightPupilCount = 0;

    private int blinkCount = 0;
    private bool wasBlinking = false;

    private StreamWriter writer;
    private string filePath;
    private bool isFirstEntry = true;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eye_data.json");
        writer = new StreamWriter(filePath, false);
        writer.WriteLine("[");
        StartCoroutine(TrackAndSaveData());
    }

    IEnumerator TrackAndSaveData()
    {
        WaitForSeconds shortInterval = new WaitForSeconds(0.1f);

        while (true)
        {
            // 1초 동안 0.1초 간격으로 누적
            for (int i = 0; i < 10; i++)
            {
                // 동공 크기
                if (XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupils) && pupils != null && pupils.Length >= 2)
                {
                    var left = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
                    var right = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

                    if (left.isDiameterValid && right.isDiameterValid)
                    {
                        leftPupilSum += left.pupilDiameter;
                        rightPupilSum += right.pupilDiameter;
                        leftPupilCount++;
                        rightPupilCount++;
                    }
                }

                // 눈 깜빡임
                var tobiiData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
                bool isBlinking = tobiiData.IsLeftEyeBlinking && tobiiData.IsRightEyeBlinking;

                if (!wasBlinking && isBlinking) {
                    blinkCount++;
                }

                wasBlinking = isBlinking;

                yield return shortInterval;
            }

            float leftAvg = leftPupilCount > 0 ? leftPupilSum / leftPupilCount * 1000f : 0f;
            float rightAvg = rightPupilCount > 0 ? rightPupilSum / rightPupilCount * 1000f : 0f;

            EyeData entry = new EyeData
            {
                timestamp = Time.time,
                leftPupilAvg = leftAvg,
                rightPupilAvg = rightAvg,
                blinkCount = blinkCount
            };

            string json = JsonUtility.ToJson(entry);
            if (isFirstEntry)
            {
                writer.WriteLine(json);
                isFirstEntry = false;
            }
            else
            {
                writer.WriteLine("," + json);
            }

            leftPupilSum = rightPupilSum = 0f;
            leftPupilCount = rightPupilCount = 0;
        }
    }

    // TODO. 종료 조건 물어보고 로직 수정하기
    public void SaveEyesData()
    {
        writer.WriteLine("]");
        writer.Flush();
        writer.Close();

        Debug.Log("파일 저장 완료: " + filePath);
    }

    [System.Serializable]
    public class EyeData
    {
        public float timestamp;
        public float leftPupilAvg;
        public float rightPupilAvg;
        public int blinkCount;
    }
}
