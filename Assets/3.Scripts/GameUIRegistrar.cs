using UnityEngine;
using UnityEngine.UI;

public class GameUIRegistrar : MonoBehaviour
{
    [Header("연결할 UI 컴포넌트")]
    public Text hpText;
    public Text timerText;
    public Text scoreText;

    void Awake()
    {
        if (GameManager.Instance != null)
        {
            // 런타임에 GameManager에 레퍼런스 할당
            GameManager.Instance.hpText    = hpText;
            GameManager.Instance.timerText = timerText;
            GameManager.Instance.scoreText = scoreText;
        }
    }
}
