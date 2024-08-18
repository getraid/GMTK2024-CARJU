using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCarPrefabSelector : MonoBehaviour
{
    [SerializeField] List<VehicleController> _vehicleLevels;

    [field:SerializeField] public VehicleController LatestController { get; set; }
    void Start()
    {
        GameManager.Instance.PlayerLeveledUp += OnPlayerLeveledUp;
    }
    void OnPlayerLeveledUp()
    {
        VehicleController newActiveController = _vehicleLevels[GameManager.Instance.CurrentPlayerLevel - 1];


        newActiveController.gameObject.transform.SetPositionAndRotation(LatestController.gameObject.transform.position, LatestController.gameObject.transform.rotation);
        newActiveController.gameObject.SetActive(true);
        newActiveController.TransferRigidBodyParameters(LatestController.GetRigidBody());

        LatestController.gameObject.SetActive(false);
        LatestController = newActiveController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
