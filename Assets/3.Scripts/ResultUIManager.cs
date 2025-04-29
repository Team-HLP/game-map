using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    [Header("�� ��� ǥ�ÿ� �ؽ�Ʈ")]
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
        if (detailPanel != null)
            detailPanel.SetActive(false);

        foreach (var result in GameManager.Instance.gameResults)
        {
            AddHistoryEntry(result);
        }
    }

    public void AddHistoryEntry(GameResult result)
    {
        GameObject entryGO = Instantiate(historyEntryPrefab, contentParent);
        HistoryEntryUI entryUI = entryGO.GetComponent<HistoryEntryUI>();

        string resultText = result.success ? "����" : "����";

        entryUI.Initialize(result.destroyedMeteo, result.hp, resultText, result.score, result.gameTime, this);
    }

    public void DisplayDetails(int meteorCount, int hp, string result, int score, float playTime)
    {
        if (meteorCountText != null)
            meteorCountText.text = $"�ı��� � �� : {meteorCount}";

        if (hpText != null)
            hpText.text = $"���� ���� HP : {hp}";

        if (resultText != null)
            resultText.text = $"��� : {result}";

        if (playTimeText != null)
            playTimeText.text = $"�÷��� �ð� : {playTime:F1}��";

        if (scoreText != null)
            scoreText.text = $"���� : {score}";

        if (detailPanel != null)
            detailPanel.SetActive(true);
    }
}