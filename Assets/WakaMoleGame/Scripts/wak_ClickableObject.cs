using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 시선이 닿아 있는 상태에서 트리거(Trigger)를 누르면 파괴되는 오브젝트.
/// 운석 종류별로 다른 이펙트를 발생시키고, 클릭 시 체력 변화도 다르게 설정.
/// </summary>
public class wak_ClickableObject : MonoBehaviour
{
    public enum ObjectType { Meteorite_wak, OtherMeteorite_wak }
    public ObjectType objectType;

    public float autoDestroyTime = 10f; // 자동 파괴 시간
    public InputActionProperty triggerAction;

    private float gazeActiveUntil = 0f;
    private bool hasInteracted = false;

    public GameObject meteoriteExplosionEffectPrefab;
    public GameObject otherMeteoriteExplosionEffectPrefab;
    public GameObject fuelCollectEffectPrefab; // AutoDestroy 시 이펙트

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
                    Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor,
                    Time.deltaTime * highlightLerpSpeed));
            else if (hasColorProp)
                _renderer.material.SetColor("_Color",
                    Color.Lerp(_renderer.material.GetColor("_Color"), _targetColor,
                    Time.deltaTime * highlightLerpSpeed));
        }

        if (triggerAction != null && triggerAction.action != null)
    {
        if (triggerAction.action.WasPressedThisFrame())
        {
            Debug.Log("트리거 클릭됨");
        }
    }

        if (isGazedNow && triggerAction.action != null && triggerAction.action.WasPressedThisFrame())
        {
            Debug.Log("클릭됨");
            Interact();
        }
    }

    public void OnGazeEnter() => gazeActiveUntil = Time.time + 0.1f;
    public void OnGazeExit() { }

    void Interact()
    {
        if (hasInteracted) return;
        hasInteracted = true;
        CancelInvoke(nameof(AutoDestroy));

        if (objectType == ObjectType.Meteorite_wak)
        {
            GameManager.Instance.AddHp(10); // 체력 +10
            SpawnEffect(meteoriteExplosionEffectPrefab);
        }
        else if (objectType == ObjectType.OtherMeteorite_wak)
        {
            GameManager.Instance.AddHp(-20); // 체력 -20
            SpawnEffect(otherMeteoriteExplosionEffectPrefab);
        }

        Destroy(gameObject);
    }

    void AutoDestroy()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        GameManager.Instance.AddHp(-20); // 자동 파괴 시 체력 -20

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
