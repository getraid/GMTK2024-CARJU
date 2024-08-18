using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _environmentPieces;
    [SerializeField] List<GameObject> _edgesEnvironment = new List<GameObject>();
    [SerializeField] GameObject _player;
    [SerializeField] float _sizeOfTheMapBlock = 220;

    List<GameObject> _placedEnvironments = new List<GameObject>();

    int _groundLayer = 1 << 6;
    float _minimumEdgeDistance = 220;

    private void Awake()
    {
        _placedEnvironments.AddRange(_edgesEnvironment);
    }
    private void FixedUpdate()
    {
        List<GameObject> edgesToRemove = new List<GameObject>();
        List<GameObject> edgesToAdd = new List<GameObject>();
        for (int i=0;i< _edgesEnvironment.Count; i++)
        {
            float edgeDistance = Vector3.Distance(_player.transform.position, _edgesEnvironment[i].transform.position);
            if (edgeDistance < _minimumEdgeDistance)
            {
                var colls= Physics.OverlapSphere(_edgesEnvironment[i].transform.position, 50,_groundLayer);


                    //Checking to the left
                    if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position+(Vector3.left* _sizeOfTheMapBlock),50, _groundLayer).Length==0)
                {
                    int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                    GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.left * _sizeOfTheMapBlock), Quaternion.identity);
                    _placedEnvironments.Add(instance);
                    edgesToAdd.Add(instance);
                }
                //Checking to the right
                if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.right * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                {
                    int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                    GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.right * _sizeOfTheMapBlock), Quaternion.identity);
                    _placedEnvironments.Add(instance);
                    edgesToAdd.Add(instance);
                }
                //Checking to the forward
                if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.forward * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                {
                    int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                    GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.forward * _sizeOfTheMapBlock), Quaternion.identity);
                    _placedEnvironments.Add(instance);
                    edgesToAdd.Add(instance);
                }
                //Checking to the back
                if (Physics.OverlapSphere(_edgesEnvironment[i].transform.position + (Vector3.back * _sizeOfTheMapBlock), 50, _groundLayer).Length == 0)
                {
                    int envPieceIndex = Random.Range(0, _environmentPieces.Count);
                    GameObject instance = Instantiate(_environmentPieces[0], _edgesEnvironment[i].transform.position + (Vector3.back * _sizeOfTheMapBlock), Quaternion.identity);
                    _placedEnvironments.Add(instance);
                    edgesToAdd.Add(instance);

                }
                edgesToRemove.Add(_edgesEnvironment[i]);
            }
        }
        edgesToRemove.ForEach(x=> _edgesEnvironment.Remove(x));
        edgesToAdd.ForEach(x=>_edgesEnvironment.Add(x));
    }
}
