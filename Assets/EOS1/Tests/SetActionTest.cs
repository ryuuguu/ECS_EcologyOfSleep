
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.Entities.Tests;


namespace Tests {


    [TestFixture]
    //[Category("ECS Test")]
    public class SetActionTest : ECSTestsFixture {
        
        [Test]
        public void SetActionSystemTest() {

            Experiment1.hour = 0;
            var instance = m_Manager.CreateEntity();
            m_Manager.AddComponentData(instance, new Genome {h0 = Genome.Allele.Eat});
            m_Manager.AddComponentData(instance, new Action {Value = (Genome.Allele) Genome.Allele.Eat});
            m_Manager.AddComponentData(instance, new FoodEnergy() {Value = 0});
            m_Manager.AddComponentData(instance, new SleepEnergy() {Value = 0});
            var setAction = World.CreateSystem<SetActionSystem>();
            Genome.Allele action ;
            Genome.Allele currAllele ;
            Genome.Allele expected ;    
            
            // test Eat & sleep Alleles in genome
            for (int i = 0; i < 2; i++) {
                if (i == 0) {
                    m_Manager.SetComponentData(instance, new Genome {h0 = Genome.Allele.Eat});
                    currAllele = Genome.Allele.Eat;
                    expected = Genome.Allele.Eat;
                }
                else {
                    m_Manager.SetComponentData(instance, new Genome {h0 = Genome.Allele.Sleep});
                    currAllele = Genome.Allele.Sleep;
                    expected = Genome.Allele.Sleep; 
                }

                //This also tests for prev Action == Choose which is not a valid condition 
                // but as long this does not fail it does not cause a problem.
                for (int prevAction = 0; prevAction < 3; prevAction++) {
                    m_Manager.SetComponentData(instance, new Action {Value = (Genome.Allele) prevAction});
                    for (int foodNRG = 0; foodNRG < 2; foodNRG++) {
                        m_Manager.SetComponentData(instance, new FoodEnergy() {Value = foodNRG});
                        for (int sleepNRG = 0; sleepNRG < 2; sleepNRG++) {
                            m_Manager.SetComponentData(instance, new SleepEnergy() {Value = sleepNRG});
                            setAction.Update();
                            action = m_Manager.GetComponentData<Action>(instance).Value;
                            Assert.AreEqual(expected, action, "{0} {1} {2} {3}", prevAction,
                                currAllele, 0, 1);
                        }
                    }
                }
            }

            Genome.Allele prevAction2;
            // test Choose Alleles in genome
            m_Manager.SetComponentData(instance, new Genome {h0 = Genome.Allele.Choose});
            currAllele = Genome.Allele.Choose;
            
            // FoodNrg == SleepNRG
            m_Manager.SetComponentData(instance, new FoodEnergy() {Value = 0});
            m_Manager.SetComponentData(instance, new SleepEnergy() {Value = 0});
            for (int i = 0; i < 2; i++) {
                if (i == 0) {
                    m_Manager.SetComponentData(instance, new Action() {Value = Genome.Allele.Eat});
                    prevAction2 = Genome.Allele.Eat;
                    expected = Genome.Allele.Eat;
                }
                else {
                    m_Manager.SetComponentData(instance, new Action {Value = Genome.Allele.Sleep});
                    prevAction2 = Genome.Allele.Sleep;
                    expected = Genome.Allele.Sleep;
                }

                setAction.Update();
                action = m_Manager.GetComponentData<Action>(instance).Value;
                Assert.AreEqual(expected, action, "{0} {1} {2} {3}", prevAction2, currAllele, 0, 0);
            }
            
            // FoodNrg > SleepNRG
            var foodEnergy = new FoodEnergy() {Value = 100};
            var foodFitness = foodEnergy.Fitness();
            var sleepEnergy = new SleepEnergy() {Value = 0};
            var sleepFitness = sleepEnergy.Fitness();
            m_Manager.SetComponentData(instance, foodEnergy);
            m_Manager.SetComponentData(instance, sleepEnergy);
            for (int i = 0; i < 2; i++) {
                if (i == 0) {
                    m_Manager.SetComponentData(instance, new Action() {Value = Genome.Allele.Eat});
                    prevAction2 = Genome.Allele.Eat;
                    expected = Genome.Allele.Sleep;
                }
                else {
                    m_Manager.SetComponentData(instance, new Action {Value = Genome.Allele.Sleep});
                    prevAction2 = Genome.Allele.Sleep;
                    expected = Genome.Allele.Sleep;
                }

                setAction.Update();
                action = m_Manager.GetComponentData<Action>(instance).Value;
                Assert.AreEqual(expected, action, "{0} {1} {2} {3} FF{4} SF {5}",
                    prevAction2, currAllele, foodEnergy.Value, sleepEnergy.Value, foodFitness, sleepFitness );
            }
            
            // FoodNrg < SleepNRG
            
            foodEnergy = new FoodEnergy() {Value = 0};
            foodFitness = foodEnergy.Fitness();
            sleepEnergy = new SleepEnergy() {Value = 100};
            sleepFitness = sleepEnergy.Fitness();
            m_Manager.SetComponentData(instance, foodEnergy);
            m_Manager.SetComponentData(instance, sleepEnergy);
            for (int i = 0; i < 2; i++) {
                if (i == 0) {
                    m_Manager.SetComponentData(instance, new Action() {Value = Genome.Allele.Eat});
                    prevAction2 = Genome.Allele.Eat;
                    expected = Genome.Allele.Eat;
                }
                else {
                    m_Manager.SetComponentData(instance, new Action {Value = Genome.Allele.Sleep});
                    prevAction2 = Genome.Allele.Sleep;
                    expected = Genome.Allele.Eat;
                }

                setAction.Update();
                action = m_Manager.GetComponentData<Action>(instance).Value;
                Assert.AreEqual(expected, action, "{0} {1} {2} {3} FF{4} SF {5}",
                    prevAction2, currAllele, foodEnergy.Value, sleepEnergy.Value, foodFitness, sleepFitness );
            }
        }
    }
}
