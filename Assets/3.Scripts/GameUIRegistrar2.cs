using UnityEngine;
using UnityEngine.UI;

public class GameUIRegistrar2 : MonoBehaviour
{
    [Header("연결할 UI 컴포넌트")]
    public Text hpText;
    public Text timerText;
    public Text scoreText;

    public GameObject gameResultUI;
    public Text resultHpText;
    public Text resultScoreText;
    public Text resultText;

    void Awake()
    {
        if (GameManager2.Instance != null)
        {
            // 런타임에 GameManager에 레퍼런스 할당
            GameManager2.Instance.hpText    = hpText;
            GameManager2.Instance.timerText = timerText;
            GameManager2.Instance.scoreText = scoreText;

            GameManager.Instance.gameResultUI = gameResultUI;
            GameManager.Instance.resultHpText = resultHpText;
            GameManager.Instance.resultScoreText = resultScoreText;
            GameManager.Instance.resultText = resultText;
        }
    }
}
