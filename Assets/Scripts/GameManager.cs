using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Action PlayerLeveledUp { get; set; }

    [field:SerializeField] public float FuelPercentGUI { get; set; } = 1f; // 0-1f
    
    [field:SerializeField] public float SpeedometerPercentGUI { get; set; } = 1f; // 0-1f
    
    [field:SerializeField] public float LevelUpPercentGUI { get; set; } = 1f; // 0-1f

    [SerializeField] MusicSfxManager _musicManager;
    float _DebreePartsTotalCollected = 0;
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
                _musicManager.RequestCarUpgrade();
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
    List<int> _levelDebreeTresholds = new List<int>() { 100, 200, 300, 400 };
    bool _isLevelingUp;

    void LeveledUp()
    {
        LevelUpPercentGUI = 0;
        _DebreePartsTotalCollected = 0;
        CurrentPlayerLevel++;
        _isLevelingUp = false;
        PlayerLeveledUp?.Invoke();
    }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            _musicManager.StartMusic(1);
            _musicManager.MusicUpgradeHappened += LeveledUp;
        }
            
        // Link / Init UI
        PlayerStatusUI = GetComponent<PlayerStatusUI>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DebreePartsTotalCollected = 0;
            LeveledUp();
        }
    }
}
