using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalLocalPos;
    private bool hasSavedOriginalPos = false; // ★ 추가

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Shake(float duration, float magnitude)
    {
        if (!hasSavedOriginalPos)  // ★ 최초로 흔들기 요청 왔을 때만 위치 저장
        {
            originalLocalPos = transform.localPosition;
            hasSavedOriginalPos = true;
        }

        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalLocalPos + new Vector3(x, 0f, z); // Y는 고정

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
    }
}
