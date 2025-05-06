using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Looxid.Link;
using System;

public class EEGDataManager : MonoBehaviour
{
    enum BandType { Delta, Theta, Alpha, Beta, Gamma }
    private string filePath;

    private EEGDataWrapper collectedEEG;
    private EEGSensorID sensorID = EEGSensorID.Fp1;  // Fp1만 사용

    private Coroutine measureCoroutine;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eeg_data.json");
        collectedEEG = new EEGDataWrapper();
        LooxidLinkManager.Instance.Initialize();
        StartMeasuring();
    }

    IEnumerator MeasureEEGData()
    {
        WaitForSeconds interval = new WaitForSeconds(1.0f);

        while (true)
        {
            List<EEGFeatureIndex> recentData = LooxidLinkData.Instance.GetEEGFeatureIndexData(1.0f);

            foreach (var data in recentData)
            {
                EEGEntry entry = new EEGEntry
                {
                    time_stamp = data.timestamp,
                    delta = ConvertLogToLinear(data.Delta(sensorID)),
                    theta = ConvertLogToLinear(data.Theta(sensorID)),
                    alpha = ConvertLogToLinear(data.Alpha(sensorID)),
                    beta = ConvertLogToLinear(data.Beta(sensorID)),
                    gamma = ConvertLogToLinear(data.Gamma(sensorID))
                };

                Debug.Log($"[EEG] Time: {entry.time_stamp}, Delta: {entry.delta}, Theta: {entry.theta}, Alpha: {entry.alpha}, Beta: {entry.beta}, Gamma: {entry.gamma}");

                collectedEEG.eeg_data.Add(entry);
            }

            yield return interval;
        }
    }

    // 로그 → 선형 변환 + NaN 처리 (필터링 없이 그대로 사용)
    double ConvertLogToLinear(double value)
    {
        // NaN, Infinity → 0 처리
        if (double.IsNaN(value) || double.IsInfinity(value))
            return 0.0;

        // 로그 → 선형 변환
        return Math.Pow(10, value);
    }

    public void StartMeasuring()
    {
        if (measureCoroutine == null)
        {
            measureCoroutine = StartCoroutine(MeasureEEGData());
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
        collectedEEG = new EEGDataWrapper();
    }

    public void ReMeasuring()
    {
        StartMeasuring();
    }

    public void SaveEEGData()
    {
        StopMeasuring();
        string json = JsonUtility.ToJson(collectedEEG, true);
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Debug.Log("[EEG] JSON 저장 완료: " + filePath);
    }

    [Serializable]
    public class EEGEntry
    {
        public double time_stamp;
        public double delta;
        public double theta;
        public double alpha;
        public double beta;
        public double gamma;
    }

    [Serializable]
    public class EEGDataWrapper
    {
        public List<EEGEntry> eeg_data = new List<EEGEntry>();
    }
}
