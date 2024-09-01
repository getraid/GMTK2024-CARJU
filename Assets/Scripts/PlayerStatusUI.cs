using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] private List<Sprite> spriteAnim;
    [SerializeField] TMP_Text _debreeCount;
    #endregion
    
    const float speedometerMaxTransformToDeg = -88.8f;

    private const float trashMin = -81.6f;
    private const float trashMax = 8.4f;
    private const float timeTrash= 0.5f;
    private float timeTrashC= 0f;
    private int trashImgIdx= 0;
    private float prevlevelUpPercentage = 0f;
    private float _uiDebreeCount = 0;


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

        if (!Mathf.Approximately(levelUpPercentage, prevlevelUpPercentage))
        {
            if(timeTrashC > timeTrash )
            {
                // animate
                trashImgIdx++;
                trashImgIdx =  trashImgIdx % (spriteAnim.Count);
                Debug.Log(trashImgIdx);
                TrashDynamic.overrideSprite= spriteAnim[trashImgIdx];
                timeTrashC = 0f;
            }
        
        }

        TrashDynamic.transform.localPosition = new Vector3(TrashDynamic.rectTransform.localPosition.x * math.sin(0.2f),
            levelUpPercentage, TrashDynamic.rectTransform.localPosition.z);
        
        // Close to levelup
        var closeToLevelUp = gameManager.IsCloseToLevelUp;
        LevelUp_incoming_glow.gameObject.SetActive(closeToLevelUp);
        
        prevlevelUpPercentage = levelUpPercentage;

        if (_uiDebreeCount > gameManager.DebreePartsTotalCollected)
        {
            _uiDebreeCount = gameManager.DebreePartsTotalCollected;
            _debreeCount.color = Color.white;
            _debreeCount.text = $"{(int)_uiDebreeCount}/{gameManager.LevelDebreeTresholds[gameManager.CurrentPlayerLevel - 1]}";
        }
        else
        {
            int difference = 1 + (int)((gameManager.DebreePartsTotalCollected - _uiDebreeCount) / 20);      //If the UI is not catching on, lets speed it up
            
            float tmp = MathF.Min(gameManager.DebreePartsTotalCollected, _uiDebreeCount + difference);

            if (tmp > gameManager.LevelDebreeTresholds[gameManager.CurrentPlayerLevel - 1])
            {
                _debreeCount.color = Color.green;
                _uiDebreeCount = gameManager.LevelDebreeTresholds[gameManager.CurrentPlayerLevel - 1];
                _debreeCount.text = "LEVEL UP SOON";

            }
            else
            {
                _uiDebreeCount = tmp;
                _debreeCount.text = $"{(int)_uiDebreeCount}/{gameManager.LevelDebreeTresholds[gameManager.CurrentPlayerLevel - 1]}";
            }
        }

        timeTrashC += Time.deltaTime;
    }
}