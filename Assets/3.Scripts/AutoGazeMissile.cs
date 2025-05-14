using UnityEngine;
using VSX.Weapons;      // ← 수정된 네임스페이스
using Tobii.XR;

public class AutoGazeMissile : MonoBehaviour
{
    [Header("Gaze → Fire Settings")]
    public float dwellTime = 1.0f;
    public float cooldown  = 0.3f;
    public int   missileTriggerIndex = 1;   // Secondary

    [Header("References")]
    public TriggerablesManager triggerables;   // 드래그
    public LayerMask gazeMask;                 // GazeTarget 레이어

    float timer = 0f, nextTime = 0f;
    ClickableObject last;

    void Awake()
    {
        Debug.Log("[AutoGazeMissile] Awake called!");
        triggerables = GetComponentInParent<TriggerablesManager>();
        triggerables.SetSelectedTriggerGroup(0);

        // 디버그: 어떤 모듈이 어떤 트리거 인덱스에 매핑됐는지 출력
        for (int i = 0; i < triggerables.MountedTriggerables.Count; ++i)
        {
            var m = triggerables.MountedTriggerables[i];
            Debug.Log($"[Triggerables] Mounted[{i}]: {m.triggerable.name} → triggerValue {m.triggerValuesByGroup[0]}");
        }
    }

    void Reset()
    {
        triggerables = GetComponent<TriggerablesManager>();
        gazeMask     = LayerMask.GetMask("GazeTarget");
    }

    void Update()
    {
        // Debug.Log("[AutoGazeMissile] Update tick");
        var eye = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (!eye.GazeRay.IsValid) { ResetTimer(); return; }

        Ray ray = new Ray(eye.GazeRay.Origin, eye.GazeRay.Direction);
        if (!Physics.Raycast(ray, out var hit, 300f, gazeMask)) { 
            Debug.Log($"[AutoGaze] Raycast hit: {hit.collider.name}");
            ResetTimer(); return; }

        var obj = hit.collider.GetComponent<ClickableObject>();
        if (obj == null) { ResetTimer(); return; }

        if (obj == last) timer += Time.deltaTime;
        else { last = obj; timer = 0f; }

        if (timer >= dwellTime && Time.time >= nextTime)
        {
            triggerables.TriggerOnce(missileTriggerIndex);   // ★ 한 발
            nextTime = Time.time + cooldown;
        }
    }

    void ResetTimer() { timer = 0f; last = null; }
}
