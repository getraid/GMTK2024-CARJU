using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    [SerializeField] private List<PoliceLogic> policePrefabs;

    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float repositionDistance = 100f;
    [SerializeField] private int maxPoliceCars = 0;
    [SerializeField] private float heightCheck = 100f;

    [SerializeField] private LayerMask groundLayer;

    private Transform target;
    RaycastHit hit;

    private List<PoliceLogic> _activePoliceCars = new List<PoliceLogic>();

    private void Update()
    {
        if (target == null)
            return;

        // Check if a Police Car is Stuck
        for (int i = 0; i < _activePoliceCars.Count; i++)
        {
            // Is Stuck or too far from target
            if (_activePoliceCars[i].IsStuck() || _activePoliceCars[i].DistanceToTarget() >= repositionDistance)
            {
                RespawnPoliceCar(_activePoliceCars[i]);
            }
        }

        if (_activePoliceCars.Count < maxPoliceCars)
        {
            SpawnPolice();
        }
    }

    private void SpawnPolice()
    {
        if (GetValidSpawnPosition(out Vector3 spawn_position))
        {
            // Spawn the police car
            PoliceLogic selected_car = policePrefabs[Random.Range(0, policePrefabs.Count)];
            PoliceLogic police_car = Instantiate(selected_car, spawn_position, Quaternion.identity);
            police_car.SetTarget(target);

            _activePoliceCars.Add(police_car);

            MusicSfxManager.Instance.TryPoliceSfx();
        }
    }

    private bool GetValidSpawnPosition(out Vector3 spawn_position)
    {
        float angle = Random.Range(0, 360);
        spawn_position = target.position + new Vector3(Mathf.Cos(angle) * spawnRadius, heightCheck, Mathf.Sin(angle) * spawnRadius);

        bool is_valid = false;

        // Raycast to check if the spawn position is valid
        if (Physics.Raycast(spawn_position, Vector3.down, out hit, heightCheck + 1f))
        {
            // Check if the hit collider is in the ground layermask
            if (groundLayer == (groundLayer | (1 << hit.collider.gameObject.layer)))
            {
                spawn_position = hit.point;
                is_valid = true;
            }
        }

        // Escape if not a valid spawn hit
        if (!is_valid)
        {
            return false;
        }


        return true;
    }

    private void RespawnPoliceCar(PoliceLogic police_car)
    {
        if (GetValidSpawnPosition(out Vector3 spawn_position))
        {
            police_car.transform.position = spawn_position;
            police_car.transform.rotation = Quaternion.identity;
            police_car.ResetIsStuck();
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;

        // Update all active police cars
        for (int i = 0; i < _activePoliceCars.Count; i++)
        {
            _activePoliceCars[i].SetTarget(target);
        }
    }

    public void SetMaxSpawnCount(int maxPoliceCars)
    {
        this.maxPoliceCars = maxPoliceCars;
    }
}
