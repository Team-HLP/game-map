using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class BehaviorDataManager : MonoBehaviour
{
    private string filePath;
    private List<ObjectBehaviorRecord> behaviorRecords = new List<ObjectBehaviorRecord>();

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "behavior_data.json");
    }

    // 오브젝트 이벤트 기록 함수
    public void RecordObjectEvent(string objectType, string eventType)
    {
        behaviorRecords.Add(new ObjectBehaviorRecord
        {
            time_stamp = Time.time,
            object_type = objectType,   // 예: "Meteorite", "Fuel"
            event_type = eventType      // 예: "Spawned", "Destroyed", "LookedAt"
        });
    }

    // JSON으로 저장
    public void SaveBehaviorData()
    {
        BehaviorDataWrapper wrapper = new BehaviorDataWrapper
        {
            behaviors = behaviorRecords.ToArray()
        };

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Debug.Log("행동 데이터 저장 경로: " + filePath);
    }

    [System.Serializable]
    private class BehaviorDataWrapper
    {
        public ObjectBehaviorRecord[] behaviors;
    }

    [System.Serializable]
    private class ObjectBehaviorRecord
    {
        public float time_stamp;
        public string object_type;
        public string event_type;
    }
}
