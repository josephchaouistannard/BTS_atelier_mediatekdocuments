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
    public class PublicTests
    {
        [TestMethod()]
        public void PublicTest()
        {
            string id = "test";
            string libelle = "test";

            Public unPublic = new Public(id, libelle);

            Assert.AreEqual(id, unPublic.Id);
            Assert.AreEqual(libelle, unPublic.Libelle);
        }
    }
}