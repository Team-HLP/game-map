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
        request.SetRequestHeader("Authorization", "Bearer " + "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJpZCI6MzksImV4cCI6MTc0NzI5NTk2MX0.h6Xac4xo0Ule8fw8zcQoNb5-zbYNNPAQEbjugeoP7GD4gTMvFlQqDnjHxU1gi8VUwP7mXw7kE3rvxi9rtyiCog");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("서버에서 받은 결과:\n" + json);

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
        else
        {
            Debug.LogError("서버에서 결과 불러오기 실패: " + request.error);
            Debug.LogError("서버 응답:\n" + request.downloadHandler.text);
        }

        url = Apiconfig.url + "/games?gameCategory=CATCH_MOLE";
        request = UnityWebRequest.Get(url);

        accessToken = PlayerPrefs.GetString("access_token", "");
        request.SetRequestHeader("Authorization", "Bearer " + "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJpZCI6MzksImV4cCI6MTc0NzI5NTk2MX0.h6Xac4xo0Ule8fw8zcQoNb5-zbYNNPAQEbjugeoP7GD4gTMvFlQqDnjHxU1gi8VUwP7mXw7kE3rvxi9rtyiCog");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("서버에서 받은 결과:\n" + json);

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
        else
        {
            Debug.LogError("서버에서 결과 불러오기 실패: " + request.error);
            Debug.LogError("서버 응답:\n" + request.downloadHandler.text);
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
}