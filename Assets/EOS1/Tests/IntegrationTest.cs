
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Tests {


    [TestFixture]
    //[Category("ECS Test")]
    public class ItegrationTest : ECSTestsFixture {

        protected Entity agent;
        protected Experiment experiment;
        
        /// <summary>
        ///
        /// This code produces randomResult = false
        /// var testRandom = new Random(1);
        /// var randomResult =  testRandom.NextUInt(0, 2) == 0 ? false : true;
        ///
        /// Adding two call to Next...()
        /// produces randomResult = true
        /// var testRandom = new Random(1);
        ///testRandom.NextUInt();
        ///testRandom.NextUInt();
        ///var randomResult =  testRandom.NextUInt(0, 2) == 0 ? false : true; 
        /// </summary>
        [SetUp]
        public override void Setup() {
            base.Setup();
            Experiment.StaticSetup(m_Manager);
            Experiment.hour = 0;
            Experiment.turnAngleRadian = math.PI / 2f; //90º
            Experiment.incrMultiplier = 3;
            experiment = new Experiment();
            experiment.SetRandomSeed(1);
            experiment.SetupPatches(1,3, 3);
            agent = experiment.SetupAgent(new float2(1.5f, 1.5f),0);
        }

        [TearDown]
        public override void TearDown() {
            base.TearDown();
        }
        
        /// <summary>
        /// run three complete Ticks
        /// check AdjustFoodArea Component & Systems
        ///
        /// does not do tick advancement just repeats first action
        /// </summary>
        [Test]
        public void ThreeTickFoodTest() {
            // set up eat as first  actions and test for correct results 
            var centerPatch = Experiment.GetPatchAt(1, 1,0);
            Experiment.turnAngleRadian = 0; // always want to move straight for test.
            m_Manager.SetComponentData(centerPatch, new FoodArea(){Value = 2});
            m_Manager.SetComponentData(agent, new Facing() {Value = 0, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Eat});
            var eatGenome = new Genome();
            for(int i= 0; i<24;i++) {
                eatGenome[i] = Genome.Allele.Eat;
            };
            m_Manager.AddComponentData(agent, eatGenome); 
            
           
            World.CreateSystem<AdjustFoodAreaSystem>().Update();
            World.CreateSystem<SetActionSystem>().Update();
            World.CreateSystem<ExecuteActionSystem>().Update();
            World.CreateSystem<SetPatchSystem>().Update();
            var endSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            Assert.IsFalse( m_Manager.HasComponent<AdjustFoodArea>(centerPatch),"Tick 0 pre");
            Assert.AreEqual(2, m_Manager.GetComponentData<FoodArea>(centerPatch).Value,"FoodArea Tick 0 pre");
            // adjustFood added
            endSimulationEcbSystem.Update();
            Experiment.NextTick();
            
            Assert.AreEqual(-1, m_Manager.GetComponentData<AdjustFoodArea>(centerPatch).Value,"AdjustFoodArea Tick 0 post");
            Assert.IsTrue( m_Manager.HasComponent<AdjustFoodArea>(centerPatch),"Tick 0 post");
            Assert.AreEqual(3, m_Manager.GetComponentData<FoodEnergy>(agent).Value,"FoodEnergy Tick 0 post"); 
            Assert.AreEqual(Genome.Allele.Eat, m_Manager.GetComponentData<Action>(agent).Value,"Action Tick 0 post"); 
            Assert.AreEqual(centerPatch,m_Manager.GetComponentData<Patch>(agent).Value);
            Assert.AreEqual(new float2(1.5f, 1.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
            
           
            World.CreateSystem<AdjustFoodAreaSystem>().Update();
            World.CreateSystem<SetActionSystem>().Update();
            World.CreateSystem<ExecuteActionSystem>().Update();
            World.CreateSystem<SetPatchSystem>().Update();
            Assert.AreEqual(1, m_Manager.GetComponentData<FoodArea>(centerPatch).Value,"FoodArea Tick1 pre");
            Assert.AreEqual(Genome.Allele.Eat, m_Manager.GetComponentData<Action>(agent).Value,"Action Tick 1 pre B");
            Assert.AreEqual(6, m_Manager.GetComponentData<FoodEnergy>(agent).Value,"FoodEnergy Tick1 pre"); 
            Assert.IsTrue( m_Manager.HasComponent<AdjustFoodArea>(centerPatch),"Tick 1 pre");
            Assert.AreEqual(centerPatch,m_Manager.GetComponentData<Patch>(agent).Value);
            Assert.AreEqual(new float2(1.5f, 1.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
         
            // adjust food added & adjustfood removed
            endSimulationEcbSystem.Update();
            Experiment.NextTick();
            
            Assert.IsTrue( m_Manager.HasComponent<AdjustFoodArea>(centerPatch),"Tick 1 post");
            
            
            World.CreateSystem<AdjustFoodAreaSystem>().Update();
            World.CreateSystem<SetActionSystem>().Update();
            World.CreateSystem<ExecuteActionSystem>().Update();
            World.CreateSystem<SetPatchSystem>().Update();
            Assert.IsTrue( m_Manager.HasComponent<AdjustFoodArea>(centerPatch),"Tick 2 pre");
            Assert.AreEqual(Experiment.GetPatchAt(2, 1,0),m_Manager.GetComponentData<Patch>(agent).Value);
            Assert.AreEqual(new float2(2.5f, 1.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
            
            // adjust food adjustFood removed
            endSimulationEcbSystem.Update();
            Experiment.NextTick();
            
            Assert.IsFalse( m_Manager.HasComponent<AdjustFoodArea>(centerPatch),"Tick 2 post");
            
            World.CreateSystem<SetPatchSystem>().Update();
            
        }
        
    }
}
