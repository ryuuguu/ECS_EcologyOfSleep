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
                genome[i] = i;
                Assert.AreEqual(genome[i], i);
            }
        }

        [Test]
        public void GenomeTest_IndexSetOutOfRange() {
            var genome = new Genome();
            Assert.That(()=> genome[24] = 1, Throws.ArgumentException);
        }
        
        
        [Test]
        public void GenomeTest_IndexGetOutOfRange() {
            var genome = new Genome();
            Assert.That(()=> genome[24] > 1, Throws.ArgumentException);
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
