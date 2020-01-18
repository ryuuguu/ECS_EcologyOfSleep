using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EOSGrid : MonoBehaviour {
    
    public Vector2Int size = new Vector2Int(10,10);
    public bool stressTest = false;
    public float worldSize = 10f;
    public Transform holder;
    public Transform holderSC;
    public GameObject prefabCell;
    public GameObject prefabMesh;
    public Vector2 _offset;
    public Vector2 _scale ;
    
    // for next tutorial
    public Material[] materials;
    public static Material[] materialsStatic;
    public int superCellScale = 2;
    
    //Entity[,] _cells;
    private static MeshRenderer[,] _meshRenderersSC;
    private static MeshRenderer[,] _meshRenderers;

    public void Start() {
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
    
}
