using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestModeMover : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float checkpointDistancePadding = 1f;

    [Header("References")]
    [SerializeField] private Transform path;

    private Transform[] _pathNodes;
    private int _currentPointIndex = 0;

    private float currentSpeed = 0f;

    private void Start()
    {
        _pathNodes = new Transform[path.childCount];
        for (int i = 0; i < path.childCount; i++)
        {
            _pathNodes[i] = path.GetChild(i);
        }
    }

    private void Update()
    {
        // Check if we are close to the next waypoint
        if (Vector3.Distance(transform.position, _pathNodes[_currentPointIndex].position) < checkpointDistancePadding)
        {
            _currentPointIndex++;
            if (_currentPointIndex >= _pathNodes.Length)
            {
                _currentPointIndex = 0;
            }
        }

        // Rotate towards the next point
        Vector3 direction = _pathNodes[_currentPointIndex].position - transform.position;
        Quaternion target_rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, rotationSpeed * Time.deltaTime);

        // Move forward
        currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
}
