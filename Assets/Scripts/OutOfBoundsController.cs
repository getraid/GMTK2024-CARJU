using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OutOfBoundsController : MonoBehaviour
{
    [SerializeField] ActiveCarPrefabSelector _ultimatePlayer;
    public int WhenToResetUpwards = -300;
    public int WhenToResetDownwards = -300;
    public float startingHeightOffset = 5;

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
        if (!(_ultimatePlayer.LatestController.transform.position.y < WhenToResetDownwards) && !(_ultimatePlayer.LatestController.transform.position.y > WhenToResetUpwards)) return;

        _ultimatePlayer.transform.position = startPosition;
        _ultimatePlayer.LatestController.GetRigidBody().velocity = Vector3.zero;

    }
}

