using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
   
    public void PressAnyKey()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("MainMenu");

        }
    }
    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "FirstScene")
        {
            PressAnyKey();
        }
       
       
    }
 
}
