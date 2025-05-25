using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    /* ───── 효과/타이밍 설정 ───── */
    [Header("Timings")]
    public float fadeOutTime = 0.6f;
    public float fadeInTime = 0.6f;
    public float flashTime = 0.15f;
    public float slowmoFactor = 0.2f;
    public float freezeTime = 0.3f;

    PostProcessVolume volume;
    CanvasGroup panel;
    Bloom bloom;
    MotionBlur mBlur;

    /* ───────── 싱글턴 & 준비 ───────── */
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        /* 1) 전역 볼륨 찾거나 생성 */
        volume = FindObjectOfType<PostProcessVolume>();
        if (volume == null)
        {
            volume = new GameObject("GlobalVolume")
                        .AddComponent<PostProcessVolume>();
            volume.isGlobal = true;
            volume.profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            volume.profile.AddSettings<Bloom>();
            volume.profile.AddSettings<MotionBlur>();
        }
        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out mBlur);

        /* 2) 전용 페이드 캔버스 생성 (한 번만) */
        var canvasGO = new GameObject("FadeCanvas",
                         typeof(Canvas), typeof(CanvasGroup));
        DontDestroyOnLoad(canvasGO);
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        var imgGO = new GameObject("Panel",
                        typeof(UnityEngine.UI.Image));
        imgGO.transform.SetParent(canvasGO.transform, false);
        var img = imgGO.GetComponent<UnityEngine.UI.Image>();
        img.color = Color.black;
        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        panel = canvasGO.GetComponent<CanvasGroup>();
        panel.alpha = 0;                               // 시작은 투명
    }

    /* ───────── 외부 호출 함수 ───────── */
    public void FadeAndLoad(string sceneName, float delay = 0f)
        => StartCoroutine(FadeRoutine(sceneName, delay));

    IEnumerator FadeRoutine(string sceneName, float delay)
    {
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);

        /* 1) 슬로모 + 플래시 */
        Time.timeScale = slowmoFactor;
        Time.fixedDeltaTime = 0.02f * slowmoFactor;

        float t = 0;
        while (t < flashTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / flashTime;
            bloom.intensity.value = Mathf.Lerp(0, 10, k);
            mBlur.shutterAngle.value = Mathf.Lerp(0, 360, k);
            yield return null;
        }

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeTime);

        /* 2) 페이드-아웃 */
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            panel.alpha = t / fadeOutTime;
            yield return null;
        }

        /* 3) 씬 로드 */
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        /* 4) 페이드-인 */
        t = fadeInTime;
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            panel.alpha = t / fadeInTime;
            yield return null;
        }
        panel.alpha = 0;
    }
}
