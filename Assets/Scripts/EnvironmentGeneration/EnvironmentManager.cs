using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _environmentPieces;
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
                    var colls = Physics.OverlapSphere(_edgesEnvironment[i].transform.position, 50, _groundLayer);


                    //Checking to the left
                    if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.left * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                    {
                        int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                        Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

                        GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.left * _sizeOfTheMapBlock), Quaternion.Euler(pieceRotatedBy), transform);
                        _placedEnvironments.Add(instance, _edgesEnvironment[i]);
                        edgesToAdd.Add(instance);
                    }
                    //Checking to the right
                    if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.right * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                    {
                        int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                        Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

                        GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.right * _sizeOfTheMapBlock), Quaternion.Euler(pieceRotatedBy), transform);
                        _placedEnvironments.Add(instance, _edgesEnvironment[i]);
                        edgesToAdd.Add(instance);
                    }
                    //Checking to the forward
                    if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.forward * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                    {
                        int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                        Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

                        GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.forward * _sizeOfTheMapBlock), Quaternion.Euler(pieceRotatedBy), transform);
                        _placedEnvironments.Add(instance, _edgesEnvironment[i]);
                        edgesToAdd.Add(instance);
                    }
                    //Checking to the back
                    if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.back * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                    {
                        int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                        Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

                        GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.back * _sizeOfTheMapBlock), Quaternion.Euler(pieceRotatedBy), transform);
                        _placedEnvironments.Add(instance, _edgesEnvironment[i]);
                        edgesToAdd.Add(instance);

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

   
}
