using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] GameObject _environmentPiece;
    [SerializeField] GameObject _emptyPiece;
    [SerializeField] List<GameObject> _edgesEnvironment = new List<GameObject>();
    [SerializeField] ActiveCarPrefabSelector _ultimatePlayer;
    [SerializeField] float _sizeOfTheMapBlock = 220;
    [SerializeField] int _howManyToPrepool = 50;

    public Action FinishedLoading { get; set; }
    Dictionary<GameObject,GameObject> _placedEnvironments = new Dictionary<GameObject, GameObject>();
    List<Tuple<GameObject,bool>> _objectPool=new List<Tuple<GameObject,bool>>();
    int _poolIndex = 0;

    int _groundLayer = 1 << 6;
    float _minimumEdgeDistance = 220;
    float _maximumDespawnEdgeDistance = 520;
    float _mapCheckingDelaySeconds = 0.2f;

    Queue<Tuple<GameObject, bool>> _activationCommands = new Queue<Tuple<GameObject, bool>>();

    private void Awake()
    {
        foreach (var item in _edgesEnvironment)
            _placedEnvironments.Add(item.gameObject, null);


        for(int i=0;i<_howManyToPrepool;i++)
        {
            GameObject poolObj = Instantiate(_environmentPiece, transform);
            _objectPool.Add(new(poolObj,true));
        }

        StartCoroutine(UpdateLoop());
        StartCoroutine(CommandQueue());
    }
    private void Start()
    {
        _objectPool.ForEach(x => x.Item1.SetActive(false));

        FinishedLoading?.Invoke();
    }
    IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_mapCheckingDelaySeconds);

            try
            {

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
                                _activationCommands.Enqueue(new(leftInstance, true));
                            }
                            if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.right * _sizeOfTheMapBlock), out GameObject rightInstance))
                            {
                                _placedEnvironments.Add(rightInstance, _edgesEnvironment[i]);
                                edgesToAdd.Add(rightInstance);
                                _activationCommands.Enqueue(new(rightInstance, true));

                            }
                            if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.forward * _sizeOfTheMapBlock), out GameObject forwardInstance))
                            {
                                _placedEnvironments.Add(forwardInstance, _edgesEnvironment[i]);
                                edgesToAdd.Add(forwardInstance);
                                _activationCommands.Enqueue(new(forwardInstance, true));

                            }
                            if (CheckDirectionForInstantiation(_edgesEnvironment[i].transform.position + (Vector3.back * _sizeOfTheMapBlock), out GameObject backInstance))
                            {
                                _placedEnvironments.Add(backInstance, _edgesEnvironment[i]);
                                edgesToAdd.Add(backInstance);
                                _activationCommands.Enqueue(new(backInstance, true));

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
                    catch (Exception e)
                    {
                        Debug.Log("Some shit happened in tile map generation");
                    }
                }
                edgesToRemove.ForEach(x => _edgesEnvironment.Remove(x));
                edgesToAdd.ForEach(x => _edgesEnvironment.Add(x));
                objectsToDestroy.ForEach(x =>
                {
                    _placedEnvironments.Remove(x);

                    _activationCommands.Enqueue(new(x, false));
                });
            }
            catch
            {
                Debug.Log("Some shit happened in tile map generation");
            }
        }
    }
  
    private IEnumerator CommandQueue()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            try
            {
                if (_activationCommands.TryDequeue(out Tuple<GameObject, bool> activate))
                    activate.Item1.SetActive(activate.Item2);
            }
            catch
            {

            }
        }
    }
    bool CheckDirectionForInstantiation(Vector3  position, out GameObject instance)
    {
        instance = null;
        if (Physics.OverlapSphere(position, 50, _groundLayer).Length == 0)
        {
            Vector3 pieceRotatedBy = new Vector3(0, Random.Range(0, 3) * 90, 0);

            instance = _objectPool[GetNextPoolIndex()].Item1;

            if(instance.activeSelf)
            {
                instance = Instantiate(_emptyPiece, position, Quaternion.Euler(pieceRotatedBy), transform);
                _objectPool.Add(new(instance,false));
            }
            else
            {
                instance.transform.SetPositionAndRotation(position, Quaternion.Euler(pieceRotatedBy));
                instance.SetActive(true);
            }
            

            //instance = Instantiate(_environmentPiece, position, Quaternion.Euler(pieceRotatedBy), transform);


            return true;
        }
        else
            return false;
    }
    int GetNextPoolIndex()
    {

        for(int i= 0;i< _objectPool.Count;i++)
        {
            _poolIndex++;
            if (_poolIndex >= _objectPool.Count)
                _poolIndex = 0;

            if (_objectPool[_poolIndex].Item2&&!_objectPool[_poolIndex].Item1.activeSelf)
                return _poolIndex;
        }
        return _poolIndex;

    }

}
