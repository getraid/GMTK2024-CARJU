using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class CilvianSpawner : MonoBehaviour
{
    public Transform CivilianContainer;
    public float3 ObjectSpawnOffset;
    public SplineContainer SplineContainer;
    private Spline Path;
    public List<GameObject> Spawnables;
    public int AmountToSpawn = 5;
    public bool ShouldMoveAlongPath = true;

    private List<GameObject> currentGameObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        // only use first spline for now
        Path = SplineContainer[0];
        
   

        SpawnOnRandomPoint(0);
        SpawnOnRandomPoint(1);
        SpawnOnRandomPoint(0);
    }

    public void SpawnOnRandomPoint(int indexOfItemToSpawn)
    {
        var nextKnotRng = new System.Random();
        var nextKnot = nextKnotRng.Next();
        var maxKnots = Path.Count;
        
        var knot = Path.Next(nextKnot%maxKnots);
        
        //next pos;
       var nextPos= knot.Position;
       
       GameObject gameobj = Instantiate(Spawnables[indexOfItemToSpawn],CivilianContainer);
       float3 localTransformOffset = new float3(transform.position.x,transform.position.y,transform.position.z);
       gameobj.transform.localPosition= knot.Position + localTransformOffset +  ObjectSpawnOffset;
       gameobj.SetActive(true);
    }
    
    //TODO: Obj pooler / respawner
    

    // Update is called once per frame
    void Update()
    {
        // Detect if obj
        
        
    }
}
