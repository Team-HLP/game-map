using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Looxid.Link;
using System;

public class EEGDataManager2 : MonoBehaviour
{
    private const int linkFequency = 5;
    enum BandType { DELTA, THETA, ALPHA, BETA, GAMMA }
    private string filePath;
    private EEGDataWrapper collectedEEG;
    private EEGSensorID sensorID = EEGSensorID.Fp1;
    private EEGSensor sensorStatusData;
    private Coroutine measureCoroutine;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eeg_data.json");
        collectedEEG = new EEGDataWrapper();

        LooxidLinkManager.Instance.Initialize();
        LooxidLinkData.OnReceiveEEGSensorStatus += OnReceiveEEGSensorStatus;
        LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
        LooxidLinkManager.OnShowNoiseSignalMessage += () => { };
        LooxidLinkManager.OnHideNoiseSignalMessage += () => { };

        StartMeasuring();
    }

    void OnDestroy()
    {
        LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceiveEEGSensorStatus;
        LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
    }

    void OnReceiveEEGSensorStatus(EEGSensor status)
    {
        sensorStatusData = status;
    }

    void OnReceiveEEGRawSignals(EEGRawSignal raw)
    {
        double[] channelData = raw.FilteredRawSignal(sensorID);
        if (channelData != null && channelData.Length > 0)
        {
            double sample = channelData[channelData.Length - 1];
        }
    }

    public void ImmeditelyEegDataSave()
    {
        EEGEntry entry = new EEGEntry
        {
            time_stamp = GameManager2.Instance.getFrameTime(),
            delta = GetLatestFeatureValue(sensorID, BandType.DELTA),
            theta = GetLatestFeatureValue(sensorID, BandType.THETA),
            alpha = GetLatestFeatureValue(sensorID, BandType.ALPHA),
            beta = GetLatestFeatureValue(sensorID, BandType.BETA),
            gamma = GetLatestFeatureValue(sensorID, BandType.GAMMA)
        };

        collectedEEG.eeg_data.Add(entry);
    }

    IEnumerator MeasureEEGData()
    {
        yield return new WaitForSeconds(0.1f);
        //WaitForSeconds interval = new WaitForSeconds(1.0f);
        WaitForSeconds interval = new WaitForSeconds(0.1f);

        while (true)
        {
            EEGEntry entry = new EEGEntry
            {
                time_stamp = GameManager2.Instance.getFrameTime(),
                delta = GetLatestFeatureValue(sensorID, BandType.DELTA),
                theta = GetLatestFeatureValue(sensorID, BandType.THETA),
                alpha = GetLatestFeatureValue(sensorID, BandType.ALPHA),
                beta = GetLatestFeatureValue(sensorID, BandType.BETA),
                gamma = GetLatestFeatureValue(sensorID, BandType.GAMMA)
            };

            collectedEEG.eeg_data.Add(entry);

            yield return interval;
        }
    }

    private List<double> GetFeatureDataList(EEGSensorID sensorID, BandType featureIndex)
    {
        List<double> dataList = new List<double>();
        List<double> ScaleDataList = new List<double>();
        List<EEGFeatureIndex> featureScaleList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
        if (featureScaleList.Count > 0)
        {
            for (int i = 0; i < featureScaleList.Count; i++)
            {
                if (featureIndex == BandType.DELTA && !double.IsNaN(featureScaleList[i].Delta(sensorID))) ScaleDataList.Add(featureScaleList[i].Delta(sensorID));
                if (featureIndex == BandType.THETA && !double.IsNaN(featureScaleList[i].Theta(sensorID))) ScaleDataList.Add(featureScaleList[i].Theta(sensorID));
                if (featureIndex == BandType.ALPHA && !double.IsNaN(featureScaleList[i].Alpha(sensorID))) ScaleDataList.Add(featureScaleList[i].Alpha(sensorID));
                if (featureIndex == BandType.BETA && !double.IsNaN(featureScaleList[i].Beta(sensorID))) ScaleDataList.Add(featureScaleList[i].Beta(sensorID));
                if (featureIndex == BandType.GAMMA && !double.IsNaN(featureScaleList[i].Gamma(sensorID))) ScaleDataList.Add(featureScaleList[i].Gamma(sensorID));
            }
        }

        List<EEGFeatureIndex> featureDataList = LooxidLinkData.Instance.GetEEGFeatureIndexData(8.0f);
        if (featureDataList.Count > 0)
        {
            double min = Min(ScaleDataList);
            double max = Max(ScaleDataList);

            int numIndexList = 6 * linkFequency;
            for (int i = 0; i < featureDataList.Count; i++)
            {
                if (i < numIndexList)
                {
                    if (featureIndex == BandType.DELTA)
                    {
                        double delta = double.IsNaN(featureDataList[i].Delta(sensorID)) ? 0.0 : Scale(min, max, 0.0f, 1.0f, featureDataList[i].Delta(sensorID));
                        dataList.Add(delta);
                    }
                    if (featureIndex == BandType.THETA)
                    {
                        double theta = double.IsNaN(featureDataList[i].Theta(sensorID)) ? 0.0 : Scale(min, max, 0.0f, 1.0f, featureDataList[i].Theta(sensorID));
                        dataList.Add(theta);
                    }
                    if (featureIndex == BandType.ALPHA)
                    {
                        double alpha = double.IsNaN(featureDataList[i].Alpha(sensorID)) ? 0.0 : Scale(min, max, 0.0f, 1.0f, featureDataList[i].Alpha(sensorID));
                        dataList.Add(alpha);
                    }
                    if (featureIndex == BandType.BETA)
                    {
                        double beta = double.IsNaN(featureDataList[i].Beta(sensorID)) ? 0.0 : Scale(min, max, 0.0f, 1.0f, featureDataList[i].Beta(sensorID));
                        dataList.Add(beta);
                    }
                    if (featureIndex == BandType.GAMMA)
                    {
                        double gamma = double.IsNaN(featureDataList[i].Gamma(sensorID)) ? 0.0 : Scale(min, max, 0.0f, 1.0f, featureDataList[i].Gamma(sensorID));
                        dataList.Add(gamma);
                    }
                }
            }
        }
        return dataList;
    }

    private double GetLatestFeatureValue(EEGSensorID sensorID, BandType featureIndex)
    {
        List<double> scaledList = GetFeatureDataList(sensorID, featureIndex);
        if (scaledList.Count == 0) return 0.0;
        return scaledList[scaledList.Count - 1];
    }

    private double Min(List<double> minList)
    {
        if (minList.Count <= 0) return 0.0;
        double min = minList[0];
        for (int i = 0; i < minList.Count; i++)
        {
            if (!double.IsNaN(minList[i]))
            {
                if (minList[i] < min) min = minList[i];
            }
        }
        return min;
    }

    private double Max(List<double> maxList)
    {
        if (maxList.Count <= 0) return 0.0;
        double max = maxList[0];
        for (int i = 0; i < maxList.Count; i++)
        {
            if (!double.IsNaN(maxList[i]))
            {
                if (maxList[i] > max) max = maxList[i];
            }
        }
        return max;
    }

    private double Scale(double inputLow, double inputHigh, double outputLow, double outputHigh, double value)
    {
        if (value <= inputLow)
        {
            return outputLow;
        }

        if (value >= inputHigh)
        {
            return outputHigh;
        }

        return (outputHigh - outputLow) * ((value - inputLow) / (inputHigh - inputLow)) + outputLow;
    }

    public void StartMeasuring()
    {
        if (measureCoroutine == null)
            measureCoroutine = StartCoroutine(MeasureEEGData());
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