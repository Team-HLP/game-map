using UnityEngine;
using UnityEngine.UI;

public class GameUIRegistrar2 : MonoBehaviour
{
    [Header("연결할 UI 컴포넌트")]
    public Text hpText;
    public Text timerText;

    void Awake()
    {
        if (GameManager2.Instance != null)
        {
            // 런타임에 GameManager에 레퍼런스 할당
            GameManager2.Instance.hpText    = hpText;
            GameManager2.Instance.timerText = timerText;
        }
    }
}
