using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;

/*
[AlwaysSynchronizeSystem]
[BurstCompile]
public class UpdateGrowAutotrophSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = 
            Entities
                .ForEach((ref Energy energy, in GrowSpeed growSpeed) => {
                    energy.Value += growSpeed.Value;
                }).Schedule(inputDeps);
        return job;
    }
    
}

*/
//cell energy needs all components in same cell but only in same cell 
// so if needs one with same shared component 
// this may not be the same chunk 
// so how to find entities with same shared component to read from 
[AlwaysSynchronizeSystem]
[BurstCompile]
public class UpdateCellEnergyAutotrophSystem : JobComponentSystem {
    EntityQuery m_Group;

    protected override void OnCreate() {
        // Cached access to a set of ComponentData based on a specific query
        m_Group = GetEntityQuery(ComponentType.ReadOnly<GrowSpeed>(),
            ComponentType.ReadWrite<Energy>(),
            ComponentType.ChunkComponent<CellEnergyChunk>()
        );
    }
    
    struct CellEnergyJob : IJobChunk {
        
        [ReadOnly]public ArchetypeChunkComponentType<GrowSpeed> GrowSpeedType;
        public ArchetypeChunkComponentType<CellEnergyChunk> CellEnergyChunkType;
        public ArchetypeChunkComponentType<Energy>EnergyType;
        
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
            
            var chunkGrowSpeeds = chunk.GetNativeArray(GrowSpeedType);
            var chunkEnergys = chunk.GetNativeArray(EnergyType);
            
            for (var i = 0; i < chunk.Count; i++) {
               
                var current = chunk.GetChunkComponentData(CellEnergyChunkType).Value;
                
                chunkEnergys[i] = new Energy() {Value = chunkEnergys[i].Value + chunkGrowSpeeds[i].Value};
                
                chunk.SetChunkComponentData(CellEnergyChunkType,
                  new CellEnergyChunk(){Value = current + chunkGrowSpeeds[i].Value 
                  });

            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        
        var growSpeedType = GetArchetypeChunkComponentType<GrowSpeed>(true);
        var energyType = GetArchetypeChunkComponentType<Energy>(false);
        var cellEnergyChunkType = GetArchetypeChunkComponentType<CellEnergyChunk>();

        var job = new CellEnergyJob() {
            EnergyType = energyType,
            GrowSpeedType = growSpeedType,
            CellEnergyChunkType = cellEnergyChunkType
        };
        return job.Schedule(m_Group, inputDependencies);
    }
    
    
}
