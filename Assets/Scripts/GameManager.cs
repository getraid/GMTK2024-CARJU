using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Action PlayerLeveledUp { get; set; }

    [field:SerializeField] public float FuelPercentGUI { get; set; } = 0f; // 0-1f
    
    [field:SerializeField] public float SpeedometerPercentGUI { get; set; } = 1f; // 0-1f
    
    [field:SerializeField] public float LevelUpPercentGUI { get; set; } = 1f; // 0-1f
    
    float _DebreePartsTotalCollected = 0;


    [field: SerializeField] public float CurrentFuelAmount { get; set; } = 50;
    [field: SerializeField] public float MaxFuelAmount { get; set; } = 100;

    public float fuelDrainagePerSecond = 1f;

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
            LevelUpPercentGUI = _DebreePartsTotalCollected / (float)_levelDebreeTresholds[CurrentPlayerLevel - 1];
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
    [field: SerializeField] public int CurrentPlayerLevel { get; set; } = 1;



    private PlayerStatusUI PlayerStatusUI;
    [SerializeField] List<int> _levelDebreeTresholds = new List<int>() { 500, 1000, 2000, 3000,4000 };
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

    public float timer = 0f;
    private void Update()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            DebreePartsTotalCollected = 0;
            LeveledUp();
        }
    #endif


        if (timer >= 1f)
        {
            CurrentFuelAmount -= ((float)CurrentPlayerLevel * fuelDrainagePerSecond);
            timer = 0f;
        }
        // fuel ui update
        FuelPercentGUI = (CurrentFuelAmount / MaxFuelAmount);

        

        timer += Time.deltaTime;
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
