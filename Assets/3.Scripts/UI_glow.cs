using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_glow : MonoBehaviour
{
    public enum GlowEffect
    {
        SimplePulse,      // 기본 점멸
        Rainbow,          // 무지개 색상 변화
        WaveEffect,       // 파도처럼 퍼지는 효과
        FlickerEffect,    // 깜빡이는 효과
        BreathingEffect   // 숨쉬는 듯한 효과
    }

    [Header("효과 설정")]
    public GlowEffect currentEffect = GlowEffect.SimplePulse;
    
    [Header("기본 설정")]
    [Range(0.1f, 5f)]
    public float effectSpeed = 2f;
    [Range(0f, 1f)]
    public float minBrightness = 0.6f;
    [Range(0f, 1f)]
    public float maxBrightness = 1f;
    
    [Header("추가 설정")]
    [Range(0, 10)]
    public float waveFrequency = 2f;    // 파도 효과의 주파수
    [Range(0, 1)]
    public float flickerIntensity = 0.3f; // 깜빡임 강도
    public Color startColor = Color.white;
    public Color endColor = Color.yellow;

    [Header("대상 설정")]
    public bool affectButtons = true;    // 버튼에 효과 적용
    public bool affectImages = true;     // 이미지에 효과 적용
    public bool includeInactiveObjects = false; // 비활성화된 오브젝트도 포함
    
    private List<Graphic> targetGraphics; // Button과 Image의 부모 클래스인 Graphic 사용
    private float currentTime = 0f;
    private bool isInitialized = false;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        InitializeTargets();
    }

    void InitializeTargets()
    {
        targetGraphics = new List<Graphic>();

        if (affectButtons)
        {
            Button[] buttons = includeInactiveObjects ? 
                GetComponentsInChildren<Button>(true) : 
                GetComponentsInChildren<Button>();

            foreach (Button button in buttons)
            {
                button.interactable = true;
                
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    targetGraphics.Add(buttonImage);
                }

                Image[] childImages = button.GetComponentsInChildren<Image>(includeInactiveObjects);
                foreach (Image img in childImages)
                {
                    if (img != buttonImage && !targetGraphics.Contains(img))
                    {
                        targetGraphics.Add(img);
                    }
                }
            }
        }

        if (affectImages)
        {
            Image[] images = includeInactiveObjects ? 
                GetComponentsInChildren<Image>(true) : 
                GetComponentsInChildren<Image>();

            foreach (Image img in images)
            {
                if (!targetGraphics.Contains(img) && img.GetComponent<Button>() == null)
                {
                    targetGraphics.Add(img);
                }
            }
        }

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized || targetGraphics == null || targetGraphics.Count == 0)
        {
            InitializeTargets();
            return;
        }

        currentTime += Time.deltaTime * effectSpeed;

        switch (currentEffect)
        {
            case GlowEffect.SimplePulse:
                ApplySimplePulse();
                break;
            case GlowEffect.Rainbow:
                ApplyRainbowEffect();
                break;
            case GlowEffect.WaveEffect:
                ApplyWaveEffect();
                break;
            case GlowEffect.FlickerEffect:
                ApplyFlickerEffect();
                break;
            case GlowEffect.BreathingEffect:
                ApplyBreathingEffect();
                break;
        }
    }

    void ApplySimplePulse()
    {
        float glowValue = Mathf.Lerp(minBrightness, maxBrightness, (Mathf.Sin(currentTime) + 1f) * 0.5f);
        
        foreach (Graphic graphic in targetGraphics)
        {
            if (graphic != null && (graphic.gameObject.activeInHierarchy || includeInactiveObjects))
            {
                Color targetColor = Color.Lerp(startColor, endColor, glowValue);
                Color currentColor = graphic.color;
                
                // 알파값 보존
                targetColor.a = currentColor.a;
                
                // 현재 색상과 목표 색상을 보간
                graphic.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * effectSpeed * 2f);
            }
        }
    }

    void ApplyRainbowEffect()
    {
        Color rainbow = Color.HSVToRGB(Mathf.PingPong(currentTime * 0.1f, 1f), 1f, 1f);
        foreach (Graphic graphic in targetGraphics)
        {
            if (graphic != null && (graphic.gameObject.activeInHierarchy || includeInactiveObjects))
            {
                Color color = rainbow;
                color.a = graphic.color.a;
                graphic.color = Color.Lerp(graphic.color, color, Time.deltaTime * effectSpeed);
            }
        }
    }

    void ApplyWaveEffect()
    {
        for (int i = 0; i < targetGraphics.Count; i++)
        {
            Graphic graphic = targetGraphics[i];
            if (graphic != null && (graphic.gameObject.activeInHierarchy || includeInactiveObjects))
            {
                float offset = i * (1f / targetGraphics.Count) * waveFrequency;
                float wave = Mathf.Sin(currentTime + offset) * 0.5f + 0.5f;
                
                Color targetColor = Color.Lerp(startColor, endColor, wave);
                targetColor.a = graphic.color.a;
                
                graphic.color = Color.Lerp(graphic.color, targetColor, Time.deltaTime * effectSpeed);
            }
        }
    }

    void ApplyFlickerEffect()
    {
        foreach (Graphic graphic in targetGraphics)
        {
            if (graphic != null && (graphic.gameObject.activeInHierarchy || includeInactiveObjects))
            {
                float noise = (float)random.NextDouble() * flickerIntensity;
                float flicker = Mathf.Lerp(maxBrightness - flickerIntensity, maxBrightness, noise);
                
                Color targetColor = Color.Lerp(startColor, endColor, flicker);
                targetColor.a = graphic.color.a;
                
                graphic.color = targetColor;
            }
        }
    }

    void ApplyBreathingEffect()
    {
        float t = (Mathf.Sin(currentTime * 0.5f) + 1f) * 0.5f;
        t = Mathf.Pow(t, 2f);
        
        foreach (Graphic graphic in targetGraphics)
        {
            if (graphic != null && (graphic.gameObject.activeInHierarchy || includeInactiveObjects))
            {
                Color targetColor = Color.Lerp(startColor, endColor, t);
                targetColor.a = graphic.color.a;
                
                graphic.color = Color.Lerp(graphic.color, targetColor, Time.deltaTime * effectSpeed);
            }
        }
    }

    public void RefreshTargets()
    {
        InitializeTargets();
    }

    // 런타임에서 효과 대상 변경을 위한 함수들
    public void SetAffectButtons(bool value)
    {
        affectButtons = value;
        RefreshTargets();
    }

    public void SetAffectImages(bool value)
    {
        affectImages = value;
        RefreshTargets();
    }

    public void SetIncludeInactiveObjects(bool value)
    {
        includeInactiveObjects = value;
        RefreshTargets();
    }
}

