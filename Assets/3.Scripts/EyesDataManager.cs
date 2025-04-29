using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class EyesDataManager : MonoBehaviour
{
    private string filePath;
    public EyesPupilSizeManager eyesPupilSizeManager;
    public EyesBlinkCountManager eyesBlinkCountManager;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eye_data.json");
    }

    // TODO. 기준 동공 크기 구현되면 추가하기
    public void SaveEyesData()
    {
        eyesBlinkCountManager.StopMeasuring();
        eyesPupilSizeManager.StopMeasuring();

        int eyeBlinkCount = eyesBlinkCountManager.GetEyeBlinkCount();
        var eyeDatas = eyesPupilSizeManager.GetEyeDatas();

        var pupilRecords = new List<object>();
        foreach (var data in eyeDatas)
        {
            pupilRecords.Add(new
            {
                time_stamp = data.timestamp,
                pupil_size = new
                {
                    left = data.leftPupilSize,
                    right = data.rightPupilSize
                }
            });
        }

        var finalJsonObject = new
        {
            blink_eye_count = eyeBlinkCount,
            pupil_records = pupilRecords
        };

        string json = JsonUtility.ToJson(new Wrapper(finalJsonObject), true);
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Debug.Log("파일 저장 경로 :  " + filePath);
    }

    public void ResetManager()
    {
        eyesPupilSizeManager.ResetManager();
        eyesBlinkCountManager.ResetManager();
    }

    public void ReMeasuring()
    {
        eyesBlinkCountManager.StartMeasuring();
        eyesPupilSizeManager.StartMeasuring();
    }

    [System.Serializable]
    private class Wrapper
    {
        public object eyes_data;

        public Wrapper(object data)
        {
            this.eyes_data = data;
        }
    }
}