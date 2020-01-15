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

public class ECSGrid : MonoBehaviour {
    public Vector2Int size = new Vector2Int(10,10);
    public float worldSize = 10f;
    public Transform holder;
    public GameObject prefabCell;
    public GameObject prefabRenderMesh;
    public Vector2 _offset;
    public Vector2 _scale ;
    Entity[,] _cells;
    public static  int[] stay = new int[9];
    public static int[] born = new int[9];
    public static List<RenderMesh> renderMeshs = new List<RenderMesh>();
    public static NativeArray<Color32> nativeImage;
    public static int sizeX;
    public static int sizeY;
    

    public float zDeadSetter;
    //used to move an entity behind the holder image when it is not live
    // changing its color to black would nicer but that is only available in HDRP so far
    public static float zLive = -1;
    public static float zDead = 1;
    
    void Start() {
        sizeX = size.x;
        sizeY = size.y;
        
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
        RPentonomio((size+2*Vector2Int.one)/2, entityManager);
        stay[2] = stay[3] = 1; // does NOT include self in count
        born[3] = 1;
    }
    
    private void SetLive(int i, int j, EntityManager entityManager) {
        var instance = _cells[i, j];
            var position = new float3((i - 1) * _scale.x + _offset.x, (j - 1) * _scale.y + _offset.y, zLive) * worldSize;
            entityManager.SetComponentData(instance, new Translation {Value = position});
            entityManager.SetComponentData(instance, new Live {value = 1});
            entityManager.SetComponentData(instance, new NextState() {value = 1});
    }

    void RPentonomio(Vector2Int center, EntityManager entityManager) {
        SetLive(center.x, center.y, entityManager);
        SetLive(center.x, center.y+1, entityManager);
        SetLive(center.x+1, center.y+1, entityManager);
        SetLive(center.x, center.y-1, entityManager);
        SetLive(center.x-1, center.y, entityManager);
    }
}
