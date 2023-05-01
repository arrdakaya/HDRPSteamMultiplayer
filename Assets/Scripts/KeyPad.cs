using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour
{
    public GameObject KeypadUI;
    [SerializeField] string password;
    [SerializeField] TextMeshProUGUI passwordText;
    public bool isPasswordCorrect = false;

    public void OpenKeyUI()
    {
        KeypadUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible= true;
    }
    public void CloseKeyUI()
    {
        KeypadUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void KeyButton(string key)
    {
        if(passwordText.text.Length < 5)
        {
            passwordText.text = passwordText.text + key;
        }
    }
    public void ResetPassword()
    {
        if(passwordText.text.Length == 0)
        {
            passwordText.text = "";
        }
        else
        {
            if(!isPasswordCorrect) 
            {
                StartCoroutine("WrongPassword");

            }
            else
            {
                passwordText.text = "";
            }

        }
    }
    public void RedButton()
    {
        passwordText.text = "";

    }
    public void CheckPassword()
    {
        if(passwordText.text == password)
        {
            StartCoroutine("CorrectPassword");
        }
        else
        {
            isPasswordCorrect = false;
            ResetPassword();
            
        }
    }
    IEnumerator WrongPassword()
    {
        passwordText.text = "Incorrect";
        yield return new WaitForSeconds(1);
        passwordText.text = "";
    }

    IEnumerator CorrectPassword()
    {
        isPasswordCorrect = true;
        passwordText.text = "Correct";
        yield return new WaitForSeconds(1);
        CloseKeyUI();
        ResetPassword();

    }
}
