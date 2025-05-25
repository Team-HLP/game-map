using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EndSequence : MonoBehaviour
{
    [SerializeField] PostProcessVolume volume;      // GlobalVolume
    [SerializeField] CanvasGroup fadePanel;   // FadePanel

    [Header("지연(초)")]
    public float triggerDelay = 10f;                // 10초 후 발동

    [Header("타이밍")]
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
        /* 0) 지연 */
        if (triggerDelay > 0)
            yield return new WaitForSecondsRealtime(triggerDelay);

        /* 1) 슬로우모션 */
        Time.timeScale = slowmoFactor;
        Time.fixedDeltaTime = 0.02f * slowmoFactor;

        /* 2) 섬광 (Bloom + MotionBlur) */
        float t = 0;
        while (t < flashTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / flashTime;
            bloom.intensity.value = Mathf.Lerp(0, 10, k);
            mBlur.shutterAngle.value = Mathf.Lerp(0, 360, k);
            yield return null;
        }

        /* 3) 잠깐 정지 */
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeTime);

        /* 4) 페이드 아웃 */
        t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            fadePanel.alpha = t / fadeTime;
            yield return null;
        }

        /* 5) 타임스케일 복원만 하고 끝 */
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }
}
