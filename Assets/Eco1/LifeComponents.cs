﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;



public struct Cell : ISharedComponentData {
    public float4 rect; //xMin, yMin, xMax, yMax
}

public struct CellEnergyChunk : IComponentData {
    public float Value; // totalEnergy in  a cell 
}

public struct PosXY : IComponentData {
    public float2 Value;
}

public struct Energy : IComponentData {
    public float Value;
}

public struct GrowSpeed : IComponentData {
    public float Value;
}

