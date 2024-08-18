using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Interfaces;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public struct Spawnable {
    public GameObject type;
    public int count;
    public float3 spawn_offset;
    public Vector3 scale;
    
}


public class CilvianSpawner : MonoBehaviour
{
    private Transform CivilianContainer;
    [Header("Create a spline as a child and drag it in here!")]
    public SplineContainer SplineContainer;
    private Spline Path;

    public List<Spawnable> Spawnables; //type,count
    public bool ShouldMoveAlongPath = true;
    private List<GameObject> currentGameObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        // can be set freely if you want to
        CivilianContainer = this.transform;
        
        // only use first spline for now
        Path = SplineContainer[0];
        
        // Holds all current objs on the track
        currentGameObjects = new List<GameObject>();

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
        foreach (var vars in currentGameObjects)
        {
            return FetchDestroyablesFromGameObject(vars);
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
        var elm = currentGameObjects.FirstOrDefault(el => el.gameObject == obj);
        if (elm == null) return;

        var destroyable = FetchDestroyablesFromGameObject(elm);
        destroyable.DestructionEvent -= InterfOnDestructionEvent;   
        
    }
    
    private void OnDestroy()
    {
        var destroyable = FetchDestroyablesFromCurrentObjects();
        destroyable.DestructionEvent -= InterfOnDestructionEvent;   
    }
    
    private void InterfOnDestructionEvent(object sender, EventArgs e)
    {
        GameObject gameObj = currentGameObjects.FirstOrDefault(el => el == (GameObject)sender);
        var tmp = Spawnables.FirstOrDefault(el => gameObj);
        
        SpawnOnRandomPoint(gameObj, tmp.spawn_offset, tmp.scale);
        
        FindDestroyablesAndUnsubscribeSingle(gameObj);
    }

    public void SpawnAllEntries()
    {
        foreach (var item in Spawnables)
        {
            var objType = item.type;
            var amount = item.count;
            var offset = item.spawn_offset;
            var scale = item.scale;
            for (int i = 0; i < amount; i++)
            {
                SpawnOnRandomPoint(objType,offset,scale);
            }
        }
    }

    public void SpawnOnRandomPoint(GameObject typeOfGoToSpawn, float3 ObjectSpawnOffset, Vector3 scale)
    {
        var nextKnotRng = new System.Random();
        var nextKnot = nextKnotRng.Next();
        var maxKnots = Path.Count;
        
        var knot = Path.Next(nextKnot%maxKnots);
        
        //next pos;
       // var nextPos= knot.Position;
       
       GameObject gameobj = Instantiate(typeOfGoToSpawn,CivilianContainer);
       float3 localTransformOffset = new float3(SplineContainer.transform.position.x,SplineContainer.transform.position.y,SplineContainer.transform.position.z);
       gameobj.transform.localPosition= knot.Position + localTransformOffset + ObjectSpawnOffset;
       // gameobj.transform.position.Scale(scale);

       gameobj.SetActive(true);
       
       IEnumerator tmp()
       {
           if (scale != Vector3.zero)
           {
               yield return new WaitForEndOfFrame();
               gameobj.transform.localScale = scale;
           }
       }

       StartCoroutine(tmp());
      
       

       currentGameObjects.Add(gameobj);
       
    }
    
    

    // Update is called once per frame
    void Update()
    {
        //TODO: Obj pooler / respawner

        // Detect if obj got destroyed. First filters 
        

    }
}
