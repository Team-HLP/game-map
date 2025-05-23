using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableObject2 : MonoBehaviour
{
    public enum ObjectType { Meteorite, Fuel }
    public ObjectType objectType;

    public float autoDestroyTime = 3f;
    public InputActionProperty triggerAction;

    private float gazeActiveUntil = 0f;
    private bool hasInteracted = false;

    public GameObject explosionEffectPrefab;
    public GameObject fuelCollectEffectPrefab;

    void Start()
    {
        // 트리거 액션 활성화
        if (triggerAction.action != null)
            triggerAction.action.Enable();

        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }

    void Update()
    {
        if (hasInteracted) return;

        bool isGazedNow = Time.time <= gazeActiveUntil;

        // 응시 중 + 트리거 입력 → Interact
        if (isGazedNow && triggerAction.action != null && triggerAction.action.WasPressedThisFrame())
        {
            Interact();
        }
    }

    // GazeRaycaster 에서 호출됨
    public void OnGazeEnter() => gazeActiveUntil = Time.time + 0.1f;
    public void OnGazeExit() => gazeActiveUntil = 0f;

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

    void AutoDestroy()
    {
        if (hasInteracted)
        {
            return;
        }
        hasInteracted = true;

        GazeRaycaster2.SaveAutoDestoryStatus(objectType.ToString());
        GameManager2.Instance.ImmeditelyBioDataSave();

        if (objectType == ObjectType.Meteorite)
        {
            GameManager2.Instance.FlashHpColor(false);
            GameManager2.Instance.AddHp(-10);
        }
        else
        {
            GameManager2.Instance.FlashHpColor(true);
            SpawnEffect(fuelCollectEffectPrefab);
            GameManager2.Instance.AddHp(10);
        }

        Destroy(gameObject);
    }

    void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null) return;
        var fx = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 2f);
    }

    public string GetObjectTypeAsString() => objectType.ToString();
}
