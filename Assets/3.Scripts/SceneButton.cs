using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GAME";  // 인스펙터에서 설정 가능

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
