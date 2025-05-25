using UnityEngine;
using UnityEngine.UI;

public class GameUIRegistrar : MonoBehaviour
{
    [Header("연결할 UI 컴포넌트")]
    public Text hpText;
    public Text timerText;
    public Text scoreText;

    public GameObject Canvas;
    public GameObject gameResultUI;
    public Text resultHpText;
    public Text resultScoreText;
    public Text resultText;

    void Awake()
    {
        if (GameManager.Instance != null)
        {
            // 런타임에 GameManager에 레퍼런스 할당
            GameManager.Instance.Canvas = Canvas;
            GameManager.Instance.hpText    = hpText;
            GameManager.Instance.timerText = timerText;
            GameManager.Instance.scoreText = scoreText;

            GameManager.Instance.gameResultUI = gameResultUI;
            GameManager.Instance.resultHpText = resultHpText;
            GameManager.Instance.resultScoreText = resultScoreText;
            GameManager.Instance.resultText = resultText;
        }
    }
}
