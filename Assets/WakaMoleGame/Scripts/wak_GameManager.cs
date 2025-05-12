using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 게임 전체 흐름을 관리하는 싱글톤 매니저
public class wak_GameManager : MonoBehaviour
{
    public static wak_GameManager Instance; // 싱글톤 인스턴스

    public int hp = 100;                    // 플레이어 체력
    public int score = 0;                   // 점수 (현재 사용 안 함)
    public int destroyedMeteo = 0;          // 파괴한 운석 개수
    public bool success = false;            // 게임 성공 여부
    public float gameTime = 20f;            // 제한 시간 (90초)

    //public List<GameResult> gameResults = new List<GameResult>(); // 게임 결과 리스트 저장

    public Text hpText;                     // 체력 UI 텍스트
    public Text timerText;                  // 타이머 UI 텍스트

    public EyesDataManager eyesDataManager; // 눈 데이터 매니저 (깜빡임, 동공 등)

    // 싱글톤 초기화 및 중복 방지
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else Destroy(gameObject);          // 중복이면 제거
    }

    // 씬 로드 이벤트 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 로드 이벤트 제거
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 UI 오브젝트 연결
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "wak_PlayScene") // 특정 씬일 때만 실행
        {
            hpText = GameObject.Find("wak_HpText")?.GetComponent<Text>();         // 체력 텍스트 연결
            timerText = GameObject.Find("wak_TimerText")?.GetComponent<Text>();   // 타이머 텍스트 연결

            UpdateHpUI();      // 초기 체력 표시
            UpdateTimerUI();   // 초기 타이머 표시
        }
    }

    // 게임 시작 시 초기화
    private void Start()
    {
        Time.timeScale = 1f;   // 타임스케일 초기화 (혹시 멈춰있을 경우 대비)
        UpdateHpUI();          // 체력 UI 표시
    }

    // 매 프레임마다 실행됨
    private void Update()
    {
        if (success) return;   // 이미 성공했으면 더 이상 진행 안 함

        gameTime -= Time.deltaTime; // 남은 시간 감소
        UpdateTimerUI();           // 타이머 UI 갱신

        if (gameTime <= 0)         // 시간이 다 됐을 경우
        {
            GameSuccess();         // 성공 처리
        }
    }

    // 체력 변경 처리
    public void AddHp(int amount)
    {
        hp += amount;                        // 체력 증가 또는 감소
        hp = Mathf.Clamp(hp, 0, 100);        // 0 ~ 100 사이로 제한
        UpdateHpUI();                        // 체력 UI 갱신

        if (hp <= 0)                         // 체력이 0이 되면
        {
            GameOver();                     // 게임오버 처리
        }
    }

    // 체력 UI 텍스트 업데이트
    private void UpdateHpUI()
    {
        if (hpText != null)
            hpText.text = "HP : " + hp;
    }

    // 타이머 UI 텍스트 업데이트
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);    // 분 계산
            int seconds = Mathf.FloorToInt(gameTime % 60);    // 초 계산
            timerText.text = $"{minutes:00}:{seconds:00}";    // 00:00 형식 출력
        }
    }

    // 게임 성공 시 호출
    private void GameSuccess()
    {
        success = true;
        //SaveGameResult();                 // 결과 저장
        eyesDataManager.SaveEyesData();  // 눈 데이터 저장
        SceneManager.LoadScene("wak_GameSuccessScene"); // 성공 씬으로 전환
    }

    // 게임 오버 시 호출
    private void GameOver()
    {
        //SaveGameResult();                 // 결과 저장
        eyesDataManager.SaveEyesData();  // 눈 데이터 저장
        SceneManager.LoadScene("wak_GameOverScene"); // 게임오버 씬으로 전환
    }

    // 게임 결과 저장 (List에 추가)
    // private void SaveGameResult()
    // {
    //     int index = gameResults.Count + 1; // 플레이 횟수
    //     var result = new GameResult(index, hp, success, score, destroyedMeteo, 90f - gameTime);
    //     gameResults.Add(result);          // 리스트에 저장
    // }

    // 게임 데이터 초기화
    public void ResetGameData()
    {
        hp = 100;
        score = 0;
        destroyedMeteo = 0;
        success = false;
        gameTime = 20f;
    }
}
