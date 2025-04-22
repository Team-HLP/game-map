using UnityEngine;
using UnityEngine.UI;

public class HistoryEntryUI : MonoBehaviour
{
    public Text resultText;
    public Text scoreText;
    public Text playtimeText;

    private ResultUIManager resultUIManager;

    private string detailedResult;
    private int detailedScore;
    private float detailedTime;

    public void Initialize(string result, int score, float time, ResultUIManager manager)
    {
        resultText.text = result;
        scoreText.text = $"{score}¡°";
        playtimeText.text = $"{time:F1}√ ";

        detailedResult = result;
        detailedScore = score;
        detailedTime = time;

        resultUIManager = manager;

        GetComponent<Button>().onClick.AddListener(OnClickEntry);
    }

    void OnClickEntry()
    {
        resultUIManager.DisplayDetails(detailedResult, detailedScore, detailedTime);
    }
}