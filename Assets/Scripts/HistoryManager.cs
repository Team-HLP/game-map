using UnityEngine;
using UnityEngine.UI;

public static class GameResultData
{
    public static string result;
    public static int score;
    public static float playTime;
}

public class HistoryManager : MonoBehaviour
{
    public Transform historyContentParent; // Content 객체
    public GameObject historyEntryPrefab;  // 만든 프리팹
    public string playTimeText;

    public void AddHistory(string result, int score)
    {
        GameObject entry = Instantiate(historyEntryPrefab, historyContentParent);

        Text[] texts = entry.GetComponentsInChildren<Text>();
        texts[0].text = result; // 결과
        texts[1].text = score.ToString(); // 점수
        texts[2].text = playTimeText; // 시간
    }
}