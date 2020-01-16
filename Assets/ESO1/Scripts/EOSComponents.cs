using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct FoodEnergy : IComponentData {
    public float Value;
}

public struct SleepEnergy : IComponentData {
    public float Value;
}


public struct PosXY : IComponentData {
    public float2 Value;
}

public struct SleepArea : IComponentData {
}

public struct FoodArea : IComponentData {
    public float Value;
}

/// <summary>
/// 2D Facing in radians
/// </summary>
public struct Facing : IComponentData {
    public float Value;
}

public struct Speed : IComponentData {
    public float Value;
}

[System.Serializable]
public struct Genome  : IComponentData {
    public float h0;
    public float h1;
    public float h2;
    public float h3;
    public float h4;
    public float h5;
    public float h6;
    public float h7;
    public float h8;
    public float h9;
    public float h10;
    public float h11;
    public float h12;
    public float h13;
    public float h14;
    public float h15;
    public float h16;
    public float h17;
    public float h18;
    public float h19;
    public float h20;
    public float h21;
    public float h22;
    public float h23;
    
    /// <summary>Returns the float element at a specified index.</summary>
    unsafe public float this[int index]
    {
        get
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((uint)index >= 24)
                throw new System.ArgumentException("get index must be between[0...23] was "+ index );
#endif
            fixed (Genome* array = &this) { return ((float*)array)[index]; }
        }
        set
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((uint)index >= 24)
                throw new System.ArgumentException("set index must be between[0...23] was " + index);
#endif
            fixed (float* array = &h0) { array[index] = value; }
        }
    }

}

