using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field:SerializeField] public float FuelPercentGUI { get; set; } = 1f; // 0-1f
    
    [field:SerializeField] public float SpeedometerPercentGUI { get; set; } = 1f; // 0-1f
    
    [field:SerializeField] public float LevelUpPercentGUI { get; set; } = 1f; // 0-1f

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
                StartCoroutine(DoLevelUp());
        }
    }
    public bool IsCloseToLevelUp
    {
        get
        {
            return LevelUpPercentGUI >= 0.95;
        }
    }
    public int CurrentPlayerLevel { get; set; } = 1;


    private PlayerStatusUI PlayerStatusUI;
    List<int> _levelDebreeTresholds = new List<int>() { 100, 200, 300, 400 };
    bool _isLevelingUp;
    IEnumerator DoLevelUp()
    {
        _isLevelingUp = true;
        yield return new WaitForSeconds(5);
        LevelUpPercentGUI = 0;
        _DebreePartsTotalCollected = 0;
        CurrentPlayerLevel++;
        _isLevelingUp = false;
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
        }
            
        // Link / Init UI
        PlayerStatusUI = GetComponent<PlayerStatusUI>();

    }
}
