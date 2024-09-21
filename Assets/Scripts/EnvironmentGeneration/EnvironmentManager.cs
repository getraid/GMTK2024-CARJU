using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] GameObject _environmentPiece;
    [SerializeField] GameObject _emptyPiece;
    [SerializeField] ActiveCarPrefabSelector _ultimatePlayer;
    [SerializeField] float _sizeOfTheMapBlock = 220;
    [SerializeField] int _howManyToPrepool = 50;
    [SerializeField] GameObject _startingArea; 
    [SerializeField] GameObject LoadingBg;
    [SerializeField] GameObject StatusPanelUI;
    [SerializeField] TMP_Text _loadingText;
    [SerializeField] UnityEngine.UI.Image loadingIcon;
    
    Dictionary<Vector2, PoolValue> _placedEnvironmentsMap = new Dictionary<Vector2, PoolValue>();
    Queue<PoolValue> _objectPool=new Queue<PoolValue>();

    public class PoolValue
    {
        public GameObject PoolObject { get; set; }
        public bool CanBeReused { get; set; }
    }

    int _poolIndex = 0;

    float _minimumTileDistance = 300;
    float _maximumDespawnTileDistance = 520;
    float _mapCheckingDelaySeconds = 0.1f;

    Queue<Tuple<PoolValue, bool>> _activationCommands = new Queue<Tuple<PoolValue, bool>>();

    IEnumerator Start()
    {
        Time.timeScale = 0;
        PoolValue startingPool = new PoolValue() { PoolObject = _startingArea, CanBeReused = true };
        _placedEnvironmentsMap.Add(new Vector2(0, 0), startingPool);
        LoadingBg.SetActive(true);
        StatusPanelUI.SetActive(false); 
        
    
        loadingIcon.gameObject.SetActive(true);
        for (int i = 0; i < _howManyToPrepool; i++)
        {
            _loadingText.text = $"Spawning...\n{i+1}/{_howManyToPrepool}";
            GameObject poolObj = Instantiate(_environmentPiece, transform);
            poolObj.SetActive(false);
            poolObj.name = i.ToString();
            _objectPool.Enqueue(new PoolValue() { PoolObject = poolObj, CanBeReused = true });

            if(loadingIcon != null)
                if (loadingIcon.gameObject.activeSelf)
                    loadingIcon.rectTransform.rotation = Quaternion.Euler(0,0,i*20);
            
            yield return null;
        }
        StartCoroutine(UpdateLoop());
        StartCoroutine(CommandQueue());
        LoadingBg.SetActive(false);
        _loadingText.enabled = false;
        loadingIcon.gameObject.SetActive(false);
        StatusPanelUI.SetActive(true); 
 
        Time.timeScale = 1;
    }



    IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_mapCheckingDelaySeconds);

            try
            {
                List<KeyValuePair<Vector2, PoolValue>> toGoThru = _placedEnvironmentsMap.ToList();

                foreach (KeyValuePair<Vector2, PoolValue> tile in toGoThru)
                { 
                    try
                    {
                        float tileDistance = Vector3.Distance(_ultimatePlayer.LatestController.transform.position, tile.Value.PoolObject.transform.position);

                        Vector2 left = tile.Key + new Vector2(-_sizeOfTheMapBlock, 0);
                        Vector2 right = tile.Key + new Vector2(_sizeOfTheMapBlock, 0);
                        Vector2 up = tile.Key + new Vector2(0, _sizeOfTheMapBlock);
                        Vector2 down = tile.Key + new Vector2(0, -_sizeOfTheMapBlock);

                        if (tileDistance < _minimumTileDistance)
                        {
                            if(!_placedEnvironmentsMap.ContainsKey(left))
                            {
                                Vector3 newPos=new Vector3(left.x,0, left.y);

                                PoolValue til= SetNewTilePositionAndRotation(newPos, new Vector3(0, Random.Range(0, 3) * 90, 0));
                                _placedEnvironmentsMap.Add(left, til);

                                _activationCommands.Enqueue(new(til, true));
                            }

                            if (!_placedEnvironmentsMap.ContainsKey(right))
                            {
                                Vector3 newPos = new Vector3(right.x, 0, right.y);

                                PoolValue til = SetNewTilePositionAndRotation(newPos, new Vector3(0, Random.Range(0, 3) * 90, 0));
                                _placedEnvironmentsMap.Add(right, til);

                                _activationCommands.Enqueue(new(til, true));
                            }

                            if (!_placedEnvironmentsMap.ContainsKey(up))
                            {
                                Vector3 newPos = new Vector3(up.x, 0, up.y);

                                PoolValue til = SetNewTilePositionAndRotation(newPos, new Vector3(0, Random.Range(0, 3) * 90, 0));
                                _placedEnvironmentsMap.Add(up, til);

                                _activationCommands.Enqueue(new(til, true));
                            }

                            if (!_placedEnvironmentsMap.ContainsKey(down))
                            {
                                Vector3 newPos = new Vector3(down.x, 0, down.y);

                                PoolValue til = SetNewTilePositionAndRotation(newPos, new Vector3(0, Random.Range(0, 3) * 90, 0));
                                _placedEnvironmentsMap.Add(down, til);

                                _activationCommands.Enqueue(new(til, true));
                            }
                        }
                        else if (tileDistance > _maximumDespawnTileDistance)
                        {
                            if(!_placedEnvironmentsMap.Remove(tile.Key))
                            {

                            }
                            _activationCommands.Enqueue(new(tile.Value, false));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Some shit happened in tile map generation");
                    }
                }
            }
            catch(Exception ex)
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
                if (_activationCommands.TryDequeue(out Tuple<PoolValue, bool> activate))
                {
                    if(activate.Item2)
                        activate.Item1.PoolObject.SetActive(true);
                    else
                    {
                        activate.Item1.PoolObject.SetActive(false);
                        _objectPool.Enqueue(activate.Item1);
                    }
                }
            }
            catch
            {
                Debug.Log("Error in command queue");
            }
        }
    }
    PoolValue SetNewTilePositionAndRotation(Vector3 pos,Vector3 rotation)
    {
        PoolValue poolObj = GetNextPool();

        poolObj.PoolObject.transform.SetPositionAndRotation(pos, Quaternion.Euler(rotation));
        return poolObj;
    }
    PoolValue GetNextPool()
    {
        if (_objectPool.TryDequeue(out PoolValue res))
            return res;
        else
        {
            GameObject newObj= Instantiate(_emptyPiece, transform);
            PoolValue newPoolObj = new PoolValue() { PoolObject = newObj, CanBeReused = false };

            return newPoolObj;
        }
    }
}
