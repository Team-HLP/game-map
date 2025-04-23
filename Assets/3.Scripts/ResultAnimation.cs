using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultAnimation : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;        // 점수를 표시할 텍스트
    public TextMeshProUGUI finalGradeText;   // 최종 등급을 표시할 텍스트
    public Image starImage1;                 // 별점 이미지들
    public Image starImage2;
    public Image starImage3;
    
    [Header("Animation Settings")]
    public float scoreCountDuration = 2.0f;   // 점수 카운팅 애니메이션 시간
    public float starAnimationDelay = 0.5f;   // 별들 사이의 애니메이션 딜레이
    public float starFillDuration = 0.5f;     // 별이 채워지는 시간
    
    private int targetScore;                  // 최종 점수
    private float currentDisplayScore;        // 현재 표시되는 점수
    
    void Start()
    {
        // 시작할 때는 모든 UI를 숨김
        scoreText.text = "0";
        finalGradeText.text = "";
        starImage1.fillAmount = 0;
        starImage2.fillAmount = 0;
        starImage3.fillAmount = 0;
    }

    public void ShowResults(int score)
    {
        targetScore = score;
        StartCoroutine(AnimateResults());
    }

    IEnumerator AnimateResults()
    {
        // 점수 카운팅 애니메이션
        float elapsedTime = 0f;
        currentDisplayScore = 0;
        
        while (elapsedTime < scoreCountDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / scoreCountDuration;
            currentDisplayScore = Mathf.Lerp(0, targetScore, progress);
            scoreText.text = Mathf.RoundToInt(currentDisplayScore).ToString();
            yield return null;
        }
        
        // 최종 점수 확실히 표시
        scoreText.text = targetScore.ToString();
        
        // 등급 결정 및 표시
        string grade = DetermineGrade(targetScore);
        finalGradeText.text = grade;
        
        // 별점 애니메이션
        yield return StartCoroutine(AnimateStars(targetScore));
    }
    
    string DetermineGrade(int score)
    {
        if (score >= 90) return "S";
        if (score >= 80) return "A";
        if (score >= 70) return "B";
        if (score >= 60) return "C";
        return "D";
    }
    
    IEnumerator AnimateStars(int score)
    {
        // 점수에 따라 별 개수 결정
        int starCount = score >= 90 ? 3 : score >= 70 ? 2 : score >= 50 ? 1 : 0;
        
        // 첫번째 별
        if (starCount >= 1)
        {
            yield return StartCoroutine(FillStar(starImage1));
            yield return new WaitForSeconds(starAnimationDelay);
        }
        
        // 두번째 별
        if (starCount >= 2)
        {
            yield return StartCoroutine(FillStar(starImage2));
            yield return new WaitForSeconds(starAnimationDelay);
        }
        
        // 세번째 별
        if (starCount >= 3)
        {
            yield return StartCoroutine(FillStar(starImage3));
        }
    }
    
    IEnumerator FillStar(Image starImage)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < starFillDuration)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = elapsedTime / starFillDuration;
            starImage.fillAmount = fillAmount;
            yield return null;
        }
        
        starImage.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
