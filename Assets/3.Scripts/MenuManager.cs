using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject loginButton;

    void Start()
    {
        if (loginButton != null && PlayerPrefs.GetInt("loginSuccess", 0) == 1)
        {
            loginButton.SetActive(false);
        }
    }
}
