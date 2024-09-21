using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameObject grayBg;

    public GameObject gameoverScreen;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1);
    }


    private void Update()
    {
        if (GameManager.Instance.CurrentFuelAmount <= 0)
        {
            grayBg?.SetActive(true);
            gameoverScreen?.SetActive(true);
            Time.timeScale = 0f;
 
        }
    }
}
