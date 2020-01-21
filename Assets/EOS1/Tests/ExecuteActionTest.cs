
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
    public class ExecuteActionTest : ECSTestsFixture {

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
            Experiment.hour = 0;
            Experiment.turnAngleRadian = math.PI / 2f; //90º
            Experiment.incrMultiplier = 3;
            experiment = new Experiment();
            experiment.em = m_Manager;
            experiment.SetRandomSeed(1);
            experiment.SetupPatches(1,3, 3);
            agent = experiment.SetupAgent(new float2(1.5f, 1.5f),0);
        }

        [TearDown]
        public override void TearDown() {
            base.TearDown();
        }
        
        
        [Test]
        public void ExecuteActionSystem_CantEatTest() {
            // set up eat as first action and test for correct results 
            var centerPatch = Experiment.patches[1, 1,0];
            m_Manager.SetComponentData(centerPatch, new FoodArea(){Value = 0});
            m_Manager.SetComponentData(agent, new Facing() {Value = 0, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Eat});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(-1, m_Manager.GetComponentData<SleepEnergy>(agent).Value,"SleepEnergy");
            Assert.AreEqual(-1, m_Manager.GetComponentData<FoodEnergy>(agent).Value,"FoodEnergy");
            Assert.AreEqual(Experiment.turnAngleRadian*-1f, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(new float2(1.5f, 0.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
        }
        
        [Test]
        public void ExecuteActionSystem_EatTest() {
             // set up eat as first action and test for correct results 
             var centerPatch = Experiment.patches[1, 1,0];
             m_Manager.SetComponentData(centerPatch, new FoodArea(){Value = 2});
             m_Manager.SetComponentData(agent, new Facing(){Value = 0, random = new Random(1)});
             m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Eat});
             World.CreateSystem<ExecuteActionSystem>().Update();
             var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
             Assert.AreEqual(-1, m_Manager.GetComponentData<SleepEnergy>(agent).Value,"SleepEnergy");
             Assert.AreEqual(3, m_Manager.GetComponentData<FoodEnergy>(agent).Value,"FoodEnergy");
             Assert.AreEqual(0, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
             Assert.AreEqual(new float2(1.5f, 1.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
             var endSimulationEcbSystem = World
                 .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
             Assert.IsFalse( m_Manager.HasComponent<AdjustFoodArea>(centerPatch));
             endSimulationEcbSystem.Update();
             Assert.IsTrue( m_Manager.HasComponent<AdjustFoodArea>(centerPatch));
             
        }
        
        
        [Test]
        public void ExecuteActionSystem_SleepTest() {
            // set up sleep as first action and test for correct results 
            var centerPatch = Experiment.patches[1, 1,0];
            m_Manager.SetComponentData(centerPatch, new SleepArea(){Value = true});
            m_Manager.SetComponentData(agent, new Facing(){Value = 0, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Sleep});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(Experiment.incrMultiplier, m_Manager.GetComponentData<SleepEnergy>(agent).Value,"SleepEnergy");
            Assert.AreEqual(-1, m_Manager.GetComponentData<FoodEnergy>(agent).Value,"FoodEnergy");
            Assert.AreEqual(0, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(new float2(1.5f, 1.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
        }
        
        [Test]
        public void ExecuteActionSystem_CantSleepTest() {
            // set up sleep as first action and test for correct results 
            var centerPatch = Experiment.patches[1, 1,0];
            m_Manager.SetComponentData(centerPatch, new SleepArea(){Value = false});
            m_Manager.SetComponentData(agent, new Facing() {Value = 0, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Sleep});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(-1, m_Manager.GetComponentData<SleepEnergy>(agent).Value,"SleepEnergy");
            Assert.AreEqual(-1, m_Manager.GetComponentData<FoodEnergy>(agent).Value,"FoodEnergy");
            Assert.AreEqual(Experiment.turnAngleRadian*-1f, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(new float2(1.5f, 0.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
        }
        
        [Test]
        public void ExecuteActionSystem_BounceY0Test() {
            agent = experiment.SetupAgent(new float2(1.5f, 0.5f),0);
            m_Manager.SetComponentData(agent, new Facing() {Value = 0, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Sleep});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(Experiment.turnAngleRadian, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(new float2(1.5f, 0.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
            
        }
        
        [Test]
        public void ExecuteActionSystem_BounceY2Test() {
            agent = experiment.SetupAgent(new float2(1.5f, 2.5f),0);
            m_Manager.SetComponentData(agent, new Facing() {Value = math.PI, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Sleep});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(Experiment.turnAngleRadian*3, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(new float2(1.5f, 2.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
            
        }
        
        [Test]
        public void ExecuteActionSystem_BounceX2Test() {
            agent = experiment.SetupAgent(new float2(2.5f, 1.5f),0);
            m_Manager.SetComponentData(agent, new Facing() {Value = math.PI/2f, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Sleep});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(math.PI, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(new float2(2.5f, 1.5f), m_Manager.GetComponentData<PosXY>(agent).Value,"PosXY");
        }
        
        [Test]
        public void ExecuteActionSystem_BounceX0Test() {
            agent = experiment.SetupAgent(new float2(0.5f, 1.5f),0);
            m_Manager.SetComponentData(agent, new Facing() {Value = -1*math.PI/2f, random = new Random(1)});
            m_Manager.SetComponentData(agent, new Action(){Value = Genome.Allele.Sleep});
            World.CreateSystem<ExecuteActionSystem>().Update();
            var agentPatch = m_Manager.GetComponentData<Patch>(agent).Value;
            Assert.AreEqual(0, m_Manager.GetComponentData<Facing>(agent).Value,"Facing");
            Assert.AreEqual(0.5f, m_Manager.GetComponentData<PosXY>(agent).Value.x,0.0001,"PosXY.x ");
            Assert.AreEqual(1.5f, m_Manager.GetComponentData<PosXY>(agent).Value.y,0.0001,"PosXY.y");
        }
        
    }
}
