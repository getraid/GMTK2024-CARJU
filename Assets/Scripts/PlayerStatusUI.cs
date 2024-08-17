using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject StatusPanelUI;

    [SerializeField] private Image FuelDynamic;

    [SerializeField] private Image SpeedometerDynamic;
    [SerializeField] private Image TrashDynamic;
    [SerializeField] private Image LevelUp_incoming_glow;

    #endregion
    
    const float speedometerMaxTransformToDeg = -88.8f;

    private const float trashMin = -81.6f;
    private const float trashMax = 8.4f;
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        //fallback
        if (!StatusPanelUI || !FuelDynamic || !SpeedometerDynamic || !gameManager)
        {
            if (!StatusPanelUI)
                throw new Exception(nameof(PlayerStatusUI) + " Link the components in Unity!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Fuel
        FuelDynamic.fillAmount = gameManager.FuelPercentGUI;

        // Speedometer
        float inputSpeedometer = Mathf.Clamp(gameManager.SpeedometerPercentGUI, 0, 1f);
        inputSpeedometer *= speedometerMaxTransformToDeg;
        Quaternion speedometerQuat = Quaternion.Euler(0, 0, inputSpeedometer);
        SpeedometerDynamic.transform.rotation = speedometerQuat;
        
        // Trash Level Up 
        float levelUpPercentage = 1 - Mathf.Clamp(gameManager.LevelUpPercentGUI, 0, 1f);
        var distance = trashMin - trashMax;
        levelUpPercentage *= distance;

        TrashDynamic.transform.localPosition = new Vector3(TrashDynamic.rectTransform.localPosition.x,
            levelUpPercentage, TrashDynamic.rectTransform.localPosition.z);
        
        // Close to levelup
        var closeToLevelUp = gameManager.IsCloseToLevelUp;
        LevelUp_incoming_glow.gameObject.SetActive(closeToLevelUp);

    }
}