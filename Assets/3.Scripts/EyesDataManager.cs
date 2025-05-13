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

    public void SaveEyesData()
    {
        eyesBlinkCountManager.StopMeasuring();
        eyesPupilSizeManager.StopMeasuring();

        int eyeBlinkCount = eyesBlinkCountManager.GetEyeBlinkCount();
        var eyeDatas = eyesPupilSizeManager.GetEyeDatas();
        float leftAvg = PlayerPrefs.GetFloat("left_avg", 0f);
        float rightAvg = PlayerPrefs.GetFloat("right_avg", 0f);

        List<PupilRecord> pupilRecords = new List<PupilRecord>();
        foreach (var data in eyeDatas)
        {
            pupilRecords.Add(new PupilRecord
            {
                time_stamp = data.timestamp,
                pupil_size = new PupilSize
                {
                    left = data.leftPupilSize,
                    right = data.rightPupilSize
                }
            });
        }

        EyesData eyesData = new EyesData
        {
            blink_eye_count = eyeBlinkCount,
            pupil_records = pupilRecords.ToArray(),
            base_pupil_size = new PupilSize
            {
                left = leftAvg,
                right = rightAvg
            }
        };

        string json = JsonUtility.ToJson(eyesData, true);
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Debug.Log("파일 저장 경로 : " + filePath);
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
    private class EyesData
    {
        public int blink_eye_count;
        public PupilRecord[] pupil_records;
        public PupilSize base_pupil_size;
    }

    [System.Serializable]
    private class PupilRecord
    {
        public float time_stamp;
        public PupilSize pupil_size;
    }

    [System.Serializable]
    private class PupilSize
    {
        public float left;
        public float right;
    }
}
