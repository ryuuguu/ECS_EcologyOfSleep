﻿using System.Collections;
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
    public class ExperimentTest : ECSTestsFixture {

        protected Entity agent;
        protected Experiment experiment;

        [SetUp]
        public override void Setup() {
            base.Setup();
            Experiment.StaticSetup(m_Manager);
            EOSGrid.displaySimID = 1; // !=0 so EOSGrid instance methods will not be called
            experiment = new Experiment();
            experiment.SetRandomSeed(1);
            experiment.SetupPatches(1, 3, 3);
            //agent = experiment.SetupAgent(new float2(1.5f, 1.5f), 0);
            //experiment.FoodCluster(0, new int2(3, 3), new float2(0, 0), 1, 1, 1, 1, new Random(1));
            //experiment.SleepCluster(0, new int2(3, 3), new float2(2, 2), 1, 1, new Random(1));
        }

        [TearDown]
        public override void TearDown() {
            base.TearDown();
        }

        [Test]
        public void FoodCluster_R1Test() {
            Assert.AreEqual(0,m_Manager.GetComponentData<FoodArea>(Experiment.GetPatchAt(0,0,0)).Value, "Start Empty [0,0,0]");
            Assert.AreEqual(0,m_Manager.GetComponentData<FoodArea>(Experiment.GetPatchAt(0,1,0)).Value, "Start Empty [0,1,0]");
            experiment.FoodCluster(0, new int2(3, 3), new float2(0, 0), 1, 1, 1, 1, new Random(1));
            Assert.AreNotEqual(0,m_Manager.GetComponentData<FoodArea>(Experiment.GetPatchAt(0,0,0)).Value, "After cluster [0,0,0]");
            Assert.AreEqual(0,m_Manager.GetComponentData<FoodArea>(Experiment.GetPatchAt(0,1,0)).Value, "After cluster [0,1,0]"); 
           
        }
        
        [Test]
        public void SleepCluster_R1Test() {
            Assert.AreEqual(false,m_Manager.GetComponentData<SleepArea>(Experiment.GetPatchAt(2,2,0)).Value, "Start Empty [2,2,0]");
            Assert.AreEqual(false,m_Manager.GetComponentData<SleepArea>(Experiment.GetPatchAt(0,1,0)).Value, "Start Empty [0,1,0]");
            experiment.SleepCluster(0, new int2(3, 3), new float2(2, 2), 1, 1, new Random(1));
            Assert.AreEqual(true,m_Manager.GetComponentData<SleepArea>(Experiment.GetPatchAt(2,2,0)).Value, "After cluster [2,2,0]");
            Assert.AreEqual(false,m_Manager.GetComponentData<SleepArea>(Experiment.GetPatchAt(0,1,0)).Value, "After cluster [0,1,0]"); 
           
        }
    }
}