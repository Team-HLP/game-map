using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("���� ����")]
    [SerializeField] float maxOffset = 0.2f;   // �� ������ �ִ� �̵���(�� ���̸� ������)
    [SerializeField] float shakeDecay = 1.5f;   // ���� �ӵ�(�� ũ�� ���� ��Ƶ�)

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
            float t = intensity * (1 - timer / duration);          // ���� ����
            transform.localPosition = originalPos +
                                       Random.insideUnitSphere * t * maxOffset;

            timer += Time.deltaTime * shakeDecay;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
