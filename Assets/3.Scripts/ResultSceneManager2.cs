using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager2 : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        if (GameManager2.Instance != null)
        {
            GameManager2.Instance.ResetGameData();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("wak_PlayScene");
    }
    public void OnQuitButtonClicked()
    {
        if (GameManager2.Instance != null)
        {
            GameManager2.Instance.ResetGameData();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("MENU");
    }
    public void OnResultButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("ResultScene");
    }
}