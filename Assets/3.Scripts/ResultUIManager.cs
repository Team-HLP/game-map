using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using System.Globalization;
using System;

public class ResultUIManager : MonoBehaviour
{
    [Header("�� ��� ǥ�ÿ� �ؽ�Ʈ")]
    public Text createdAtText;
    public Text resultText;
    public Text scoreText;
    public Text hpText;
    public Text meteorCountText;

    [Header("�����丮 ����Ʈ")]
    public GameObject historyEntryPrefab;
    public Transform contentParent;
    public GameObject detailPanel;

    [Header("�� ��� ǥ�ÿ� �ؽ�Ʈ")]
    public Text moleCreatedAtText;
    public Text moleResultText;
    public Text moleScoreText;
    public Text moleHpText;
    public Text moleCountText;

    [Header("�δ��� �����丮 ����Ʈ")]
    public GameObject moleHistoryEntryPrefab;
    public Transform moleContentParent;
    public GameObject moleDetailPanel;

    void Start()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
        if (moleDetailPanel != null)
            moleDetailPanel.SetActive(false);

        StartCoroutine(LoadGameResultsFromServer());
    }

    IEnumerator LoadGameResultsFromServer()
    {
        string url = Apiconfig.url + "/games?gameCategory=METEORITE_DESTRUCTION";
        UnityWebRequest request = UnityWebRequest.Get(url);

        string accessToken = PlayerPrefs.GetString("access_token", "");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            List<GameResultResponse> serverResults = JsonConvert.DeserializeObject<List<GameResultResponse>>(json);

            foreach (var result in serverResults)
            {
                string resultStr = result.result == "����" ? "����" : "����";

                DateTime createdTime;
                string displayTime;

                if (DateTime.TryParseExact(result.created_at, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out createdTime))
                {
                    displayTime = createdTime.ToString("yyyy.MM.dd HH:mm");
                }
                else
                {
                    displayTime = result.created_at;
                }

                AddHistoryEntry(resultStr, result.score, result.hp, result.meteorite_broken_count, displayTime);
            }
        }

        url = Apiconfig.url + "/games?gameCategory=CATCH_MOLE";
        request = UnityWebRequest.Get(url);

        accessToken = PlayerPrefs.GetString("access_token", "");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            List<GameResultResponse> serverResults = JsonConvert.DeserializeObject<List<GameResultResponse>>(json);

            foreach (var result in serverResults)
            {
                string resultStr = result.result == "����" ? "����" : "����";

                DateTime createdTime;
                string displayTime;

                if (DateTime.TryParseExact(result.created_at, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out createdTime))
                {
                    displayTime = createdTime.ToString("yyyy.MM.dd HH:mm");
                }
                else
                {
                    displayTime = result.created_at;
                }

                AddMoleHistoryEntry(resultStr, result.score, result.hp, result.meteorite_broken_count, displayTime);
            }
        }
    }

    public void AddHistoryEntry(string result, int score, int hp, int meteorCount, string created_at)
    {
        GameObject entryGO = Instantiate(historyEntryPrefab, contentParent);
        HistoryEntryUI entryUI = entryGO.GetComponent<HistoryEntryUI>();
        entryUI.Initialize(result, score, hp, meteorCount, created_at, this);
    }

    public void AddMoleHistoryEntry(string result, int score, int hp, int meteorCount, string created_at)
    {
        GameObject entryGO = Instantiate(moleHistoryEntryPrefab, moleContentParent);
        HistoryEntryUICopy entryUI = entryGO.GetComponent<HistoryEntryUICopy>();
        entryUI.Initialize(result, score, hp, meteorCount, created_at, this);
    }

    public void DisplayDetails(string created_at, string result, int score, int meteorCount, int hp)
    {
        if (createdAtText != null)
            createdAtText.text = $"�÷��� �ð�  :  {created_at}";

        if (resultText != null)
            resultText.text = $"���  :  {result}";

        if (scoreText != null)
            scoreText.text = $"����  :  {score}";

        if (hpText != null)
            hpText.text = $"���� ���� HP  :  {hp}";

        if (meteorCountText != null)
            meteorCountText.text = $"�ı��� � ��  :  {meteorCount}";

        if (detailPanel != null)
            detailPanel.SetActive(true);
    }

    public void MoleDisplayDetails(string created_at, string result, int score, int moleCount, int hp)
    {
        if (moleCreatedAtText != null)
            moleCreatedAtText.text = $"�÷��� �ð�  :  {created_at}";

        if (moleResultText != null)
            moleResultText.text = $"���  :  {result}";

        if (moleScoreText != null)
            moleScoreText.text = $"����  :  {score}";

        if (moleHpText != null)
            moleHpText.text = $"���� ���� HP  :  {hp}";

        if (moleCountText != null)
            moleCountText.text = $"���� �δ��� ��  :  {moleCount}";

        if (moleDetailPanel != null)
            moleDetailPanel.SetActive(true);
    }
}