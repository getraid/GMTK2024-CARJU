using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Action PlayerLeveledUp { get; set; }

    public float FuelPercentGUI { get; set; } = 0f; // 0-1f
    
    public float SpeedometerPercentGUI { get; set; } = 0f; // 0-1f
    
    public float LevelUpPercentGUI { get; set; } = 0f; // 0-1f

    public float fuelDrainagePerSecond { get; set; } = 1f;


    float _DebreePartsTotalCollected = 0;


    [field: SerializeField] public float CurrentFuelAmount { get; set; } = 50;
    [field: SerializeField] public float MaxFuelAmount { get; set; } = 100;


    [SerializeField] List<Material> _skyboxesMaterials;

    private VehicleController _activeVehicle;

    public float DebreePartsTotalCollected 
    {
        get 
        {
            return _DebreePartsTotalCollected;
        }
        set
        {

            _DebreePartsTotalCollected = value;
            LevelUpPercentGUI = _DebreePartsTotalCollected / (float)LevelDebreeTresholds[CurrentPlayerLevel - 1];
            if (IsCloseToLevelUp && !_isLevelingUp)
            {
                _isLevelingUp = true;
                MusicSfxManager.Instance.RequestCarUpgrade();
            }
        }
    }
    public bool IsCloseToLevelUp
    {
        get
        {
            return LevelUpPercentGUI >= 0.95;
        }
    }
    int _CurrentPlayerLevel = 1;
    public int CurrentPlayerLevel
    {
        get { return _CurrentPlayerLevel; }
        set { _CurrentPlayerLevel = Math.Clamp(value,1,5); }
    }
    public List<int> LevelDebreeTresholds { get; set; } = new List<int>() { 50, 750, 2000, 5000, 10000 };




    private PlayerStatusUI PlayerStatusUI;
    bool _isLevelingUp;



    public void LeveledUp()
    {
        LevelUpPercentGUI = 0;
        _DebreePartsTotalCollected = 0;
        CurrentPlayerLevel++;
        _isLevelingUp = false;
        PlayerLeveledUp?.Invoke();
        RenderSettings.skybox = _skyboxesMaterials[CurrentPlayerLevel - 1];
    }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
            
        // Link / Init UI
        PlayerStatusUI = GetComponent<PlayerStatusUI>();

    }

    [FormerlySerializedAs("timer")] public float timer_fueldepletion = 0f;
    private void Update()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            DebreePartsTotalCollected = 0;
            LeveledUp();
        }
    #endif


        if (timer_fueldepletion >= 1f)
        {
            if(CurrentPlayerLevel == 1)
                CurrentFuelAmount -= (0.2f);
            else if(CurrentPlayerLevel == 2)
                CurrentFuelAmount -= (1f);
            else if(CurrentPlayerLevel == 3)
                CurrentFuelAmount -= (2f);
            else if(CurrentPlayerLevel == 4)
                CurrentFuelAmount -= (3f);
            else 
                CurrentFuelAmount -= (4f);
            
            
            timer_fueldepletion = 0f;
        }
        // fuel ui update
        FuelPercentGUI = (CurrentFuelAmount / MaxFuelAmount);



        if (CurrentPlayerLevel == 5 &&
            (_DebreePartsTotalCollected / (float)LevelDebreeTresholds[CurrentPlayerLevel - 1] >= 0.99f))
        {

            SceneManager.LoadScene(2);
        }
        

        timer_fueldepletion += Time.deltaTime;
    }

    public VehicleController GetActiveVehicle()
    {
        return _activeVehicle;
    }

    public void SetActiveVehicle(VehicleController vehicle)
    {
        _activeVehicle = vehicle;
    }
}
