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
        triggerables = GetComponentInParent<TriggerablesManager>();
        triggerables.SetSelectedTriggerGroup(0);
    }

    void Reset()
    {
        triggerables = GetComponent<TriggerablesManager>();
        gazeMask     = LayerMask.GetMask("GazeTarget");
    }

    void Update()
    {
        var eye = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (!eye.GazeRay.IsValid) { ResetTimer(); return; }

        Ray ray = new Ray(eye.GazeRay.Origin, eye.GazeRay.Direction);
        if (!Physics.Raycast(ray, out var hit, 300f, gazeMask)) { 
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
