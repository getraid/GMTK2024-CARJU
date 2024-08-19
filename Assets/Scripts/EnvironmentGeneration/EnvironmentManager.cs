using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] GameObject _environmentPiece;
    [SerializeField] List<GameObject> _edgesEnvironment = new List<GameObject>();
    [SerializeField] ActiveCarPrefabSelector _ultimatePlayer;
    [SerializeField] float _sizeOfTheMapBlock = 220;
    [SerializeField] int _howManyToPrepool = 100;
    [SerializeField] GameObject _poolParent;

    Dictionary<GameObject,GameObject> _placedEnvironments = new Dictionary<GameObject, GameObject>();
    List<GameObject> _objectPool=new List<GameObject>();
    int _poolIndex = 0;

    int _groundLayer = 1 << 6;
    float _minimumEdgeDistance = 220;
    float _maximumDespawnEdgeDistance = 520;
    float _mapCheckingDelaySeconds = 0.2f;
    private void Awake()
    {
        foreach (var item in _edgesEnvironment)
            _placedEnvironments.Add(item.gameObject, null);

        StartCoroutine(UpdateLoop());

        for(int i=0;i<_howManyToPrepool;i++)
        {
            GameObject poolObj = Instantiate(_environmentPiece, _poolParent.transform);
            _objectPool.Add(poolObj);
        }
    }
    IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_mapCheckingDelaySeconds);

            List<GameObject> edgesToRemove = new List<GameObject>();
            List<GameObject> edgesToAdd = new List<GameObject>();
            List<GameObject> objectsToDestroy = new List<GameObject>();
            for (int i = 0; i < _edgesEnvironment.Count; i++)
            {
                try
                {
                    float edgeDistance = Vector3.Distance(_ultimatePlayer.LatestController.transform.position, _edgesEnvironment[i].transform.position);

                    if (edgeDistance < _minimumEdgeDistance)
                    {

                        if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.left * _sizeOfTheMapBlock), out GameObject leftInstance))
                        {
                            _placedEnvironments.Add(leftInstance, _edgesEnvironment[i]);
                            edgesToAdd.Add(leftInstance);
                        }
                        if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.right * _sizeOfTheMapBlock), out GameObject rightInstance))
                        {
                            _placedEnvironments.Add(rightInstance, _edgesEnvironment[i]);
                            edgesToAdd.Add(rightInstance);
                        }
                        if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.forward * _sizeOfTheMapBlock), out GameObject forwardInstance))
                        {
                            _placedEnvironments.Add(forwardInstance, _edgesEnvironment[i]);
                            edgesToAdd.Add(forwardInstance);
                        }
                        if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.back * _sizeOfTheMapBlock), out GameObject backInstance))
                        {
                            _placedEnvironments.Add(backInstance, _edgesEnvironment[i]);
                            edgesToAdd.Add(backInstance);
                        }

                        edgesToRemove.Add(_edgesEnvironment[i]);
                    }
                    else if (edgeDistance > _maximumDespawnEdgeDistance)
                    {
                        GameObject lastConnection = null;

                        if (_placedEnvironments.ContainsKey(_edgesEnvironment[i]))
                            lastConnection = _placedEnvironments[_edgesEnvironment[i]];

                        if (lastConnection != null && !_edgesEnvironment.Contains(lastConnection) && !edgesToAdd.Contains(lastConnection))
                        {
                            edgesToAdd.Add(lastConnection);
                        }

                        edgesToRemove.Add(_edgesEnvironment[i]);
                        objectsToDestroy.Add(_edgesEnvironment[i].gameObject);
                    }
                }
                catch(Exception e)
                {
                    Debug.Log("Some shit happened in tile map generation");
                }
            }
            edgesToRemove.ForEach(x => _edgesEnvironment.Remove(x));
            edgesToAdd.ForEach(x => _edgesEnvironment.Add(x));
            objectsToDestroy.ForEach(x =>
            {
                x.transform.parent = _poolParent.transform;
                _placedEnvironments.Remove(x);

                x.SetActive(false);
            });
        }
    }
    bool CheckDirectionForInstantiation(Vector3  position, out GameObject instance)
    {
        instance = null;
        if (Physics.OverlapSphere(position, 50, _groundLayer).Length == 0)
        {
            Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

            instance = _objectPool[GetNextPoolIndex()];
            instance.transform.SetPositionAndRotation(position, Quaternion.Euler(pieceRotatedBy));
            instance.transform.parent = transform;
            instance.SetActive(true);

            //instance = Instantiate(_environmentPiece, position, Quaternion.Euler(pieceRotatedBy), transform);


            return true;
        }
        else
            return false;
    }
    int GetNextPoolIndex()
    {
        _poolIndex++;
        if (_poolIndex >= _objectPool.Count)
            _poolIndex = 0;

        return _poolIndex;
    }
   
}
