using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Looxid.Link;
using System;

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

public class EEGDataManager : MonoBehaviour
{
    private float collectDuration = 90f;
    private EEGDataWrapper collectedEEG = new EEGDataWrapper();
    private EEGSensorID[] sensorIDs = new EEGSensorID[]
    {
        EEGSensorID.AF3, EEGSensorID.AF4,
        EEGSensorID.Fp1, EEGSensorID.Fp2,
        EEGSensorID.AF7, EEGSensorID.AF8
    };

    void Start()
    {
        StartCoroutine(CollectEEGCoroutine());
    }

    IEnumerator CollectEEGCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < collectDuration)
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
                collectedEEG.eeg_data.Add(entry);
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        SaveToJson(collectedEEG);
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

    void SaveToJson(EEGDataWrapper data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "eeg_data.json");
        File.WriteAllText(path, json, Encoding.UTF8);
        Debug.Log("EEG JSON 저장 완료: " + path);
    }

    enum BandType { Delta, Theta, Alpha, Beta, Gamma }
}
