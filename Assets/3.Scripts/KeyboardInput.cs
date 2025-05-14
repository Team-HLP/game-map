using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardInput : MonoBehaviour
{
    public TMP_InputField idField;
    public TMP_InputField pwField;
    public LoginManager loginManager;

    private TMP_InputField currentField;

    private void Start()
    {
        currentField = idField;
        idField.ActivateInputField();
    }

    public void OnNumberPress(string number)
    {
        if (currentField != null)
        {
            currentField.text += number;
        }
    }

    public void OnBackspacePress()
    {
        if (currentField != null && currentField.text.Length > 0)
        {
            currentField.text = currentField.text.Substring(0, currentField.text.Length - 1);
        }
    }

    public void OnTabPress()
    {
        currentField = (currentField == idField) ? pwField : idField;
        currentField.ActivateInputField();
    }

    public void OnLoginPress()
    {
        if (loginManager != null)
        {
            loginManager.OnLoginButtonClick();
        }
    }
}
