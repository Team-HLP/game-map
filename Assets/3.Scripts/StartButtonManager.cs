using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayScene");
    }
}