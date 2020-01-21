﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;


public class Experiment {

    public static int levels = 2;
    public static int2 gridSize;
    public static int minuteMod = 60;
    public static int hourMod = 24;

    public static int minute; //0~59
    public static int hour; //0~23
    public static int day; //0~6
    protected static Entity[,,] patches;
    protected static Dictionary<int3,Entity> patchDict = new Dictionary<int3, Entity>();
    public static Entity emptyPatch;
    public static Entity sleepPatch;


    public float speed;
    public static float incrMultiplier = 3;
    public static float turnAngleRadian= 1;
    public static Random random; // This Random will set seeds for all other Randoms used
    
    [NonSerialized]
    public EntityManager em;

    private Entity agent;

    public static Entity GetPatchAt(int x, int y, int simID) {
        
        return patches[x, y, simID];
    }
    
    public static void SetPatchAt(int x, int y, int simID, Entity patch) {
        patches[x, y, simID]=  patch;
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

    /// <summary>
    /// Cluster
    ///     returns a list quantity coordinates centered at center of a radius radius.
    /// coordinates a re between (0,0) and aGridSize
    /// use random generator aRandom
    /// </summary>
    /// <param name="aGridSize"></param>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="quantity"></param>
    /// <param name="aRandom"></param>
    /// <returns></returns>
    public static List<int2> Cluster(int2 aGridSize, float2 center, float radius, int quantity, Random aRandom) {
        var result = new List<int2>();
        var rect = new int4(0, 0, aGridSize.x-1, aGridSize.y-1);
        rect.x = math.max(rect.x, (int)math.floor(center.x - radius));
        rect.y = math.max(rect.y, (int)math.floor(center.y - radius));
        rect.z = math.min(rect.z, (int)math.ceil(center.x + radius));
        rect.w = math.min(rect.w, (int)math.ceil(center.y + radius));
        
        var rSquared = radius * radius;
        var allCoords = new List<int2>();
        for (int i = rect.x; i < rect.z + 1; i++) {
            for (int j = rect.y; j < rect.w + 1; j++) {
                if (math.distancesq(center, new float2(i, j)) < rSquared) {
                    allCoords.Add(new int2(i,j));
                }
            }
        }

        for (int i = 0; i < quantity; i++) {
            if (allCoords.Count == 0) break;
            var index = aRandom.NextInt(0, allCoords.Count);
            result.Add(allCoords[index]);
            allCoords.RemoveAt(index);
        }
        
        return result;
    }

    public void FoodCluster(int simID, int2 aGridSize, float2 center, float radius, int quantity, int minFood, int maxFood, Random aRandom) {
        var coords = Cluster(aGridSize, center, radius, quantity, aRandom);
        foreach (var c in coords) {
            var patch = em.CreateEntity();
            SetPatchAt(c.x, c.y, simID, patch);
            
            em.AddComponentData(patch, new FoodArea(){Value = aRandom.NextInt(minFood,maxFood)}); 
            em.AddComponentData(patch, new SleepArea(){Value = false}); 
            EOSGrid.SetFood(simID,c);
        }
    }
    
    public void SleepCluster(int simID,int2 aGridSize, float2 center, float radius, int quantity, Random aRandom) {
        var coords = Cluster(aGridSize, center, radius, quantity, aRandom);
        var patch = em.CreateEntity();
        em.AddComponentData(patch, new SleepArea(){Value = true});
        em.AddComponentData(patch, new FoodArea(){Value = 0}); 
        foreach (var c in coords) {
            SetPatchAt(c.x, c.y, simID, sleepPatch);
            
            EOSGrid.SetSleep(simID,c);
        }
    }
    
    public void DisplayTest(int2 size) {
        gridSize = size;
        hour = 0;
        turnAngleRadian = math.PI / 12f; //15º
        incrMultiplier = 3;
        em  = World.DefaultGameObjectInjectionWorld.EntityManager;
        SetRandomSeed(1);
        for (int simID = 0; simID < levels; simID++) {
            SetupPatches(levels, gridSize.x, gridSize.y);
            agent = SetupAgent(new float2(1.5f, 1.5f), simID);
            em.SetComponentData(agent, RandomGenome(new Random(random.NextUInt())));
            var foodCenter = ((float2) size) * 0.8f;
            FoodCluster(simID, gridSize, foodCenter, 10, 40, 15, 200, new Random(random.NextUInt()));
            var sleepCenter = ((float2) size) * 0.2f;
            SleepCluster(simID, gridSize, sleepCenter, 10, 40, new Random(random.NextUInt()));
            em.SetComponentData(agent, new Facing() {Value = 0, random = new Random(1)});
            em.SetComponentData(agent, new Action() {Value = Genome.Allele.Eat});
        }
    }

    public Genome RandomGenome(Random aRandom) {
        var result = new Genome();
        for (int i = 0; i < 24; i++) {
            var val = aRandom.NextInt(0, 3);
            result[i] = (Genome.Allele) val;
        }
        return result;
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

    public void SetupPatches(int levels,int x, int y) {
        emptyPatch =   em.CreateEntity();
        em.AddComponentData(emptyPatch, new SleepArea() {Value = false});
        em.AddComponentData(emptyPatch, new FoodArea() {Value = 0});
        sleepPatch = em.CreateEntity();
        em.AddComponentData(sleepPatch, new SleepArea() {Value = true});
        em.AddComponentData(sleepPatch, new FoodArea() {Value = 0});
        
        patches = new Entity[x, y,levels];
        
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                for (int k = 0; k < levels; k++) {
                    SetPatchAt(i, j, k, emptyPatch);
                }
            }
        }
        PosXY.bounds = new float2(x,y);
    }

    public Entity SetupAgent(float2 startXY, int simID) {
        var agent = em.CreateEntity();
        em.AddComponentData(agent, new PosXY(){Value = startXY});
        em.AddComponentData(agent, new SimID(){Value = simID});
        em.AddComponentData(agent, new FoodEnergy() {Value = 0});
        em.AddComponentData(agent, new SleepEnergy() {Value = 0});
        var x = (int) math.floor(startXY.x); 
        var y = (int) math.floor(startXY.y);
        em.AddComponentData(agent, new Patch() {Value = GetPatchAt(x, y,simID)});
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
