using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AsyncOperation = System.ComponentModel.AsyncOperation;

public class Startup: MonoBehaviour
{

    [Header("For TitleScreen")] 
    private bool isLoading = false;
    public GameObject credits = null;
    private UnityEngine.AsyncOperation loading;
    public void SwitchSceneFromMainMenu()
    {

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        LoadAsync();
#elif UNITY_WEBGL
        LoadSync();
#endif
    }

    private void LoadAsync()
    {
        isLoading = true;
        loading = SceneManager.LoadSceneAsync(1);
        
    }

    private void LoadSync()
    {
        SceneManager.LoadScene(1);
    }
    
    public void DisableCredits()
    {
        credits.SetActive(false);
    }

    public void enableCred()
    {
        credits.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    
}