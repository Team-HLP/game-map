using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject loginButton;
    public GameObject logoutButton;

    void Start()
    {
        int isLoggedIn = PlayerPrefs.GetInt("loginSuccess", 0);

        if (isLoggedIn == 1)
        {
            loginButton.SetActive(false);
            logoutButton.SetActive(true);
        }
        else
        {
            loginButton.SetActive(true);
            logoutButton.SetActive(false);
        }
    }
}
