using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCarPrefabSelector : MonoBehaviour
{
    [SerializeField] List<VehicleController> _vehicleLevels;

    VehicleController _latestController;
    void Start()
    {
        _latestController = _vehicleLevels[0];
        GameManager.Instance.PlayerLeveledUp += OnPlayerLeveledUp;
    }
    void OnPlayerLeveledUp()
    {
        VehicleController newActiveController = _vehicleLevels[GameManager.Instance.CurrentPlayerLevel - 1];


        newActiveController.gameObject.transform.SetPositionAndRotation(_latestController.gameObject.transform.position, _latestController.gameObject.transform.rotation);
        newActiveController.gameObject.SetActive(true);
        newActiveController.TransferRigidBodyParameters(_latestController.GetRigidBody());

        _latestController.gameObject.SetActive(false);
        _latestController = newActiveController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
