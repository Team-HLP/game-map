using UnityEngine;
using UnityEngine.Events;
using VSX.GameStates;      // GameState 형식

public class GameStateEventRelay : MonoBehaviour
{
    [Header("필터")]
    public GameState targetState;     // Success 또는 PlayerDeath 등 드래그

    [Header("콜백")]
    public UnityEvent onTargetStateEntered;   // 원하는 함수들 추가

    // GameStateManager.onEnteredGameState(GameState) 의 Dynamic 리스너로 연결
    public void HandleEnteredState(GameState state)
    {
        if (state == targetState)
            onTargetStateEntered.Invoke();
    }
}
