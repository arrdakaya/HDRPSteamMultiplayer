using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour
{
    [SerializeField] GameObject Door;
    [SerializeField] GameObject KeypadUI;
    [SerializeField] string password;
    [SerializeField] TextMeshProUGUI passwordText;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OpenKeyUI()
    {
        KeypadUI.SetActive(true);
        Cursor.visible= true;
    }
    public void KeyButton(string key)
    {
        passwordText.text = passwordText + key;
    }
    public void ResetPassword()
    {
        passwordText.text = "";
    }
    public void CheckPassword()
    {
        if(passwordText.text == password)
        {
            KeypadUI.SetActive(false);
            ResetPassword();
        }
        else
        {
            ResetPassword();
            
        }
    }
}
