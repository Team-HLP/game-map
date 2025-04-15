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

        meteoriteCountText.text = "�ı��� � �� : " + co.meteoriteDestroyedCount;
        finalHpText.text = "���� ���� ü�� : " + gm.hp;
        resultText.text = "��� : " + (gm.hp > 0 ? "����" : "����");

        float playTime = 90f - gm.gameTime;
        int minutes = Mathf.FloorToInt(playTime / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);
        playTimeText.text = $"�÷��� �ð� : {minutes:00}:{seconds:00}";
    }
}