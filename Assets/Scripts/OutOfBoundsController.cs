using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OutOfBoundsController : MonoBehaviour
{
    public Rigidbody Player;
    public int WhenToResetUpwards = -300;
    public int WhenToResetDownwards = -300;
    public float startingHeightOffset = 5;

    private Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = Player.transform.position;
        startPosition.y += startingHeightOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(Player.position.y < WhenToResetDownwards) && !(Player.position.y > WhenToResetUpwards)) return;
        
        Player.transform.position = startPosition;
        Player.velocity = Vector3.zero;

    }
}

