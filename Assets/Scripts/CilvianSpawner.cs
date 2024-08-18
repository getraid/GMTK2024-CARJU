using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Interfaces;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

[Serializable]
public struct Spawnable {
    [Header("This can only be set on init")]
    public GameObject type;
    public int count;
    public float3 spawn_offset;
    public Vector3 scale;
    public float moveSpeed;
}


[Serializable]
public class GoWithIndex {
    public GameObject currentGameObject;
    public int knot;
    public Spawnable typeInfo;
    public float localTimer;
    public float timeToCompleteTheLoop;
    
    public GoWithIndex(GameObject gameobj, int knotI, Spawnable typeInfoS)
    {
        currentGameObject = gameobj;
        knot = knotI;
        typeInfo = typeInfoS;
        localTimer = 0;
        timeToCompleteTheLoop = 5f;
    }
}


public class CilvianSpawner : MonoBehaviour
{
    private Transform CivilianContainer;
    [Header("Create a spline as a child and drag it in here!")]
    public SplineContainer SplineContainer;
    private Spline Path;

    public List<Spawnable> Spawnables; //type,count
    public bool ShouldMoveAlongPath = true;
    
    private List<GoWithIndex> currentGameObjects;

    
    
    // Start is called before the first frame update
    void Start()
    {
        // can be set freely if you want to
        CivilianContainer = this.transform;
        
        // only use first spline for now
        Path = SplineContainer[0];
        
        // Holds all current objs on the track
        currentGameObjects = new List<GoWithIndex>();

        SpawnAllEntries();
        
        FindDestroyablesAndSubscribe();
    }

    // I know, much copying and pasting, but idc tbh.
    private void FindDestroyablesAndSubscribe()
    {
        var destroyable = FetchDestroyablesFromCurrentObjects();
        destroyable.DestructionEvent += InterfOnDestructionEvent;   
    }

    private IDestroyable FetchDestroyablesFromCurrentObjects()
    {
        foreach (var currentGo in currentGameObjects)
        {
            return FetchDestroyablesFromGameObject(currentGo.currentGameObject);
        }
        return null;
    }

    private IDestroyable FetchDestroyablesFromGameObject(GameObject vars)
    {
        List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();
        vars.GetComponents(monoBehaviours);
        
        var idescroyables = monoBehaviours.Where(ell => ell is IDestroyable);
        foreach (var gameObjThatHasIdestroyable in idescroyables)
        {
            if (gameObjThatHasIdestroyable is IDestroyable destroyable)
            {
                return destroyable;
            }
        }
        return null;
    }

    private void FindDestroyablesAndUnsubscribeSingle(GameObject obj)
    {
        var elm = currentGameObjects.FirstOrDefault(el => el.currentGameObject.gameObject == obj);
        if (elm.currentGameObject == null) return;

        var destroyable = FetchDestroyablesFromGameObject(elm.currentGameObject);
        destroyable.DestructionEvent -= InterfOnDestructionEvent;   
        
    }
    
    private void OnDestroy()
    {
        var destroyable = FetchDestroyablesFromCurrentObjects();
        destroyable.DestructionEvent -= InterfOnDestructionEvent;   
    }
    
    private void InterfOnDestructionEvent(object sender, EventArgs e)
    {
        GameObject gameObj = (currentGameObjects.FirstOrDefault(el => el.currentGameObject == (GameObject)sender)).currentGameObject;
        var tmp = Spawnables.FirstOrDefault(el => gameObj);
        
        SpawnOnRandomPoint(gameObj, tmp);
        
        FindDestroyablesAndUnsubscribeSingle(gameObj);
    }

    public void SpawnAllEntries()
    {
        foreach (var item in Spawnables)
        {
            var objType = item.type;
            var amount = item.count;

            for (int i = 0; i < amount; i++)
            {
                SpawnOnRandomPoint(objType, item);
            }
        }
    }

    public int GetValidKnotIndex(int index) => index % Path.Count;
    
    
    public void SpawnOnRandomPoint(GameObject typeOfGoToSpawn, Spawnable spawnable)
    {
        var nextKnotRng = new System.Random();
        var nextKnot = nextKnotRng.Next();
        var knotIndex = GetValidKnotIndex(nextKnot);
        var knot = Path.Next(knotIndex);
        
       GameObject gameobj = Instantiate(typeOfGoToSpawn,CivilianContainer);
       float3 localTransformOffset = new float3(SplineContainer.transform.position.x,SplineContainer.transform.position.y,SplineContainer.transform.position.z);
       gameobj.transform.localPosition= knot.Position + localTransformOffset + spawnable.spawn_offset;
       gameobj.SetActive(true);
       IEnumerator ScaleBypass()
       {
           if (spawnable.scale != Vector3.zero)
           {
               yield return new WaitForEndOfFrame();
               gameobj.transform.localScale = spawnable.scale;
           }
       }
       StartCoroutine(ScaleBypass());
       currentGameObjects.Add(new GoWithIndex(gameobj, knotIndex,spawnable));
       
    }

    private Vector3 ConvertKnotPosToVec3(BezierKnot knot, float3 ObjectSpawnOffset )
    {
        var knotPos = knot.Position;
        float3 localTransformOffset = new float3(SplineContainer.transform.position.x,SplineContainer.transform.position.y,SplineContainer.transform.position.z);
        return new Vector3(knotPos.x+ObjectSpawnOffset.x+localTransformOffset.x,knot.Position.y+ObjectSpawnOffset.y+localTransformOffset.y,knot.Position.z+ObjectSpawnOffset.z+localTransformOffset.z);
    }

    private Vector3 ConvertTransformPosToVec3(Transform knot, float3 ObjectSpawnOffset )
    {
        float3 localTransformOffset = new float3(SplineContainer.transform.position.x,SplineContainer.transform.position.y,SplineContainer.transform.position.z);
        return new Vector3(knot.localPosition.x+ObjectSpawnOffset.x+localTransformOffset.x,knot.localPosition.y+ObjectSpawnOffset.y+localTransformOffset.y,knot.localPosition.z+ObjectSpawnOffset.z+localTransformOffset.z);
    }


    

    public void MoveGameObjectToNextPosition(GoWithIndex go)
    {
        var current = Path.Next(go.knot);
        var next =  Path.Next(Path.NextIndex(go.knot)); 
    
        MoveToKnot(go.currentGameObject, go, current, next);

    }

    private void MoveToKnot(GameObject lgameObject,GoWithIndex go, BezierKnot from, BezierKnot to)
    {
        Vector3 fromPos = ConvertKnotPosToVec3(from, go.typeInfo.spawn_offset);
        Vector3 toPos = ConvertKnotPosToVec3(to, go.typeInfo.spawn_offset);
        
        lgameObject.transform.localPosition = Vector3.Lerp(fromPos, toPos, go.localTimer / go.timeToCompleteTheLoop);
            
        if(go.localTimer / go.timeToCompleteTheLoop > 0.98f)
        {
       
            go.knot = Path.NextIndex(go.knot);
            go.localTimer = 0f;
        }
        go.localTimer += Time.deltaTime;
    }



    // Update is called once per frame
    void Update()
    {
        if (!ShouldMoveAlongPath) return;
        
        foreach (var item in currentGameObjects)
        {
            MoveGameObjectToNextPosition(item);
        }

     

    }
}
