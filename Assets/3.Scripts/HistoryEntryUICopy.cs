using UnityEngine;
using UnityEngine.UI;

public class HistoryEntryUICopy : MonoBehaviour
{
    public Text createdAtText;
    public Text resultText;
    public Text scoreText;

    private ResultUIManager resultUIManager;

    private string result;
    private int score;
    private int hp;
    private int moleCount;
    private string createdAt;

    public void Initialize(string result, int score, int hp, int moleCount, string created_at, ResultUIManager manager)
    {
        createdAtText.text = created_at;
        resultText.text = result;
        scoreText.text = $"{score}점";

        this.createdAt = created_at;
        this.result = result;
        this.score = score;
        this.hp = hp;
        this.moleCount = moleCount;

        resultUIManager = manager;

        GetComponent<Button>().onClick.AddListener(OnClickEntry);
    }

    void OnClickEntry()
    {
        resultUIManager.MoleDisplayDetails(createdAt, result, score, moleCount, hp);
    }
}