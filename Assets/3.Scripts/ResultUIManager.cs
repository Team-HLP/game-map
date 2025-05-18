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
    [Header("상세 결과 표시용 텍스트")]
    public Text createdAtText;
    public Text resultText;
    public Text scoreText;
    public Text hpText;
    public Text meteorCountText;

    [Header("히스토리 리스트")]
    public GameObject historyEntryPrefab;
    public Transform contentParent;
    public GameObject detailPanel;

    [Header("상세 결과 표시용 텍스트")]
    public Text moleCreatedAtText;
    public Text moleResultText;
    public Text moleScoreText;
    public Text moleHpText;
    public Text moleCountText;

    [Header("두더지 히스토리 리스트")]
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
                string resultStr = result.result == "성공" ? "성공" : "실패";

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
                string resultStr = result.result == "성공" ? "성공" : "실패";

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
            createdAtText.text = $"플레이 시간  :  {created_at}";

        if (resultText != null)
            resultText.text = $"결과  :  {result}";

        if (scoreText != null)
            scoreText.text = $"점수  :  {score}";

        if (hpText != null)
            hpText.text = $"종료 시점 HP  :  {hp}";

        if (meteorCountText != null)
            meteorCountText.text = $"파괴한 운석 수  :  {meteorCount}";

        if (detailPanel != null)
            detailPanel.SetActive(true);
    }

    public void MoleDisplayDetails(string created_at, string result, int score, int moleCount, int hp)
    {
        if (moleCreatedAtText != null)
            moleCreatedAtText.text = $"플레이 시간  :  {created_at}";

        if (moleResultText != null)
            moleResultText.text = $"결과  :  {result}";

        if (moleScoreText != null)
            moleScoreText.text = $"점수  :  {score}";

        if (moleHpText != null)
            moleHpText.text = $"종료 시점 HP  :  {hp}";

        if (moleCountText != null)
            moleCountText.text = $"잡은 두더지 수  :  {moleCount}";

        if (moleDetailPanel != null)
            moleDetailPanel.SetActive(true);
    }
}