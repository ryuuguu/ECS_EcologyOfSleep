
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class SetActionTest {

        [Test]
        public void GenomeTest_IndexSetGet() {
            Genome.Allele result;
            //Choose
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Choose,0, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Choose,0, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Choose, 0, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Choose, 0, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Choose, 1, 0);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Choose, 1, 0);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Choose ,1, 0);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Choose ,1, 0);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Choose, 1, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Choose ,1, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Choose, 1, 1);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Choose, 1, 1);
            
            //Eat
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Eat,0, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Eat,0, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Eat, 0, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Eat, 0, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Eat, 1, 0);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Eat, 1, 0);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Eat ,1, 0);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Eat,1, 0);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Eat, 1, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Eat ,1, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Eat, 1, 1);
            Assert.AreEqual(result, Genome.Allele.Eat,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Eat, 1, 1);
 
            //Sleep
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Sleep,0, 1);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Sleep,0, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Sleep, 0, 1);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Sleep, 0, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Sleep, 1, 0);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Sleep, 1, 0);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Sleep ,1, 0);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Sleep,1, 0);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Eat,Genome.Allele.Sleep, 1, 1);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Eat,Genome.Allele.Sleep ,1, 1);
            
            result = SetActionSystem.SelectAction(Genome.Allele.Sleep,Genome.Allele.Sleep, 1, 1);
            Assert.AreEqual(result, Genome.Allele.Sleep,"{0} {1} {2} {3}",Genome.Allele.Sleep,Genome.Allele.Sleep, 1, 1);
            
        }
    }
}
