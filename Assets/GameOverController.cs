using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameObject gameoverScreen;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1);
        MusicSfxManager.Instance.mixer.SetFloat("MasterVolume", 0f);
    }


    private void Update()
    {
        if (GameManager.Instance.CurrentFuelAmount <= 0)
        {
            gameoverScreen?.SetActive(true);
            Time.timeScale = 0f;
            
            // Cut Volume in Half
            MusicSfxManager.Instance.mixer.SetFloat("MasterVolume", -60f);
        }
    }
}
