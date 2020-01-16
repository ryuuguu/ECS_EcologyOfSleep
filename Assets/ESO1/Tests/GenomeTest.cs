using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class GenomeTest {
        
        [Test]
        public void GenomeTest_IndexSetGet() {
            var genome = new Genome();
            for (int i = 0; i < 24; i++) {
                
                genome[i] = Genome.Allele.Eat;
                Assert.AreEqual(genome[i], Genome.Allele.Eat);
            }
        }

        [Test]
        public void GenomeTest_IndexSetOutOfRange() {
            var genome = new Genome();
            Assert.That(()=> genome[24] = Genome.Allele.Eat, Throws.ArgumentException);
        }
        
        
        [Test]
        public void GenomeTest_IndexGetOutOfRange() {
            var genome = new Genome();
            Assert.That(()=> genome[24] != Genome.Allele.Eat, Throws.ArgumentException);
        }

        /*
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator GenomeTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        */
    }
}
