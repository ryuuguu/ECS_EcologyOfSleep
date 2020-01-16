using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class ECSEcoSetup : MonoBehaviour {
    public Vector2Int size = new Vector2Int(10,10);// this is for cells 
    public float worldSize = 10f;
    public Transform holder;
    public GameObject prefabCell;
    public Vector2 _offset;
    public Vector2 _scale ;
    Entity[,] _cells;
    
    public float zDeadSetter;
    //used to move an entity behind the holder image when it is not live
    // changing its color to black would nicer but that is only available in HDRP so far
    public static float zLive = -1;
    public static float zDead = 2000;
    
    protected 


    private void Start() {
        TestAutotrophs();
    }

    void InitCellRanges(Vector2Int size) {
        
    }
    
    //randomly place a couple of autorophs
    void TestAutotrophs() {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        uint seed = 15;
        
        Random random =new  Random(seed);
        for (int i = 0; i < 9; i++) {
            var instance = entityManager.CreateEntity();
            var pos = random.NextFloat2();
            entityManager.AddComponentData(instance, new PosXY() {Value =pos});
            entityManager.AddComponentData(instance, new FoodEnergy {Value =0});
            entityManager.AddComponentData(instance, new GrowSpeed {Value =0.5f });
            
            var rect = new float4();
            rect[0] = (math.floor(pos.x * size.x)) / size.x;
            rect[2] = rect[0] + 1.0f / size.x;
            rect[1] = (math.floor(pos.y * size.y)) / size.y;
            rect[3] = rect[1] + 1.0f / size.y;
            entityManager.AddSharedComponentData(instance, new Cell {rect = rect});
            /*
            entityManager.AddChunkComponentData<CellEnergyChunk>(instance);
            var entityChunk = entityManager.GetChunk(instance);
            entityManager.SetChunkComponentData<CellEnergyChunk>(entityChunk, 
                new CellEnergyChunk(){Value = 0f});
                */
            
            
        }
        EntityQueryDesc ChunksWithoutCellEnergyChunkDesc = new EntityQueryDesc() {
            None = new ComponentType[] {ComponentType.ChunkComponent<CellEnergyChunk>()}
        };
        var ChunksWithoutCellEnergyChunk = entityManager.CreateEntityQuery(ChunksWithoutCellEnergyChunkDesc);
        entityManager.AddChunkComponentData<CellEnergyChunk>(ChunksWithoutCellEnergyChunk,
            new CellEnergyChunk() {Value = 0F});
    }

    void CellInit() {
        zDead = zDeadSetter;
        
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabCell, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        
        _scale = ( Vector2.one / size);
        _offset = ((-1 * Vector2.one) + _scale)/2;
        _cells = new Entity[size.x+2,size.y+2];
        
        for (int i = 0; i < size.x+2; i++) {
            for (int j = 0; j < size.y+2; j++) {
                var instance = entityManager.Instantiate(entity);
                var position = new float3((i-1) * _scale.x + _offset.x, (j-1) * _scale.y + _offset.y, zDead)*worldSize;
                entityManager.SetComponentData(instance, new Translation {Value = position});
                entityManager.AddComponentData(instance, new Scale {Value = _scale.x*worldSize});
                entityManager.AddComponentData(instance, new Live { value = 0});
                entityManager.AddComponentData(instance, new debugFilterCount { Value = 0});
                _cells[i, j] = instance;
            }
        }
        entityManager.DestroyEntity(entity);
        for (int i = 1; i < size.x+1; i++) {
            for (int j = 1; j < size.y+1; j++) {
                var instance = _cells[i, j];
                entityManager.AddComponentData(instance, new NextState() {value = 0});
                entityManager.AddComponentData(instance, new Neighbors() {
                    nw = _cells[i - 1, j - 1], n = _cells[i - 1, j], ne =  _cells[i - 1, j+1],
                    w = _cells[i , j-1], e = _cells[i, j + 1],
                    sw = _cells[i + 1, j - 1], s = _cells[i + 1, j], se =  _cells[i + 1, j + 1]
                });
                
                //another good test pattern for seeing the edges of grid
                /*
                if ((i + j) % 2 == 0) {
                    SetLive(i, j, entityManager);
                }
                */
            }
        }
    }
    
   

}
