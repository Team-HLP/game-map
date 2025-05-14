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

    public Color highlightColor = Color.red;
    public float highlightLerpSpeed = 10f;

    private Renderer _renderer;
    private Color _originalColor;
    private Color _targetColor;

    private bool hasBaseColorProp;
    private bool hasColorProp;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            hasBaseColorProp = _renderer.material.HasProperty("_BaseColor");
            hasColorProp = _renderer.material.HasProperty("_Color");

            if (hasBaseColorProp)
                _originalColor = _renderer.material.GetColor("_BaseColor");
            else if (hasColorProp)
                _originalColor = _renderer.material.GetColor("_Color");
            else
                _renderer = null;

            _targetColor = _originalColor;
        }

        if (triggerAction.action != null)
            triggerAction.action.Enable();

        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }

    void Update()
    {
        if (hasInteracted) return;

        bool isGazedNow = Time.time <= gazeActiveUntil;
        _targetColor = isGazedNow ? highlightColor : _originalColor;

        if (_renderer != null)
        {
            if (hasBaseColorProp)
                _renderer.material.SetColor("_BaseColor",
                    Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor, Time.deltaTime * highlightLerpSpeed));
            else if (hasColorProp)
                _renderer.material.SetColor("_Color",
                    Color.Lerp(_renderer.material.GetColor("_Color"), _targetColor, Time.deltaTime * highlightLerpSpeed));
        }

        if (isGazedNow && triggerAction.action != null && triggerAction.action.WasPressedThisFrame())
        {
            Debug.Log("[ClickableObject2] 트리거 입력 감지됨 → Interact 호출");
            Interact();
        }
    }

    public void OnGazeEnter() => gazeActiveUntil = Time.time + 0.1f;
    public void OnGazeExit() {
        // 즉시 하이라이트 해제
        gazeActiveUntil = 0f;
        _targetColor = _originalColor;
    }

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
        if (hasInteracted) return;
        hasInteracted = true;

        GazeRaycaster2.SaveAutoDestoryStatus(objectType.ToString());
        GameManager2.Instance.ImmeditelyBioDataSave();

        if (objectType == ObjectType.Meteorite)
        {
            GameManager2.Instance.AddHp(-10);
        }
        else
        {
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

    public string GetObjectTypeAsString()
    {
        return objectType.ToString();
    }
}
