using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class ClickableObject : MonoBehaviour
{
    public enum ObjectType { Meteorite, Fuel }
    public ObjectType objectType;

    [SerializeField] private float continuousGazeTime = 3f; // ★ 연속 응시 필요 시간
    public float autoDestroyTime = 10f;

    private float gazeTimer = 0f;          // ★ 연속 응시 누적용
    private float gazeActiveUntil = 0f;    // 0.1초 유예

    private bool hasInteracted = false;

    public GameObject explosionEffectPrefab;
    public GameObject fuelCollectEffectPrefab;
    public GameObject healParticles;

    public Color highlightColor = Color.red;
    public float highlightLerpSpeed = 10f;

    private Renderer _renderer;
    private Color _originalColor;
    private Color _targetColor;

    /* ──────────────────────────────── 초기 설정 ─────────────────────────────── */
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
            _targetColor = _originalColor;
        }
        if (healParticles != null)
        {
            healParticles.SetActive(false);
        }
        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }

    /* ──────────────────────────────── 매 프레임 ─────────────────────────────── */
    void Update()
    {
        if (hasInteracted) return;

        // 1) 지금 이 프레임에 시선이 유효한가?
        bool isGazedNow = Time.time <= gazeActiveUntil;

        // 2) 연속 응시 타이머 처리
        if (isGazedNow)
        {
            gazeTimer += Time.deltaTime;
            _targetColor = highlightColor;

            if (gazeTimer >= continuousGazeTime)
            {
                OnGazeComplete();
            }
        }
        else
        {
            gazeTimer = 0f;
            _targetColor = _originalColor;
        }

        // 3) 하이라이트 색상 보간
        if (_renderer != null)
        {
            _renderer.material.color = Color.Lerp(
                _renderer.material.color,
                _targetColor,
                Time.deltaTime * highlightLerpSpeed
            );

        }
    }

    /* ──────────────────── 시선이 들어오는 매 프레임마다 호출 ─────────────────── */
    public void OnGazeEnter()
    {
        gazeActiveUntil = Time.time + 0.1f;
    }

    public void OnGazeExit()
    {
        // 연속 응시 판별은 Update에서 처리하므로 생략
    }

    /* ────────────────────────────── 성공 처리 ──────────────────────────────── */
    void OnGazeComplete()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        GazeRaycaster.SaveUserDestoryStatus(objectType.ToString());
        GameManager.Instance.ImmeditelyBioDataSave();
        CancelInvoke(nameof(AutoDestroy));

        if (objectType == ObjectType.Meteorite)
        {
            GameManager.Instance.destroyedMeteo++;
            SpawnEffect(explosionEffectPrefab);
            GameManager.Instance.AddScore();
        }
        else
        {
            SpawnEffect(explosionEffectPrefab);
        }

        Destroy(gameObject);
    }

    /* ────────────────────────────── 자동 파괴 ──────────────────────────────── */
    void AutoDestroy()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        GazeRaycaster.SaveAutoDestoryStatus(objectType.ToString());
        GameManager.Instance.ImmeditelyBioDataSave();

        if (objectType == ObjectType.Meteorite)
        {
            SpawnEffect(explosionEffectPrefab);

            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(intensity: 0.8f, duration: 0.4f);

            GameManager.Instance.FlashHpColor(false);
            GameManager.Instance.AddHp(-10);
        }
        else
        {
            GameManager.Instance.FlashHpColor(true);
            SpawnEffect(fuelCollectEffectPrefab);
            GameManager.Instance.AddHp(10);
            ShowHealParticlesFor3Seconds();
        }

        Destroy(gameObject);
    }

    public void ShowHealParticlesFor3Seconds()
    {
        StartCoroutine(ActivateHealParticlesTemporarily(1f));
    }

    private IEnumerator ActivateHealParticlesTemporarily(float duration)
    {
        healParticles.SetActive(true);
        yield return new WaitForSeconds(duration);
        healParticles.SetActive(false);
    }

    /* ────────────────────────────── 이펙트 스폰 ────────────────────────────── */
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
