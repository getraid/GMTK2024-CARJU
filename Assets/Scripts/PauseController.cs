using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] RectTransform pauseMenu;

    bool _isPaused = false;

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

        pauseMenu.gameObject.SetActive(true);
    }

    public void Unstuck()
    {
        ContinueGame();

        VehicleController vehicle = GameManager.Instance.GetActiveVehicle();

        // Move the vehicle up and in a random x/z direction
        vehicle.transform.position = new Vector3(vehicle.transform.position.x + Random.Range(-5, 5), vehicle.transform.position.y + 5, vehicle.transform.position.z + Random.Range(-5, 5));
    }

    public void ContinueGame()
    {
        _isPaused = false;
        Time.timeScale = 1;

        pauseMenu.gameObject.SetActive(false);
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

    public void QuitGame()
    {
#if UNITY_STANDALONE_WIN
        Application.Quit();
#endif

#if UNITY_WEBGL
        ContinueGame();
#endif
    }
}
