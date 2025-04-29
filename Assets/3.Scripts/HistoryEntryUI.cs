using UnityEngine;
using UnityEngine.UI;

public class HistoryEntryUI : MonoBehaviour
{
    public Text resultText;
    public Text scoreText;
    public Text playtimeText;

    private ResultUIManager resultUIManager;

    private int meteorCount;
    private int hp;
    private string result;
    private int score;
    private float playTime;

    public void Initialize(int meteorCount, int hp, string result, int score, float playTime, ResultUIManager manager)
    {
        resultText.text = result;
        scoreText.text = $"{score}¡°";
        playtimeText.text = $"{playTime:F1}√ ";

        this.meteorCount = meteorCount;
        this.hp = hp;
        this.result = result;
        this.score = score;
        this.playTime = playTime;

        resultUIManager = manager;

        GetComponent<Button>().onClick.AddListener(OnClickEntry);
    }

    void OnClickEntry()
    {
        resultUIManager.DisplayDetails(meteorCount, hp, result, score, playTime);
    }
}