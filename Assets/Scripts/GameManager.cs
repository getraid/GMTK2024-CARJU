using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] 
    public float FuelPercentGUI = 1f; // 0-1f
    
    [SerializeField] 
    public float SpeedometerPercentGUI = 1f; // 0-1f
    
    [SerializeField] 
    public float LevelUpPercentGUI = 1f; // 0-1f
    
    [SerializeField] 
    public bool IsCloseToLevelUp = false; // 0-1f
    
    private PlayerStatusUI PlayerStatusUI;
    public int CurrentPlayerLevel { get; set; } = 1;

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
