using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EOSGrid : MonoBehaviour {

    public Experiment experiment;
    
    public Vector2Int size = new Vector2Int(10,10);
    public int levels = 2;
    public Transform holder;
    
    public GameObject prefabAgent;
    public GameObject prefabMesh;
    public Vector2 _offset;
    public Vector2 _scale ;

    public float2 agentLoc;
    public float agentFood;
    public float agentSleep;
    public Genome agentGenome;
    public AgentData[] agentDatas;

    public GameObject agent;

    public Material backgroundMaterial;
    public Material foodMaterial;
    public Material sleepMaterial;
    public Material foodEnergy;
    public Material sleepEnergy;
    
    public static int displaySimID = 0; // static so SetFood & SetSleep can be called in tests
    
    private static MeshRenderer[,] _meshRenderers;
    private static EOSGrid inst;

    public void Start() {
        experiment = new Experiment();
        Experiment.levels = levels;
        agentDatas = new AgentData[Experiment.levels];
        inst = this;
        InitDisplay();
        agent = prefabAgent;
        experiment.DisplayTest(new int2(size.x,size.y));
        
        
    }

    public void Update() {
        Experiment.NextTick();
        if (Experiment.minute == 0 && Experiment.hour == 0 && Experiment.day == 7) {
            Debug.Log("Generation " + experiment.generationNumber + " Tick " + Experiment.day + " : " + Experiment.hour + " : " + Experiment.minute );
            var ag = agentDatas[0];
            Debug.Log("Loc " + ag.loc + " : " + ag.foodFitness + " : " + ag.sleepFitness + " : " + ag.genome);
            ClearDisplay();
            experiment.NextGeneration();
            Experiment.minute = 0;
            Experiment.hour = 0;
            Experiment.day = 0;
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

    public static void SetFood(int simdID, int2 loc) {
        if (simdID == displaySimID) {
            _meshRenderers[loc.x, loc.y].enabled = true;
            _meshRenderers[loc.x, loc.y].material = inst.foodMaterial;
        }
    }
    
    public static void SetSleep(int simID,int2 loc) {
        if (simID == displaySimID) {
            _meshRenderers[loc.x, loc.y].enabled = true;
            _meshRenderers[loc.x, loc.y].material = inst.sleepMaterial;
        }
        
    }


    public  void  ClearDisplay() {
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                _meshRenderers[i,j].enabled = false;
            }
        }
    }
    public static void SetClear(int2 loc) { 
        _meshRenderers[loc.x, loc.y].enabled = false;
    }
    public static void SetAgent(int simID, float2 loc, float foodFitness, float sleepFitness, Genome.Allele action , Genome genome) {
        inst.SetAgentInstance(simID, loc, foodFitness, sleepFitness, action, genome );
    }

    public void SetAgentInstance(int simID, float2 loc, float foodFitness, float sleepFitness, Genome.Allele action,
        Genome genome) {
        var pos = new Vector3((loc.x - 0.5f) * _scale.x + _offset.x, (loc.y - 0.5f) * _scale.y + _offset.y, -1);
        if (simID == displaySimID) agent.transform.localPosition = pos;
            agentDatas[simID] = new AgentData() { 
            loc = loc,
            foodFitness = foodFitness,
            sleepFitness = sleepFitness,
            genome = genome
        };

}
    
    public static void SetPatch(int simID, float2 loc, float foodArea, bool sleepArea ) {
        inst.SetPatchInstance(simID, loc, foodArea, sleepArea );
    }

    public void SetPatchInstance(int simID, float2 loc, float foodArea, bool sleepArea) {
        if (sleepArea) {
            SetSleep(simID,(int2) loc);
        } else if(foodArea <= 0) {
            SetFood(simID,(int2)loc);
        }
        else {
            SetClear((int2)loc);
        }
        
    }

    [Serializable]
    public struct AgentData {
        public int simID;
        public float2 loc;
        public float foodFitness;
        public float sleepFitness;
        public Genome genome;
        
    }
}
