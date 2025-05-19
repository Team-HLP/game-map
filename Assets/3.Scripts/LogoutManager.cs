using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public void OnLogoutButtonClick()
    {
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.SetInt("loginSuccess", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MENU");
    }
}