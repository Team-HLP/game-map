using UnityEngine;
using UnityEngine.UI;

public class HistoryEntryUI : MonoBehaviour
{
    public Text createdAtText;
    public Text resultText;
    public Text scoreText;

    private ResultUIManager resultUIManager;

    private string result;
    private int score;
    private int hp;
    private int meteorCount;
    private string createdAt;

    public void Initialize(string result, int score, int hp, int meteorCount, string created_at, ResultUIManager manager)
    {
        createdAtText.text = created_at;
        resultText.text = result;
        scoreText.text = $"{score}Á¡";

        this.createdAt = created_at;
        this.result = result;
        this.score = score;
        this.hp = hp;
        this.meteorCount = meteorCount;

        resultUIManager = manager;

        GetComponent<Button>().onClick.AddListener(OnClickEntry);
    }

    void OnClickEntry()
    {
        resultUIManager.DisplayDetails(createdAt, result, score, meteorCount, hp);
    }
}