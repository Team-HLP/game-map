using UnityEngine;
using UnityEngine.SceneManagement;

public class wak_ResultSceneManger : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart 버튼 클릭됨");  // 이거 찍히는지 확인
        wak_GameManager.Instance.ResetGameData();
        Time.timeScale = 1;
        SceneManager.LoadScene("wak_PlayScene");

    }

    public void OnQuitButtonClicked()
    {
        wak_GameManager.Instance.ResetGameData();
        SceneManager.LoadScene("MENU");
    }
    public void OnResultButtonClicked()
    {
        SceneManager.LoadScene("wak_ResultScene");
    }
}