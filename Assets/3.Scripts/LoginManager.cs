using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInput;    // ID 입력창으로 변경
    [SerializeField] private TMP_InputField passwordInput; // TMP_InputField로 변경
    [SerializeField] private string nextSceneName = "GAME"; // 로그인 성공 후 전환될 씬 이름

    public void OnLoginButtonClick()
    {
        string id = idInput.text;
        string password = passwordInput.text;

        // TODO: 여기에 백엔드 연결 코드가 들어갈 예정
        Debug.Log($"ID: {id}, Password: {password}"); // 테스트용 로그
        
        // 테스트: 입력값이 비어있지 않으면 다음 씬으로 전환
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
