using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class ExperimentSetting : MonoBehaviour {


    public static int minuteMod = 60;
    public static int hourMod = 24;

    public static int minute; //0~59
    public static int hour; //0~23
    public static int day; //0~6
    public static Entity[,] patches;

    public float speed;
    public static float incrMultiplier = 3;
    public static float turnAngleRadian= 1;
    public static Random random; // This Random will set seeds for all other Randoms used
    
    [NonSerialized]
    public EntityManager em;

    private Entity agent;
    

    public void Update() {
        NextTick();
    }
    
    public static void NextTick() {
        minute++;
        minute %= minuteMod;
        if (minute == 0) {
            hour++;
            hour %= hourMod;
            if (hour == 0) {
                day++;
            }
        }
    }


    public void DisplayTest(int2 size) {
        hour = 0;
        turnAngleRadian = math.PI / 2f; //90º
        incrMultiplier = 3;
        var go = new GameObject("ExperimentSetting");
        em  = World.DefaultGameObjectInjectionWorld.EntityManager;
        SetRandomSeed(1);
        SetupPatches(size.x, size.y);
        agent = SetupAgent(new float2(1.5f, 1.5f));
        var centerPatch =patches[1, 1];
        em.SetComponentData(centerPatch, new FoodArea(){Value = 2});
        var sleepPatch =patches[2, 2];
        em.SetComponentData(sleepPatch, new SleepArea(){Value =true});
        em.SetComponentData(agent, new Facing(){Value = 0, random = new Random(1)});
        em.SetComponentData(agent, new Action(){Value = Genome.Allele.Eat});
        
        var agentPatch = em.GetComponentData<Patch>(agent).Value;
        EOSGrid.SetFood(new int2(1,1));
        EOSGrid.SetSleep(new int2(2,2));
        Debug.Log(em.HasComponent<AdjustFoodArea>(centerPatch));
    }
    
    public void Test() {
        
        hour = 0;
        turnAngleRadian = math.PI / 2f; //90º
        incrMultiplier = 3;
        var go = new GameObject("ExperimentSetting");
        em  = World.DefaultGameObjectInjectionWorld.EntityManager;
        SetRandomSeed(1);
        SetupPatches(3, 3);
        agent = SetupAgent(new float2(1.5f, 1.5f));
        var centerPatch =patches[1, 1];
        em.SetComponentData(centerPatch, new FoodArea(){Value = 2});
        em.SetComponentData(agent, new Facing(){Value = 0, random = new Random(1)});
        em.SetComponentData(agent, new Action(){Value = Genome.Allele.Eat});
        
        var agentPatch = em.GetComponentData<Patch>(agent).Value;
       
        Debug.Log(em.HasComponent<AdjustFoodArea>(centerPatch));
        
      
        
    }
    
    /// <summary>
    /// Set master Random seed
    /// 0 is changed to 1
    /// </summary>
    /// <param name="seed"></param>
    public void SetRandomSeed(uint seed) {
        seed = seed == 0 ? 1: seed ;
        random = new  Random(seed);
    }

    public void SetupPatches(int x, int y ) {
        patches = new Entity[x, y];
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                var patch = em.CreateEntity();
                em.AddComponentData(patch, new PosXY() {Value = new float2(x, y)});
                em.AddComponentData(patch, new SleepArea() {Value = false});
                em.AddComponentData(patch, new FoodArea() {Value = 0});
                patches[i, j] = patch;
            }
        }
        PosXY.bounds = new float2(x,y);
    }

    public Entity SetupAgent(float2 startXY) {
        var agent = em.CreateEntity();
        em.AddComponentData(agent, new PosXY(){Value = startXY});
        em.AddComponentData(agent, new FoodEnergy() {Value = 0});
        em.AddComponentData(agent, new SleepEnergy() {Value = 0});
        var x = (int) math.floor(startXY.x); 
        var y = (int) math.floor(startXY.y);
        em.AddComponentData(agent, new Patch() {Value = patches[x, y]});
        em.AddComponentData(agent, new Speed() {Value = speed});
        uint seed = random.NextUInt();
        seed = seed == 0 ? 1: seed ;
        em.AddComponentData(agent, new Facing() {Value = 0, random = new Random(1)});
        em.AddComponentData(agent, new Action());
        var chooseGenome = new Genome();
        for(int i= 0; i<24;i++) {
            chooseGenome[i] = Genome.Allele.Choose;
        };
        em.AddComponentData(agent, chooseGenome); 
        return agent;
    }
    
    
}
