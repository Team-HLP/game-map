using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class BehaviorDataManager : MonoBehaviour
{
    [SerializeField] int sessionSeconds = 90;

    string filePath;
    // 초 단위 → (오브젝트 타입 → 카운터)
    readonly Dictionary<int, Dictionary<string, ObjectCounter>> data =
        new Dictionary<int, Dictionary<string, ObjectCounter>>();

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "behavior_series.json");
        for (int s = 0; s <= sessionSeconds; s++)
            data[s] = new Dictionary<string, ObjectCounter>();
    }

    public void RecordObjectEvent(string objectType, string eventType)
    {
        int sec = Mathf.Clamp(Mathf.FloorToInt(Time.time), 0, sessionSeconds);

        if (!data[sec].TryGetValue(objectType, out var counter))
        {
            counter = new ObjectCounter { object_type = objectType };
            data[sec][objectType] = counter;
        }

        if (eventType == "LookedAt")      counter.looked++;
        else if (eventType == "Destroyed") counter.destroyed++;
    }

    public void SaveBehaviorData()
    {
        var wrapper = new TimeSeriesWrapper();
        var list = new List<SecondEntry>();

        foreach (var kv in data)
        {
            var entry = new SecondEntry
            {
                time_stamp = kv.Key,
                objects    = new List<ObjectCounter>(kv.Value.Values).ToArray()
            };
            list.Add(entry);
        }
        wrapper.series = list.ToArray();

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Debug.Log("시계열 행동 데이터 저장: " + filePath);
    }

    [System.Serializable]
    class TimeSeriesWrapper
    {
        public SecondEntry[] series;
    }

    [System.Serializable]
    class SecondEntry
    {
        public int time_stamp;             // 기존 second → time_stamp
        public ObjectCounter[] objects;
    }

    [System.Serializable]
    class ObjectCounter
    {
        public string object_type;
        public int looked;
        public int destroyed;
    }
}
