using System.Collections;
using System.Collections.Generic;
// CameraShake.cs
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] float shakeDecay = 1.5f;   // °¨¼è ¼Óµµ
    Vector3 originalPos;
    float currentIntensity;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    public void Shake(float intensity = 0.5f, float duration = 0.3f)
    {
        currentIntensity = intensity;
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration));
    }

    System.Collections.IEnumerator ShakeRoutine(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * currentIntensity;
            currentIntensity = Mathf.Lerp(currentIntensity, 0, Time.deltaTime * shakeDecay);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
