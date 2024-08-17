using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    [SerializeField] float _amountOfFuelRefilled = 0.1f;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.FuelPercentGUI+=_amountOfFuelRefilled;
        gameObject.SetActive(false);
    }
}
