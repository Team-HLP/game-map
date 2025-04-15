using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Text meteoriteCountText;
    public Text finalHpText;
    public Text resultText;
    public Text playTimeText;

    void Start()
    {
        var gm = GameManager.Instance;
        var co = ClickableObject.Instance;

        meteoriteCountText.text = "파괴한 운석 수 : " + co.meteoriteDestroyedCount;
        finalHpText.text = "종료 시점 체력 : " + gm.hp;
        resultText.text = "결과 : " + (gm.hp > 0 ? "성공" : "실패");

        float playTime = 90f - gm.gameTime;
        int minutes = Mathf.FloorToInt(playTime / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);
        playTimeText.text = $"플레이 시간 : {minutes:00}:{seconds:00}";
    }
}