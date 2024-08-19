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

    Dictionary<GameObject,GameObject> _placedEnvironments = new Dictionary<GameObject, GameObject>();

    int _groundLayer = 1 << 6;
    float _minimumEdgeDistance = 220;
    float _maximumDespawnEdgeDistance = 520;

    private void Awake()
    {
        foreach (var item in _edgesEnvironment)
            _placedEnvironments.Add(item.gameObject, null);
    }
    private void FixedUpdate()
    {
        List<GameObject> edgesToRemove = new List<GameObject>();
        List<GameObject> edgesToAdd = new List<GameObject>();
        List<GameObject> objectsToDestroy=new List<GameObject>();
        for (int i=0;i< _edgesEnvironment.Count; i++)
        {
            try
            {
                float edgeDistance = Vector3.Distance(_ultimatePlayer.LatestController.transform.position, _edgesEnvironment[i].transform.position);

                if (edgeDistance < _minimumEdgeDistance)
                {

                    if(CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.left * _sizeOfTheMapBlock), out GameObject leftInstance))
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
                    GameObject lastConnection = _placedEnvironments[_edgesEnvironment[i]];

                    if (!_edgesEnvironment.Contains(lastConnection) && !edgesToAdd.Contains(lastConnection))
                        edgesToAdd.Add(lastConnection);

                    edgesToRemove.Add(_edgesEnvironment[i]);
                    objectsToDestroy.Add(_edgesEnvironment[i].gameObject);
                }
            }
            catch
            {
                Debug.Log("Some shit happened in tile map generation");
            }
        }
        edgesToRemove.ForEach(x=> _edgesEnvironment.Remove(x));
        edgesToAdd.ForEach(x=>_edgesEnvironment.Add(x));
        objectsToDestroy.ForEach(x => x.SetActive(false)) ;
    }
    bool CheckDirectionForInstantiation(Vector3  position, out GameObject instance)
    {
        instance = null;
        if (Physics.OverlapSphere(position, 50, _groundLayer).Length == 0)
        {
            Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

            instance = Instantiate(_environmentPiece, position, Quaternion.Euler(pieceRotatedBy), transform);
            return true;
        }
        else
            return false;
    }
   
}
