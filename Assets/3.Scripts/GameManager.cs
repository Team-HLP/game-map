using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Valve.Newtonsoft.Json;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int hp = 100;
    public int score = 0;           // 점수 설정 아직 안 함
    public int destroyedMeteo = 0;  // 운석 파괴 횟수
    public bool success = false;    // 게임 성공 여부
    public float gameTime = 20f;    // 게임 플레이 90초로 제한

    public Text hpText;
    public Text timerText;

    public EyesDataManager eyesDataManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            hpText = GameObject.Find("HpText")?.GetComponent<Text>();
            timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
            eyesDataManager = GameObject.Find("EyesDataManager")?.GetComponent<EyesDataManager>();

            eyesDataManager.ReMeasuring();
            UpdateHpUI();
            UpdateTimerUI();
        }
    }
    private void Start()
    {
        Time.timeScale = 1;
        UpdateHpUI();
    }
    private void Update()
    {
        if (success) return;

        gameTime -= Time.deltaTime;
        UpdateTimerUI();

        if (gameTime <= 0)
        {
            GameSuccess();
        }
    }
    public void AddHp(int amount)
    {
        hp += amount;
        hp = Mathf.Clamp(hp, 0, 100);
        UpdateHpUI();

        if (hp <= 0)
        {
            GameOver();
        }
    }

    private void UpdateHpUI()
    {
        if (hpText != null)
            hpText.text = "HP : " + hp;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void GameSuccess()
    {
        success = true;
        SaveGameResult();
        eyesDataManager.SaveEyesData();
        SceneManager.LoadScene("GameSuccessScene");
    }

    private void GameOver()
    {
        SaveGameResult();
        eyesDataManager.SaveEyesData();
        SceneManager.LoadScene("GameOverScene");
    }

    private void SaveGameResult()
    {
        string result = success ? "SUCCESS" : "FAIL";
        StartCoroutine(GameResultCoroutine(result, score, hp, destroyedMeteo));
    }

    public void ResetGameData()
    {
        hp = 100;
        score = 0;
        destroyedMeteo = 0;
        success = false;
        gameTime = 20f;

        if (eyesDataManager != null)
        {
            eyesDataManager.ResetManager();
        }
    }

    IEnumerator GameResultCoroutine(string result, int score, int hp, int meteorite_broken_count)
    {
        GameResultRequest requestData = new GameResultRequest(result, score, hp, meteorite_broken_count);

        string jsonBody = JsonConvert.SerializeObject(requestData);
        Debug.Log(jsonBody);

        UnityWebRequest request = new UnityWebRequest(Apiconfig.url + "/game/meteorite", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 엑세스 토큰
        string accessToken = PlayerPrefs.GetString("access_token", "");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("결과 저장 성공");
        }
        else
        {
            Debug.LogError("결과 저장 실패: " + request.error);
            Debug.LogError("서버 응답 본문: " + request.downloadHandler.text);
        }
    }

}