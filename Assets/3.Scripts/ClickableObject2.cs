using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableObject2 : MonoBehaviour
{
    public enum ObjectType { Meteorite, Fuel }
    public ObjectType objectType;

    /* ───────── 설정값 ───────── */
    public float autoDestroyTime = 3f;
    [Header("Rush-to-Camera")]
    public float rushDuration = 0.6f;      // 달려드는 데 걸리는 시간
    public float rushOffset = 1f;        // 카메라 앞 어느 지점에서 터질지
    public AnimationCurve rushCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public InputActionProperty triggerAction;

    public GameObject explosionEffectPrefab;
    public GameObject fuelCollectEffectPrefab;

    /* ───────── 내부 상태 ───────── */
    float gazeActiveUntil = 0f;
    bool hasInteracted = false;

    void Start()
    {
        if (triggerAction.action != null) triggerAction.action.Enable();
        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }

    void Update()
    {
        if (hasInteracted) return;

        bool isGazedNow = Time.time <= gazeActiveUntil;
        if (isGazedNow &&
            triggerAction.action != null &&
            triggerAction.action.WasPressedThisFrame())
        {
            Interact();
        }
    }

    public void OnGazeEnter() => gazeActiveUntil = Time.time + 0.1f;
    public void OnGazeExit() => gazeActiveUntil = 0f;

    /* ───────── 유저 상호작용 ───────── */
    void Interact()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        GazeRaycaster2.SaveUserDestoryStatus(objectType.ToString());
        GameManager2.Instance.ImmeditelyBioDataSave();

        if (objectType == ObjectType.Meteorite)
        {
            GameManager2.Instance.destroyedMeteo++;
            SpawnEffect(explosionEffectPrefab);
            GameManager2.Instance.AddScore();
        }
        else
        {
            SpawnEffect(explosionEffectPrefab);
            GameManager2.Instance.AddHp(-10);
        }

        Destroy(gameObject);
    }

    /* ───────── 자동 파괴 & Rush ───────── */
    void AutoDestroy()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        GazeRaycaster2.SaveAutoDestoryStatus(objectType.ToString());
        GameManager2.Instance.ImmeditelyBioDataSave();

        if (objectType == ObjectType.Meteorite)
        {
            StartCoroutine(RushAndExplode());
        }
        else
        {
            GameManager2.Instance.FlashHpColor(true);
            SpawnEffect(fuelCollectEffectPrefab);
            GameManager2.Instance.AddHp(10);
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator RushAndExplode()
    {
        // 준비
        Transform cam = Camera.main.transform;
        Vector3 startPos = transform.position;
        Vector3 endPos = cam.position + cam.forward * rushOffset;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rushDuration;
            float eased = rushCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, endPos, eased);
            transform.LookAt(cam);                        // 달려드는 느낌
            yield return null;
        }

        // 충격 연출
        SpawnEffect(explosionEffectPrefab);
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(intensity: 0.8f, duration: 0.4f);

        GameManager2.Instance.FlashHpColor(false);
        GameManager2.Instance.AddHp(-10);

        Destroy(gameObject);
    }

    /* ───────── FX 스폰 ───────── */
    void SpawnEffect(GameObject prefab)
    {
        if (prefab == null) return;
        var fx = Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(fx, 2f);
    }

    public string GetObjectTypeAsString() => objectType.ToString();
}
