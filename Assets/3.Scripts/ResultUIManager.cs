using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    public Text meteorCountText;
    public Text hpText;
    public Text resultText;
    public Text playTimeText;
    public Text scoreText;

    [Header("�����丮 ����Ʈ")]
    public GameObject historyEntryPrefab;
    public Transform contentParent;
    public GameObject detailPanel;

    void Start()
    {
        ShowCurrentResult();

        if (detailPanel != null)
            detailPanel.SetActive(false);

        foreach (var result in GameManager.Instance.gameResults)
        {
            AddHistoryEntry(result);
        }
    }

    void ShowCurrentResult()
    {
        var gm = GameManager.Instance;

        meteorCountText.text = $"�ı��� � �� : {gm.destroyedMeteo}";
        hpText.text = $"���� ���� HP : {gm.hp}";
        resultText.text = gm.success ? "��� : ����" : "��� : ����";
        playTimeText.text = $"�÷��� �ð� : {gm.gameTime:F1}��";
        scoreText.text = $"���� : {gm.score}";
    }

    public void AddHistoryEntry(GameResult result)
    {
        GameObject entryGO = Instantiate(historyEntryPrefab, contentParent);
        HistoryEntryUI entryUI = entryGO.GetComponent<HistoryEntryUI>();

        string resultText = result.success ? "����" : "����";

        entryUI.Initialize(resultText, result.score, result.gameTime, this);
    }

    public void DisplayDetails(string result, int score, float time)
    {
        resultText.text = $"��� : {result}";
        scoreText.text = $"���� : {score}";
        playTimeText.text = $"�÷��� �ð� : {time:F1}��";

        if (detailPanel != null)
            detailPanel.SetActive(true);
    }
}