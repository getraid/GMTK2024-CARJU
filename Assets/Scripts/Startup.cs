using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AsyncOperation = System.ComponentModel.AsyncOperation;

public class Startup: MonoBehaviour
{

    [Header("For TitleScreen")] 
    private bool isLoading = false;
    public Slider loadingBar = null;
    private UnityEngine.AsyncOperation loading;
    
    public void SwitchSceneFromMainMenu()
    {
        isLoading = true;
        loadingBar?.gameObject.SetActive(true);
        loading = SceneManager.LoadSceneAsync(1);
        loadingBar.value = loading.progress;
    }

    private void Update()
    {
        if(loadingBar != null && loading != null)
            if (loadingBar.gameObject.activeSelf)
                loadingBar.value = Mathf.Clamp01(loading.progress / 0.9f);
            else
            {
                MusicSfxManager.Instance.PlaySingleSfx(transform.position, MusicSfxManager.TypeOfSfx.ui_click);
                loadingBar.gameObject.SetActive(true);
            }
        
    }
}