
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
    public class NoPatchFlagTest : ECSTestsFixture {

        protected Entity agent;
        protected Experiment1 experiment;
        
        [SetUp]
        public override void Setup() {
            base.Setup();
            EOSGrid.displaySimID = 1; // !=0 so EOSGrid instance methods will not be called
            experiment = new Experiment1();
            experiment.em = m_Manager;
            experiment.SetRandomSeed(1);
            experiment.SetupPatches(1,3, 3);
            agent = experiment.SetupAgent(new float2(1.5f, 1.5f),0);
            experiment.FoodCluster(0, new int2(3,3),new float2(0,0),1,1,1,1, new Random(1));
            experiment.SleepCluster(0, new int2(3,3), new float2(2,2), 1, 1,new Random(1) );
        }

        [TearDown]
        public override void TearDown() {
            base.TearDown();
        }

        [Test]
        public void SetPatchNoPatchSystem_EmptyTest() {
            m_Manager.AddComponentData(agent, new NoPatchFlag());
            World.CreateSystem<SetPatchNoPatchSystem>().Update();
            var hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(true, hasNoPatchFlag, "Empty NoPatchFlag");
        }
        
        [Test]
        public void SetPatchNoPatchSystem_FoodTest() {
            agent = experiment.SetupAgent(new float2(0.5f, 0.5f),0);
            m_Manager.AddComponentData(agent, new NoPatchFlag());
            World.CreateSystem<SetPatchNoPatchSystem>().Update();
            var hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(false,hasNoPatchFlag,"Food NoPatchFlag");
        }
        
        [Test]
        public void SetPatchNoPatchSystem_SleepTest() { 
            agent = experiment.SetupAgent(new float2(2.5f, 2.5f),0);
            m_Manager.AddComponentData(agent, new NoPatchFlag());
            World.CreateSystem<SetPatchNoPatchSystem>().Update();
            var hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(false,hasNoPatchFlag,"Sleep NoPatchFlag");
        }
        
        [Test]
        public void SetPatchNoPatchNoneSystem_Test() {
            var hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(false,hasNoPatchFlag,"Initial NoPatchFlag");
            
            World.CreateSystem<SetPatchNoPatchNoneSystem>().Update();
            hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(true,hasNoPatchFlag,"Empty NoPatchFlag");
            
            m_Manager.RemoveComponent<NoPatchFlag>(agent);
            World.CreateSystem<SetPatchNoPatchNoneSystem>().Update();
            agent = experiment.SetupAgent(new float2(0.5f, 0.5f),0);
            hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(false,hasNoPatchFlag,"Food NoPatchFlag");
            
            
            World.CreateSystem<SetPatchNoPatchNoneSystem>().Update();
            agent = experiment.SetupAgent(new float2(2.5f, 2.5f),0);
            hasNoPatchFlag = m_Manager.HasComponent<NoPatchFlag>(agent);
            Assert.AreEqual(false,hasNoPatchFlag,"Sleep NoPatchFlag");
        }
        
    }
}
