using UnityEngine;
using VSX.GameStates;
using VSX.Vehicles;   // GameAgentManager

public class SurvivalStateController : MonoBehaviour
{
    [Header("조건")]
    public float surviveTime = 180f;         // 성공 판정까지 버틸 시간

    [Header("Game States (드래그)")]
    public GameState successState;           // UVC_Success 에셋
    public GameState failureState;           // UVC_PlayerDeath(또는 Failure) 에셋

    bool gameEnded = false;

    void Start()
    {
        // 플레이어 사망 이벤트 구독 (UnityEvent → AddListener)
        GameAgentManager.Instance.onFocusedGameAgentDied.AddListener(OnPlayerDied);

        // 생존 타이머 시작
        Invoke(nameof(OnSurvived), surviveTime);
    }

    /* ---------- 콜백 ---------- */

    // 사망 시 호출(파라미터 없음 – UnityEvent 정의가 무파라미터)
    void OnPlayerDied()
    {
        if (gameEnded) return;
        gameEnded = true;

        GameStateManager.Instance.EnterGameState(failureState);
        Debug.Log("<color=red>[Survival] Player destroyed → FAILURE</color>");
    }

    // 180초 생존 시 호출
    void OnSurvived()
    {
        if (gameEnded) return;
        gameEnded = true;

        GameStateManager.Instance.EnterGameState(successState);
        Debug.Log("<color=green>[Survival] 3 minutes survived → SUCCESS</color>");
    }
}
