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
    private EEGSensorID[] sensorIDs = new EEGSensorID[]
    {
        EEGSensorID.AF3, EEGSensorID.AF4,
        EEGSensorID.Fp1, EEGSensorID.Fp2,
        EEGSensorID.AF7, EEGSensorID.AF8
    };

    private Coroutine measureCoroutine;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eeg_data.json");
        collectedEEG = new EEGDataWrapper();
        LooxidLinkManager.Instance.Initialize();
        StartCoroutine(MeasureEEGData());
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
                    delta = AverageBand(data, sensorIDs, BandType.Delta),
                    theta = AverageBand(data, sensorIDs, BandType.Theta),
                    alpha = AverageBand(data, sensorIDs, BandType.Alpha),
                    beta = AverageBand(data, sensorIDs, BandType.Beta),
                    gamma = AverageBand(data, sensorIDs, BandType.Gamma)
                };
                Debug.Log($"EEG Entry - Time: {entry.time_stamp}, Delta: {entry.delta}, Theta: {entry.theta}, Alpha: {entry.alpha}, Beta: {entry.beta}, Gamma: {entry.gamma}");

                collectedEEG.eeg_data.Add(entry);
            }

            yield return interval;
        }
    }

    double AverageBand(EEGFeatureIndex index, EEGSensorID[] ids, BandType band)
    {
        double sum = 0.0;
        foreach (var id in ids)
        {
            sum += band switch
            {
                BandType.Delta => index.Delta(id),
                BandType.Theta => index.Theta(id),
                BandType.Alpha => index.Alpha(id),
                BandType.Beta => index.Beta(id),
                BandType.Gamma => index.Gamma(id),
                _ => 0.0
            };
        }
        return sum / ids.Length;
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
        Debug.Log("EEG JSON 저장 완료: " + filePath);
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
