using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EndSequence : MonoBehaviour
{
    [SerializeField] PostProcessVolume volume;      // GlobalVolume
    [SerializeField] CanvasGroup fadePanel;   // FadePanel

    [Header("����(��)")]
    public float triggerDelay = 10f;                // 10�� �� �ߵ�

    [Header("Ÿ�̹�")]
    public float slowmoFactor = 0.2f;
    public float flashTime = 0.15f;
    public float freezeTime = 0.3f;
    public float fadeTime = 0.6f;

    Bloom bloom;
    MotionBlur mBlur;

    void Awake()
    {
        if (volume == null) volume = FindObjectOfType<PostProcessVolume>();
        if (fadePanel == null) fadePanel = FindObjectOfType<CanvasGroup>();

        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out mBlur);
    }

    public void Trigger() => StartCoroutine(Sequence());

    IEnumerator Sequence()
    {
        /* 0) ���� */
        if (triggerDelay > 0)
            yield return new WaitForSecondsRealtime(triggerDelay);

        /* 1) ���ο��� */
        Time.timeScale = slowmoFactor;
        Time.fixedDeltaTime = 0.02f * slowmoFactor;

        /* 2) ���� (Bloom + MotionBlur) */
        float t = 0;
        while (t < flashTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / flashTime;
            bloom.intensity.value = Mathf.Lerp(0, 10, k);
            mBlur.shutterAngle.value = Mathf.Lerp(0, 360, k);
            yield return null;
        }

        /* 3) ��� ���� */
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeTime);

        /* 4) ���̵� �ƿ� */
        t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            fadePanel.alpha = t / fadeTime;
            yield return null;
        }

        /* 5) Ÿ�ӽ����� ������ �ϰ� �� */
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }
}
