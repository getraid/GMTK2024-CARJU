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
    public Image loadingIcon = null;
    public GameObject loadingText = null;
    public GameObject credits = null;
    private UnityEngine.AsyncOperation loading;
    private float timer = 0;
    public void SwitchSceneFromMainMenu()
    {
        
#if UNITY_EDITOR
        LoadAsync();
#endif
        
#if UNITY_STANDALONE_WIN
        LoadAsync();
#endif

#if UNITY_WEBGL
        LoadSync();
#endif
    }

    private void LoadAsync()
    {
        isLoading = true;
        loadingIcon?.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);
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
    

    private void Update()
    {
        if(loadingIcon != null && loading != null)
            if (loadingIcon.gameObject.activeSelf)
                loadingIcon.rectTransform.rotation = Quaternion.Euler(0,0,timer*20);
            else
            {
                MusicSfxManager.Instance.PlaySingleSfx(transform.position, MusicSfxManager.TypeOfSfx.ui_click);
                loadingText.gameObject.SetActive(true);
                loadingIcon.gameObject.SetActive(true);
            }

        timer += Time.deltaTime;
    }
}