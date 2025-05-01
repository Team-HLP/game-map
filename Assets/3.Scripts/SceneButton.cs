using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "LoginScene";

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}