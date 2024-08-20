using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCarPrefabSelector : MonoBehaviour
{
    [SerializeField] List<VehicleController> _vehicleLevels;
    [SerializeField] List<_CameraSettings> _cameraSettings;

    [SerializeField] private PlayerInput playerInputRef;
    [SerializeField] private CameraController cameraControllerRef;

    [SerializeField] private int policePerLevel = 1;


    [SerializeField] private Transform levelVFX;

    private PoliceSpawner _policeSpawner;

    private void Awake()
    {
        _policeSpawner = GetComponent<PoliceSpawner>();
    }

    [field:SerializeField] public VehicleController LatestController { get; set; }
    public Vector3 LatestVehicleStartingLocation { get; set; }
    void Start()
    {
        GameManager.Instance.PlayerLeveledUp += OnPlayerLeveledUp;

        playerInputRef.SetControlledVehicle(LatestController);
        cameraControllerRef.SetTarget(LatestController.transform, _cameraSettings[GameManager.Instance.CurrentPlayerLevel - 1]);
        _policeSpawner.SetTarget(LatestController.transform);

        GameManager.Instance.SetActiveVehicle(LatestController);
    }
    void OnPlayerLeveledUp()
    {
        VehicleController newActiveController = _vehicleLevels[GameManager.Instance.CurrentPlayerLevel - 1];
        LatestVehicleStartingLocation = LatestController.transform.position;

        newActiveController.gameObject.transform.SetPositionAndRotation(LatestController.gameObject.transform.position, LatestController.gameObject.transform.rotation);
        newActiveController.gameObject.SetActive(true);
        newActiveController.TransferRigidBodyParameters(LatestController.GetRigidBody());

        LatestController.gameObject.SetActive(false);
        LatestController = newActiveController;

        playerInputRef.SetControlledVehicle(LatestController);
        cameraControllerRef.SetTarget(LatestController.transform, _cameraSettings[GameManager.Instance.CurrentPlayerLevel - 1]);
        _policeSpawner.SetTarget(LatestController.transform);

        GameManager.Instance.SetActiveVehicle(LatestController);

        // 0, 2, 4, 6
        int spawn_count = policePerLevel * (GameManager.Instance.CurrentPlayerLevel - 1);
        _policeSpawner.SetMaxSpawnCount(spawn_count);

        if (levelVFX)
        {
            Transform instance = Instantiate(levelVFX, LatestController.transform.position, Quaternion.identity);
            instance.localScale = Vector3.one * GameManager.Instance.CurrentPlayerLevel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
