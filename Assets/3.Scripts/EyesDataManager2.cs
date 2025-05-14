using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class EyesDataManager2 : MonoBehaviour
{
    private string filePath;
    public EyesPupilSizeManager2 eyesPupilSizeManager2;
    public EyesBlinkCountManager2 eyesBlinkCountManager2;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eye_data.json");
    }

    public void SaveEyesData()
    {
        eyesBlinkCountManager2.StopMeasuring();
        eyesPupilSizeManager2.StopMeasuring();

        int eyeBlinkCount = eyesBlinkCountManager2.GetEyeBlinkCount();
        var eyeDatas = eyesPupilSizeManager2.GetEyeDatas();
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
        eyesPupilSizeManager2.ResetManager();
        eyesBlinkCountManager2.ResetManager();
    }

    public void ReMeasuring()
    {
        eyesBlinkCountManager2.StartMeasuring();
        eyesPupilSizeManager2.StartMeasuring();
    }

    public void ImmeditelyEyePupilDataSave()
    {
        eyesPupilSizeManager2.ImmeditelyEyePupilDataSave();
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