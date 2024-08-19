using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    
    [SerializeField] float _amountOfFuelRefilled = 0.1f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.CurrentFuelAmount <= GameManager.Instance.MaxFuelAmount)
            {
                GameManager.Instance.CurrentFuelAmount += _amountOfFuelRefilled;
            }
            
            // should the fuel reappear?
            gameObject.SetActive(false);
            MusicSfxManager.Instance.PlaySingleSfx(transform.position, MusicSfxManager.TypeOfSfx.fuel);
        }
    }
}
