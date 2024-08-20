using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OutOfBoundsController : MonoBehaviour
{
    [SerializeField] ActiveCarPrefabSelector _ultimatePlayer;
     int WhenToResetUpwards = 100;
     int WhenToResetDownwards = -30;
     float startingHeightOffset = 5;

    private Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = _ultimatePlayer.LatestController.transform.position;
        startPosition.y += startingHeightOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if ((_ultimatePlayer.LatestController.transform.position.y < WhenToResetDownwards) || (_ultimatePlayer.LatestController.transform.position.y > WhenToResetUpwards))
        {
            Vector3 pos = _ultimatePlayer.LatestVehicleStartingLocation;
            pos.y += startingHeightOffset;

            _ultimatePlayer.LatestController.transform.position = pos;
            _ultimatePlayer.LatestController.GetRigidBody().velocity = Vector3.zero;
        }
    }
}

