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
        loadingBar?.gameObject.SetActive(true);
        loading = SceneManager.LoadSceneAsync(1);
        loadingBar.value = loading.progress;
    }

    private void LoadSync()
    {
        SceneManager.LoadScene(1);
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