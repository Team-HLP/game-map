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

    // --- 3DVisualizer 예제에서 가져온 센서 상태 저장 변수
    private EEGSensor sensorStatusData;

    private Coroutine measureCoroutine;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "eeg_data.json");
        collectedEEG = new EEGDataWrapper();
        LooxidLinkManager.Instance.Initialize();

        // 센서 상태 이벤트 구독
        LooxidLinkData.OnReceiveEEGSensorStatus += OnReceiveEEGSensorStatus;

        // 원시 신호 이벤트 구독
        LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;

        // 노이즈 / 센서 Off 메시지
        LooxidLinkManager.OnShowNoiseSignalMessage += () =>
            Debug.LogWarning("[EEG] 노이즈 신호 발생 (Noise Detected)");
        LooxidLinkManager.OnHideNoiseSignalMessage += () =>
            Debug.Log("[EEG] 노이즈 신호 사라짐 (Noise Cleared)");

        StartMeasuring();
    }

    void OnDestroy()
    {
        LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceiveEEGSensorStatus;
        LooxidLinkData.OnReceiveEEGRawSignals   -= OnReceiveEEGRawSignals;
    }

    // 센서 접촉 상태 콜백
    void OnReceiveEEGSensorStatus(EEGSensor status)
    {
        sensorStatusData = status;
        bool isFp1On = sensorStatusData.IsSensorOn(EEGSensorID.Fp1);
        Debug.Log($"[EEG STATUS] Fp1 Sensor On? {isFp1On}");
    }

    // 원시 신호 콜백
    void OnReceiveEEGRawSignals(EEGRawSignal raw)
    {
        double[] channelData = raw.FilteredRawSignal(sensorID);
        if (channelData != null && channelData.Length > 0)
        {
            double sample = channelData[channelData.Length - 1];
            Debug.Log($"[RAW SIGNAL] {sensorID} latest sample: {sample}");
        }
        else
        {
            Debug.LogWarning($"[RAW SIGNAL] {sensorID} no raw data!");
        }
    }

    IEnumerator MeasureEEGData()
    {
        WaitForSeconds interval = new WaitForSeconds(1.0f);

        while (true)
        {
            // 연결 상태
            Debug.Log("[EEG] Link Core: " +
                      LooxidLinkManager.Instance.isLinkCoreConnected +
                      " | Link Hub: " +
                      LooxidLinkManager.Instance.isLinkHubConnected);

            // 밴드 파워 데이터 (float 리터럴)
            List<EEGFeatureIndex> recentData =
                LooxidLinkData.Instance.GetEEGFeatureIndexData(1.0f);

            foreach (var data in recentData)
            {
                if (sensorStatusData != null)
                {
                    bool on = sensorStatusData.IsSensorOn(sensorID);
                    Debug.Log($"[EEG STATUS] {sensorID} On? {on}");
                }

                // 원본 로그 값
                double rawDelta = data.Delta(sensorID);
                double rawTheta = data.Theta(sensorID);
                double rawAlpha = data.Alpha(sensorID);
                double rawBeta  = data.Beta(sensorID);
                double rawGamma = data.Gamma(sensorID);
                // Debug.Log($"[EEG RAW] Δ:{rawDelta}, Θ:{rawTheta}, α:{rawAlpha}, β:{rawBeta}, γ:{rawGamma}");

                // 로그→선형 변환 후 저장
                EEGEntry entry = new EEGEntry
                {
                    time_stamp = GameManager.Instance.getFrameTime(),
                    delta      = ConvertLogToLinear(rawDelta),
                    theta      = ConvertLogToLinear(rawTheta),
                    alpha      = ConvertLogToLinear(rawAlpha),
                    beta       = ConvertLogToLinear(rawBeta),
                    gamma      = ConvertLogToLinear(rawGamma)
                };
                // Debug.Log($"[EEG CONV] Δ:{entry.delta}, Θ:{entry.theta}, α:{entry.alpha}, β:{entry.beta}, γ:{entry.gamma}");

                collectedEEG.eeg_data.Add(entry);
            }

            yield return interval;
        }
    }

    double ConvertLogToLinear(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return 0.0;
        return Math.Pow(10, value);
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

    // GameManager용: 저장된 데이터 초기화
    public void ResetManager()
    {
        collectedEEG = new EEGDataWrapper();
    }

    // GameManager용: 측정 재시작
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
