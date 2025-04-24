using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClickableObject : MonoBehaviour
{
    public enum ObjectType { Meteorite, Fuel }
    public ObjectType objectType;

    [SerializeField] private float continuousGazeTime = 3f;   // ★ 연속 응시 필요 시간
    public  float  autoDestroyTime = 10f;

    private float gazeTimer = 0f;          // ★ 연속 응시 누적용
    private float gazeActiveUntil = 0f;    // 0.1초 유예

    private bool hasInteracted = false;

    public GameObject explosionEffectPrefab;
    public GameObject fuelCollectEffectPrefab;

    public Color highlightColor = Color.red;
    public float highlightLerpSpeed = 10f;

    private Renderer _renderer;
    private Color    _originalColor;
    private Color    _targetColor;

    /* ──────────────────────────────── 초기 설정 ─────────────────────────────── */
    void Start ()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
            _targetColor   = _originalColor;
        }
        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }

    /* ──────────────────────────────── 매 프레임 ─────────────────────────────── */
    void Update ()
    {
        if (hasInteracted) return;

        /* 1) 지금 이 프레임에 시선이 유효한가? */
        bool isGazedNow = Time.time <= gazeActiveUntil;

        /* 2) 연속 응시 타이머 처리 */
        if (isGazedNow)
        {
            gazeTimer += Time.deltaTime;                 // 연속 응시 누적
            _targetColor = highlightColor;

            if (gazeTimer >= continuousGazeTime)         // ★ 2초 연속 응시 달성!
            {
                OnGazeComplete();
            }
        }
        else
        {
            gazeTimer = 0f;                              // 시선 끊기면 리셋
            _targetColor = _originalColor;
        }

        /* 3) 하이라이트 색상 보간 */
        if (_renderer != null)
        {
            if (_renderer.material.HasProperty("_BaseColor"))
                _renderer.material.SetColor("_BaseColor",
                    Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor,
                               Time.deltaTime * highlightLerpSpeed));
            else
                _renderer.material.color =
                    Color.Lerp(_renderer.material.color, _targetColor,
                               Time.deltaTime * highlightLerpSpeed);
        }
    }

    /* ──────────────────── 시선이 들어오는 매 프레임마다 호출 ─────────────────── */
    public void OnGazeEnter ()
    {
        // 0.1초 유예를 계속 연장해 주면, 프레임 하나 정도의 미세 끊김을 허용
        gazeActiveUntil = Time.time + 0.1f;
    }

    public void OnGazeExit () { /* 연속 응시판별은 Update에서 처리하니 비워둠 */ }

    /* ────────────────────────────── 성공 처리 ──────────────────────────────── */
    void OnGazeComplete ()
    {
        if (hasInteracted) return;
        hasInteracted = true;
        CancelInvoke(nameof(AutoDestroy));

        if (objectType == ObjectType.Meteorite)
        {
            GameManager.Instance.destroyedMeteo++;
            SpawnEffect(explosionEffectPrefab);
        }
        else if (objectType == ObjectType.Fuel)
        {
            SpawnEffect(explosionEffectPrefab);
        }
        Destroy(gameObject);
    }

    /* ────────────────────────────── 자동 파괴 ──────────────────────────────── */
    void AutoDestroy ()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        if (objectType == ObjectType.Meteorite)
            GameManager.Instance.AddHp(-10);
        else
        {
            SpawnEffect(fuelCollectEffectPrefab);
            GameManager.Instance.AddHp(10);
        }
        Destroy(gameObject);
    }

    /* ────────────────────────────── 이펙트 스폰 ────────────────────────────── */
    void SpawnEffect (GameObject effectPrefab)
    {
        if (effectPrefab == null) return;
        var fx = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 2f);
    }
}
