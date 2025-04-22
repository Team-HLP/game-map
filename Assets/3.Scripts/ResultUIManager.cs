using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
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

        meteorCountText.text = $"파괴한 운석 수 : {gm.destroyedMeteo}";
        hpText.text = $"종료 시점 HP : {gm.hp}";
        resultText.text = gm.success ? "결과 : 성공" : "결과 : 실패";
        playTimeText.text = $"플레이 시간 : {gm.gameTime:F1}초";
        scoreText.text = $"점수 : {gm.score}";
    }

    public void AddHistoryEntry(GameResult result)
    {
        GameObject entryGO = Instantiate(historyEntryPrefab, contentParent);
        HistoryEntryUI entryUI = entryGO.GetComponent<HistoryEntryUI>();

        string resultText = result.success ? "성공" : "실패";

        entryUI.Initialize(resultText, result.score, result.gameTime, this);
    }

    public void DisplayDetails(string result, int score, float time)
    {
        resultText.text = $"결과 : {result}";
        scoreText.text = $"점수 : {score}";
        playTimeText.text = $"플레이 시간 : {time:F1}초";

        if (detailPanel != null)
            detailPanel.SetActive(true);
    }
}