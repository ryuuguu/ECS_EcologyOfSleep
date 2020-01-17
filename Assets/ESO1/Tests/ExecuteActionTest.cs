
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;

namespace Tests {


    [TestFixture]
    //[Category("ECS Test")]
    public class ExecuteActionTest : ECSTestsFixture {

        protected Entity agent;
        protected ExperimentSetting experimentSetting;

        
        
        //ref FoodEnergy foodEnergy,
        //ref SleepEnergy sleepEnergy,ref PosXY posXY, ref Facing facing, [ReadOnly] ref Action action, 
        //[ReadOnly] ref Patch patch)
        [SetUp]
        public override void Setup() {
            base.Setup();
            ExperimentSetting.hour = 0;
            ExperimentSetting.turnAngleRadian = math.PI / 2f; //90º
            ExperimentSetting.eatMultiplier = 3;
            var go = new GameObject("ExperimentSetting");
            experimentSetting = go.AddComponent<ExperimentSetting>();
            experimentSetting.em = m_Manager;
            experimentSetting.SetRandomSeed(1);
            experimentSetting.SetupPatches(3, 3);
            agent = experimentSetting.SetupAgent(new float2(1.5f, 1.5f));
            
        }

        [TearDown]
        public override void TearDown() {
            base.TearDown();
        }
        
      
        [Test]
        public void ExecuteActionSystem_EatTest() {
            // set up eat as first action and test for correct results 
            var centerPatch = ExperimentSetting.patches[1, 1];
            m_Manager.SetComponentData(centerPatch, new FoodArea(){Value = 2});
            World.CreateSystem<ExecuteActionSystem>().Update();
            Assert.AreEqual(-1, m_Manager.GetComponentData<SleepEnergy>(agent).Value);
            //sleep -1
            //eat 3
            // action = eat 
            // 
        }
    }
}
