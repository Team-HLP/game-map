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
    public Transform historyContentParent; // Content ��ü
    public GameObject historyEntryPrefab;  // ���� ������
    public string playTimeText;

    public void AddHistory(string result, int score)
    {
        GameObject entry = Instantiate(historyEntryPrefab, historyContentParent);

        Text[] texts = entry.GetComponentsInChildren<Text>();
        texts[0].text = result; // ���
        texts[1].text = score.ToString(); // ����
        texts[2].text = playTimeText; // �ð�
    }
}