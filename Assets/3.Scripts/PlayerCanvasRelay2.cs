using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasRelay2 : MonoBehaviour
{
    private void Start()
    {
        Transform canvas = transform.Find("XR Origin/Camera Offset/Canvas");
        if (canvas != null)
        {
            var hpText = canvas.Find("HpText")?.GetComponent<Text>();
            var timerText = canvas.Find("TimerText")?.GetComponent<Text>();

            if (GameManager2.Instance != null)
            {
                GameManager2.Instance.hpText = hpText;
                GameManager2.Instance.timerText = timerText;
            }
        }
        else
        {
            Debug.LogWarning("[PlayerCanvasRelay] Canvas not found!");
        }
    }
}
