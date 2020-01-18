﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EOSGrid : MonoBehaviour {
    
    public Vector2Int size = new Vector2Int(10,10);
    public bool stressTest = false;
    public float worldSize = 10f;
    public Transform holder;
    
    public GameObject prefabAgent;
    public GameObject prefabMesh;
    public Vector2 _offset;
    public Vector2 _scale ;

    public GameObject agent;

    public Material backgroundMaterial;
    public Material foodMaterial;
    public Material sleepMaterial;
    public Material foodEnergy;
    public Material sleepEnergy;
    
    private static MeshRenderer[,] _meshRenderers;
    private static EOSGrid inst;

    public void Start() {
        inst = this;
        InitDisplay();
    }
    
    public void InitDisplay() {
        _scale = ( Vector2.one / size);
        _offset = ((-1 * Vector2.one) + _scale)/2;
        _meshRenderers = new MeshRenderer[size.x,size.y];
        var cellLocalScale  = new Vector3(_scale.x,_scale.y,_scale.x);
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                var c = Instantiate(prefabMesh, holder);
                var pos = new Vector3((i) * _scale.x + _offset.x, (j) * _scale.y + _offset.y, 0);
                c.transform.localScale = cellLocalScale; 
                c.transform.localPosition = pos;
                c.name += new Vector2Int(i, j);
                _meshRenderers[i,j] = c.GetComponent<MeshRenderer>();
            }
        }
    }

    public static void SetFood(int2 loc) {
        _meshRenderers[loc.x, loc.y].material = inst.foodMaterial;
    }
    
    public static void SetSleep(int2 loc) {
        _meshRenderers[loc.x, loc.y].material = inst.sleepMaterial;
    }

    public static void SetAgent(float2 loc, float foodFitness, float sleepFitness) {
        inst.SetAgentInstance(loc, foodFitness, sleepFitness);
    }
    
    public void SetAgentInstance(float2 loc, float foodFitness, float sleepFitness) {
        var pos = new Vector3(loc.x * _scale.x + _offset.x, loc.y * _scale.y + _offset.y, -1);
        agent.transform.localPosition = pos;
    }
}
