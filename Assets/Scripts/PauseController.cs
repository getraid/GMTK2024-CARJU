using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] RectTransform pauseMenu;
    [SerializeField] RectTransform pauseMenu2;
    [SerializeField] TMP_Text _debreeCountText;
    [SerializeField] Slider _debreeSlider;
    [SerializeField] GameObject StatusPanelUI;
    [SerializeField] GameObject grayBg;
    [SerializeField] GameObject _layout;

    bool _isPaused = false;

    private void Start()
    {
        _debreeSlider.value = Debree.MaxDebrieLimitor;      //Sync
        SetMusicVolume(0.8f);
        SetSfxVolume(0.6f);
    }
    // Update is called once per frame
    void Update()
    {
        // Pause Check
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (!_isPaused)
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0;
        grayBg.gameObject.SetActive(true);
        StatusPanelUI.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        pauseMenu2.gameObject.SetActive(true);
        _layout.SetActive(true);
    }

    public void Unstuck()
    {
        ContinueGame();

        VehicleController vehicle = GameManager.Instance.GetActiveVehicle();
        if (vehicle == null)
            return;

        vehicle.CallUnstuck();
    }

    public void ContinueGame()
    {
        _isPaused = false;
        Time.timeScale = 1;
        grayBg.gameObject.SetActive(false);
        StatusPanelUI.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        pauseMenu2.gameObject.SetActive(false);
        _layout.SetActive(false);

    }

    public void SetMusicVolume(float value)
    {
        float log_value = (value <= 0f) ? -120f : Mathf.Log10(value) * 20;
        MusicSfxManager.Instance.SetVolume(MusicSfxManager.PauseMenuVolume.music, log_value);
    }

    public void SetSfxVolume(float value)
    {
        float log_value = (value <= 0f) ? -120f : Mathf.Log10(value) * 20;
        MusicSfxManager.Instance.SetVolume(MusicSfxManager.PauseMenuVolume.sfx, log_value);
    }
    public void SetMaxDebreeCount(float value)
    {
        Debree.MaxDebrieLimitor = (int)value;
        _debreeCountText.text= Debree.MaxDebrieLimitor.ToString();
    }

    public void QuitGame()
    {
        #if UNITY_STANDALONE_OSX
        Application.Quit();
        #endif
        
#if UNITY_STANDALONE_WIN
        Application.Quit();
#endif

#if UNITY_WEBGL
        ContinueGame();
#endif
    }
}
