using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEditor;


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




