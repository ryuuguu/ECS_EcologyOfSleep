using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/// <summary>
/// decide attempt sleep or eat (before(Can't eat, can't sleep))
///check genome
///  if eat or sleep use them
///  if choose compare energies 
/// 
/// </summary>
[AlwaysSynchronizeSystem]
[BurstCompile]
public class AdjustFoodAreaSystem : JobComponentSystem {
    EntityQuery m_Group;
    protected EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate() {
        base.OnCreate();
        m_Group = GetEntityQuery(
            ComponentType.ReadWrite<FoodArea>(),
            ComponentType.ReadWrite<AdjustFoodArea>()
        );
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    struct AdjustFoodAreaJob : IJobForEachWithEntity< FoodArea,AdjustFoodArea> {
        
        public EntityCommandBuffer.Concurrent ecb;
        
        public void Execute(Entity entity, int entityInQueryIndex, ref FoodArea foodArea,
            [ReadOnly] ref AdjustFoodArea adjustFoodArea) {

            foodArea.Value += adjustFoodArea.Value;
            ecb.RemoveComponent<AdjustFoodArea>(entityInQueryIndex,entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        
        
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        var job = new AdjustFoodAreaJob() {
            ecb = ecb
        };
        var jobHandle = job.Schedule(m_Group, inputDependencies);
        m_EndSimulationEcbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}





/// <summary>
/// decide attempt sleep or eat (before(Can't eat, can't sleep))
///check genome
///  if eat or sleep use them
///  if choose compare energies 
/// 
/// </summary>
[AlwaysSynchronizeSystem]
[BurstCompile]
[UpdateBefore(typeof(ExecuteActionSystem))]
public class SetActionSystem : JobComponentSystem {
    EntityQuery m_Group;
    protected override void OnCreate() {
        // Cached access to a set of ComponentData based on a specific query
        m_Group = GetEntityQuery(ComponentType.ReadWrite<Genome>(),
            ComponentType.ReadWrite<Action>(),
            ComponentType.ReadOnly<FoodEnergy>(),
            ComponentType.ReadOnly<SleepEnergy>()
        );
    }
    
    struct SetActionJob : IJobForEach<Action,Genome,FoodEnergy,SleepEnergy> {
        public int hour;
        
        public void Execute(ref Action action, [ReadOnly] ref Genome genome, [ReadOnly] ref FoodEnergy foodEnergy,
            [ReadOnly] ref SleepEnergy sleepEnergy) {

            var foodFitness = foodEnergy.Fitness();
            var sleepFitness = sleepEnergy.Fitness();
            var prevChoice = math.select((int)action.Value, (int)Genome.Allele.Eat, foodFitness + 0.1f < sleepFitness);
            prevChoice = math.select(prevChoice, (int)Genome.Allele.Sleep, sleepFitness + 0.1f < foodFitness);
            prevChoice = math.select(prevChoice, (int)Genome.Allele.Eat, genome[hour] == Genome.Allele.Eat);
            action = new Action(){Value =  (Genome.Allele) math.select(prevChoice, (int)Genome.Allele.Sleep,
                    genome[hour] == Genome.Allele.Sleep)};
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        
        var job = new SetActionJob() {
            hour = Experiment1.hour
        };
        return job.Schedule(m_Group, inputDependencies);
    }
    
   
}


/// <summary>
/// decide attempt sleep or eat (before(Can't eat, can't sleep))
///check genome
///  if eat or sleep use them
///  if choose compare energies 
/// 
/// </summary>
[AlwaysSynchronizeSystem]
[BurstCompile]
public class ExecuteActionSystem : JobComponentSystem {
    EntityQuery m_Group;
    protected EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate() {
        base.OnCreate();
        m_Group = GetEntityQuery(
            ComponentType.ReadWrite<FoodEnergy>(),
            ComponentType.ReadWrite<SleepEnergy>(),
            ComponentType.ReadWrite<PosXY>(),
            ComponentType.ReadWrite<Facing>(),
            ComponentType.ReadOnly<Action>(),
            ComponentType.ReadOnly<Patch>()
        );
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    
    struct ExecuteActionJob : IJobForEachWithEntity< FoodEnergy,SleepEnergy,PosXY,Facing,Action,Patch> {

        [ReadOnly]public ComponentDataFromEntity<SleepArea> sleepAreaLookup;
        [ReadOnly]public ComponentDataFromEntity<FoodArea> foodAreaLookup;
        
        public EntityCommandBuffer.Concurrent ecb;

        public float incrMultiplier;
        public float turnAngle;
        public void Execute(Entity entity, int entityInQueryIndex, ref FoodEnergy foodEnergy,
            ref SleepEnergy sleepEnergy,ref PosXY posXY, ref Facing facing, [ReadOnly] ref Action action, 
             [ReadOnly] ref Patch patch) {

            float eatAmount = math.select(-1f, 1f* incrMultiplier,
                (action.Value == Genome.Allele.Eat) && (foodAreaLookup[patch.Value].Value > 0));
            foodEnergy.Value += eatAmount ;
            
            float sleepAmount = math.select(-1f, 1f*incrMultiplier,
                (action.Value == Genome.Allele.Sleep) && sleepAreaLookup[patch.Value].Value);
            sleepEnergy.Value = sleepAmount; // if not sleeping -1 sleep
            
            var move = ((sleepAmount <= 0) &&   (eatAmount <= 0)) ;

            if (move) {
               var flipTurn = facing.random.NextUInt(0, 2) == 0 ? -1f : 1f;
               
                facing.Value += flipTurn * turnAngle;
                var delta = new float2(math.cos(facing.Value), math.sin(facing.Value));
                posXY.Value += delta;
                bool flipAngle = false;
                flipAngle = posXY.Value.x < 0;
                posXY.Value.x = math.select(posXY.Value.x, posXY.Value.x * -1, posXY.Value.x < 0);
                
                flipAngle = flipAngle || posXY.Value.y < 0;
                posXY.Value.y = math.select(posXY.Value.y, posXY.Value.y * -1, posXY.Value.y < 0);
                
                flipAngle = flipAngle || posXY.Value.x > PosXY.bounds.x;
                posXY.Value.x = math.select(posXY.Value.x, 2 * PosXY.bounds.x - posXY.Value.x,
                    posXY.Value.x > PosXY.bounds.x);
                
                flipAngle = flipAngle || posXY.Value.y > PosXY.bounds.y;
                posXY.Value.y = math.select(posXY.Value.y, 2 * PosXY.bounds.y - posXY.Value.y,
                    posXY.Value.y > PosXY.bounds.y);

                facing.Value = math.select(facing.Value, facing.Value += math.PI, flipAngle);

                facing.Value %= 2f * math.PI;
            }
            
            if (eatAmount >0) {
                ecb.AddComponent(entityInQueryIndex,patch.Value,new AdjustFoodArea(){Value = -1});
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        ComponentDataFromEntity<SleepArea> sleepAreas = GetComponentDataFromEntity<SleepArea>(); 
        ComponentDataFromEntity<FoodArea> foodAreas = GetComponentDataFromEntity<FoodArea>();
        
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        var job = new ExecuteActionJob() {
           sleepAreaLookup = sleepAreas,
           foodAreaLookup = foodAreas,
           ecb = ecb,
           incrMultiplier = Experiment1.incrMultiplier,
           turnAngle = Experiment1.turnAngleRadian
        }; 
        var jobHandle = job.Schedule(m_Group, inputDependencies);
        m_EndSimulationEcbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;


    }
    
}

/// <summary>
/// decide attempt sleep or eat (before(Can't eat, can't sleep))
///check genome
///  if eat or sleep use them
///  if choose compare energies 
/// 
/// </summary>
[UpdateInGroup(typeof(PresentationSystemGroup))]
[AlwaysSynchronizeSystem]
public class DisplayAgentSystem : JobComponentSystem {
    EntityQuery m_Group;

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        
         Entities
             .WithoutBurst()
            .ForEach((in Action action, in  FoodEnergy foodEnergy,
             in  SleepEnergy sleepEnergy, in PosXY posXY)=> {
                EOSGrid.SetAgent(posXY.Value,foodEnergy.Fitness(),sleepEnergy.Fitness());   
            }).Run();
       
        return default;
    }
    
   
}


