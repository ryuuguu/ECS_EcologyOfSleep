using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct FoodEnergy : IComponentData {
    public float Value;
}

public struct SleepEnergy : IComponentData {
    public float Value;
}


public struct PosXY : IComponentData {
    public float2 Value;
    
    public static float2 bounds;
    
}

public struct SleepArea : IComponentData {
    public bool Value;
}

public struct FoodArea : IComponentData {
    public float Value;
}

public struct Patch : IComponentData {
    public Entity Value;
}

public struct AdjustFoodArea : IComponentData {
    public float Value;
}

/// <summary>
/// 2D Facing in radians
/// </summary>
public struct Facing : IComponentData {
    public float Value;
    //public Random random;
}

public struct Speed : IComponentData {
    public float Value;
}

[System.Serializable]
public struct Genome  : IComponentData {
    public Allele h0;
    public Allele h1;
    public Allele h2;
    public Allele h3;
    public Allele h4;
    public Allele h5;
    public Allele h6;
    public Allele h7;
    public Allele h8;
    public Allele h9;
    public Allele h10;
    public Allele h11;
    public Allele h12;
    public Allele h13;
    public Allele h14;
    public Allele h15;
    public Allele h16;
    public Allele h17;
    public Allele h18;
    public Allele h19;
    public Allele h20;
    public Allele h21;
    public Allele h22;
    public Allele h23;
    
    /// <summary>Returns the float element at a specified index.</summary>
    unsafe public Allele this[int index]
    {
        get
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((uint)index >= 24)
                throw new System.ArgumentException("get index must be between[0...23] was "+ index );
#endif
            fixed (Genome* array = &this) { return ((Allele*)array)[index]; }
        }
        set
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((uint)index >= 24)
                throw new System.ArgumentException("set index must be between[0...23] was " + index);
#endif
            fixed (Allele* array = &h0) { array[index] = value; }
        }
    }

    public enum Allele {
        Sleep,
        Eat,
        Choose
    }
    
}


public struct Action : IComponentData {
    public Genome.Allele Value;
}
