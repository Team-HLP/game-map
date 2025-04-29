using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    [Header("상세 결과 표시용 텍스트")]
    public Text meteorCountText;
    public Text hpText;
    public Text resultText;
    public Text playTimeText;
    public Text scoreText;

    [Header("히스토리 리스트")]
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

        string resultText = result.success ? "성공" : "실패";

        entryUI.Initialize(result.destroyedMeteo, result.hp, resultText, result.score, result.gameTime, this);
    }

    public void DisplayDetails(int meteorCount, int hp, string result, int score, float playTime)
    {
        if (meteorCountText != null)
            meteorCountText.text = $"파괴한 운석 수 : {meteorCount}";

        if (hpText != null)
            hpText.text = $"종료 시점 HP : {hp}";

        if (resultText != null)
            resultText.text = $"결과 : {result}";

        if (playTimeText != null)
            playTimeText.text = $"플레이 시간 : {playTime:F1}초";

        if (scoreText != null)
            scoreText.text = $"점수 : {score}";

        if (detailPanel != null)
            detailPanel.SetActive(true);
    }
}