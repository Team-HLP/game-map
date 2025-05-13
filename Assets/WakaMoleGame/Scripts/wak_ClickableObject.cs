using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class wak_ClickableObject : MonoBehaviour
{
    public enum ObjectType { Meteorite_wak, OtherMeteorite_wak }
    public ObjectType objectType;

    public float autoDestroyTime = 10f;
    public InputActionProperty triggerAction;

    private float gazeActiveUntil = 0f;
    private bool hasInteracted = false;

    public GameObject meteoriteExplosionEffectPrefab;
    public GameObject otherMeteoriteExplosionEffectPrefab;
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

        if (triggerAction != null && triggerAction.action != null)
        {
            triggerAction.action.Enable();
            Debug.Log($"[wak_ClickableObject] TriggerAction enabled: {triggerAction.action.name}");
        }
        else
        {
            Debug.LogWarning("[wak_ClickableObject] TriggerAction is not assigned or invalid.");
        }

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
                    Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor,
                    Time.deltaTime * highlightLerpSpeed));
            else if (hasColorProp)
                _renderer.material.SetColor("_Color",
                    Color.Lerp(_renderer.material.GetColor("_Color"), _targetColor,
                    Time.deltaTime * highlightLerpSpeed));
        }

        // 트리거 반응 체크
        if (triggerAction != null && triggerAction.action != null)
        {
            if (triggerAction.action.WasPressedThisFrame())
            {
                Debug.Log("[wak_ClickableObject] 트리거 눌림 감지됨");
            }

            if (isGazedNow && triggerAction.action.WasPressedThisFrame())
            {
                Debug.Log("[wak_ClickableObject] 응시 중 트리거 입력 → Interact 실행");
                Interact();
            }
        }
    }

    public void OnGazeEnter()
    {
        gazeActiveUntil = Time.time + 0.15f; // 유예 시간
        Debug.Log("[wak_ClickableObject] Gaze Enter");
    }

    public void OnGazeExit() { }

    void Interact()
    {
        if (hasInteracted) return;
        hasInteracted = true;
        CancelInvoke(nameof(AutoDestroy));

        switch (objectType)
        {
            case ObjectType.Meteorite_wak:
                GameManager.Instance.AddHp(10);
                SpawnEffect(meteoriteExplosionEffectPrefab);
                break;
            case ObjectType.OtherMeteorite_wak:
                GameManager.Instance.AddHp(-20);
                SpawnEffect(otherMeteoriteExplosionEffectPrefab);
                break;
        }

        Destroy(gameObject);
    }

    void AutoDestroy()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        GameManager.Instance.AddHp(-20);

        if (fuelCollectEffectPrefab != null)
            SpawnEffect(fuelCollectEffectPrefab);

        Destroy(gameObject);
    }

    void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null) return;

        var fx = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 2f);
    }
}
