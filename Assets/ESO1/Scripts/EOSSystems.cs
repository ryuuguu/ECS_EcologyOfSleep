using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEditor;
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
public class SetActionSystem : JobComponentSystem {
    EntityQuery m_Group;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Genome.Allele SelectAction(Genome.Allele prevAllele, Genome.Allele currAllele, float foodNRG,
        float sleepNRG) {
        var prevChoice = math.select((int)prevAllele, (int)Genome.Allele.Eat, foodNRG + 0.1 < sleepNRG);
        prevChoice = math.select(prevChoice, (int)Genome.Allele.Sleep, sleepNRG + 0.1 < foodNRG);
        prevChoice = math.select(prevChoice, (int)Genome.Allele.Eat, currAllele == Genome.Allele.Eat);
        return (Genome.Allele) math.select(prevChoice, (int)Genome.Allele.Sleep, currAllele == Genome.Allele.Sleep);
    }
    
    protected override void OnCreate() {
        // Cached access to a set of ComponentData based on a specific query
        m_Group = GetEntityQuery(ComponentType.ReadOnly<Genome>(),
            ComponentType.ReadWrite<Action>(),
            ComponentType.ReadOnly<FoodEnergy>(),
            ComponentType.ReadOnly<SleepEnergy>()
        );
    }
    
    struct SetActionJob : IJobForEach<Action,Genome,FoodEnergy,SleepEnergy> {

        public int hour;
        
        public void Execute(ref Action action, [ReadOnly] ref Genome genome, [ReadOnly] ref FoodEnergy foodEnergy,
            [ReadOnly] ref SleepEnergy sleepEnergy) {
            action = new Action(){Value = SelectAction(action.Value,genome[hour],
                foodEnergy.Value, sleepEnergy.Value)};
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        
        var job = new SetActionJob() {
            hour = ExperimentSetting.hour
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

        public float eatMultiplier;
        public float turnAngle;
        public void Execute(Entity entity, int entityInQueryIndex, ref FoodEnergy foodEnergy,
            ref SleepEnergy sleepEnergy,ref PosXY posXY, ref Facing facing, [ReadOnly] ref Action action, 
             [ReadOnly] ref Patch patch) {

            float eatAmount = math.select(0f, 1f,
                (action.Value == Genome.Allele.Eat) && (foodAreaLookup[patch.Value].Value > 0));
            foodEnergy.Value += eatAmount * eatMultiplier;
            foodEnergy.Value -= 1f - eatAmount; // if not eating -1 food
             
            float sleepAmount = math.select(-1f, 1f,
                (action.Value == Genome.Allele.Sleep) && sleepAreaLookup[patch.Value].Value);
            sleepEnergy.Value = sleepAmount; // if not sleeping -1 sleep
            
            var move = ((sleepAmount <= 0) &&   (eatAmount <= 0)) ;

            if (move) {
               var flipTurn = facing.random.NextUInt(0, 2) == 0 ? -1f : 1f;
               
                facing.Value += flipTurn * turnAngle;
                var delta = new float2(math.cos(facing.Value), math.sin(facing.Value));
                posXY.Value += delta;
                bool flipAngle = false;
                posXY.Value.x = math.select(posXY.Value.x, posXY.Value.x * -1, posXY.Value.x < 0);
                flipAngle = posXY.Value.x < 0;

                posXY.Value.y = math.select(posXY.Value.y, posXY.Value.y * -1, posXY.Value.y < 0);
                flipAngle = posXY.Value.y < 0;

                posXY.Value.x = math.select(posXY.Value.x, 2 * PosXY.bounds.x - posXY.Value.x,
                    posXY.Value.x > PosXY.bounds.x);
                flipAngle = posXY.Value.x > PosXY.bounds.x;

                posXY.Value.y = math.select(posXY.Value.y, 2 * PosXY.bounds.y - posXY.Value.y,
                    posXY.Value.y > PosXY.bounds.y);
                flipAngle = posXY.Value.y > PosXY.bounds.y;

                facing.Value = math.select(facing.Value, facing.Value += math.PI, flipAngle);

                facing.Value %= 2f * math.PI;
            }
            
            if (eatAmount != 0) {
                ecb.SetComponent(entityInQueryIndex,patch.Value,new AdjustFoodArea(){Value = -1*eatAmount});
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
           eatMultiplier = ExperimentSetting.eatMultiplier,
           turnAngle = ExperimentSetting.turnAngleRadian
        }; 
        var jobHandle = job.Schedule(m_Group, inputDependencies);
        m_EndSimulationEcbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;


    }
    
   
}



