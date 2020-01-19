using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EOSGrid : MonoBehaviour {

    public Experiment1 experiment;
    
    public Vector2Int size = new Vector2Int(10,10);
    public Transform holder;
    
    public GameObject prefabAgent;
    public GameObject prefabMesh;
    public Vector2 _offset;
    public Vector2 _scale ;

    public float2 agentLoc;
    public float agentFood;
    public float agentSleep;
    public Genome agentGenome;

    public GameObject agent;

    public Material backgroundMaterial;
    public Material foodMaterial;
    public Material sleepMaterial;
    public Material foodEnergy;
    public Material sleepEnergy;
    
    private static MeshRenderer[,] _meshRenderers;
    private static EOSGrid inst;

    public void Start() {
        experiment = new Experiment1();
        inst = this;
        InitDisplay();
        agent = prefabAgent;
        experiment.DisplayTest(new int2(size.x,size.y));
        
    }

    public void Update() {
        Experiment1.NextTick();
        if (Experiment1.minute == 0 && Experiment1.hour == 0 && Experiment1.day == 7) {
            Debug.Log("Loc "+ agentLoc + " : " + agentFood + " : " + agentSleep + " : "+ agentGenome);
        }
    }
    
    public void InitDisplay() {
        _scale = ( Vector2.one / size);
        _offset = ((-1 * Vector2.one) + _scale)/2;
        _meshRenderers = new MeshRenderer[size.x,size.y];
        var cellLocalScale  = new Vector3(_scale.x,_scale.y,_scale.x);
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                var c = Instantiate(prefabMesh, holder);
                var pos = new Vector3((i) * _scale.x + _offset.x, (j) * _scale.y + _offset.y, -0.1f);
                c.transform.localScale = cellLocalScale; 
                c.transform.localPosition = pos;
                c.name += new Vector2Int(i, j);
                _meshRenderers[i,j] = c.GetComponent<MeshRenderer>();
            }
        }
    }

    public static void SetFood(int2 loc) {
        _meshRenderers[loc.x, loc.y].enabled = true;
        _meshRenderers[loc.x, loc.y].material = inst.foodMaterial;
    }
    
    public static void SetSleep(int2 loc) {
        _meshRenderers[loc.x, loc.y].enabled = true;
        _meshRenderers[loc.x, loc.y].material = inst.sleepMaterial;
    }

    public static void SetClear(int2 loc) {
        _meshRenderers[loc.x, loc.y].enabled = false;
    }
    public static void SetAgent(float2 loc, float foodFitness, float sleepFitness, Genome.Allele action , Genome genome) {
        inst.SetAgentInstance(loc, foodFitness, sleepFitness, action, genome );
    }
    
    public void SetAgentInstance(float2 loc, float foodFitness, float sleepFitness, Genome.Allele action, Genome genome ) {
        var pos = new Vector3((loc.x -0.5f)* _scale.x + _offset.x, (loc.y-0.5f) * _scale.y + _offset.y, -1);
        agent.transform.localPosition = pos;
        agentLoc = loc;
        agentFood = foodFitness;
        agentSleep = sleepFitness;
        agentGenome = genome;

    }
    
    public static void SetPatch(float2 loc, float foodArea, bool sleepArea ) {
        inst.SetPatchInstance(loc, foodArea, sleepArea );
    }

    public void SetPatchInstance(float2 loc, float foodArea, bool sleepArea) {
        if (sleepArea) {
            SetSleep((int2) loc);
        } else if(foodArea <= 0) {
            SetFood((int2)loc);
        }
        else {
            SetClear((int2)loc);
        }
        
    }
}
