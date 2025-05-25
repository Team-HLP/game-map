using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("세기 조절")]
    [SerializeField] float maxOffset = 0.2f;   // 한 프레임 최대 이동량(← 줄이면 약해짐)
    [SerializeField] float shakeDecay = 1.5f;   // 감쇠 속도(↑ 크면 빨리 잦아듦)

    Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    public void Shake(float intensity = 0.3f, float duration = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    System.Collections.IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = intensity * (1 - timer / duration);          // 선형 감쇠
            transform.localPosition = originalPos +
                                       Random.insideUnitSphere * t * maxOffset;

            timer += Time.deltaTime * shakeDecay;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
