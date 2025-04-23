using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene("PlayScene");
    }
    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene("StartScene");
    }
}