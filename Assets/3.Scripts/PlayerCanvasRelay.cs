using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasRelay : MonoBehaviour
{
    private void Start()
    {
        Transform canvas = transform.Find("XR Origin/Camera Offset/Canvas");
        if (canvas != null)
        {
            var hpText = canvas.Find("HpText")?.GetComponent<Text>();
            var timerText = canvas.Find("TimerText")?.GetComponent<Text>();
            var scoreText = canvas.Find("ScoreText")?.GetComponent<Text>();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.hpText = hpText;
                GameManager.Instance.timerText = timerText;
                GameManager.Instance.scoreText = scoreText;
            }
        }
        else
        {
            Debug.LogWarning("[PlayerCanvasRelay] Canvas not found!");
        }
    }
}
