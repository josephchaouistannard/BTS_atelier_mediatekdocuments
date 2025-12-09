using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class RayonTests
    {
        [TestMethod()]
        public void RayonTest()
        {
            string id = "test";
            string libelle = "test";

            Rayon unRayon = new Rayon(id, libelle);

            Assert.AreEqual(id, unRayon.Id);
            Assert.AreEqual(libelle, unRayon.Libelle);
        }
    }
}