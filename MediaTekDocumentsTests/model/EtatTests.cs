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
    public class EtatTests
    {
        [TestMethod()]
        public void EtatTest()
        {
            string id = "test";
            string libelle = "test";

            Etat et = new Etat(id, libelle);

            Assert.AreEqual(id, et.Id);
            Assert.AreEqual(libelle, et.Libelle);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            string id = "test";
            string libelle = "test";

            Etat et = new Etat(id, libelle);

            Assert.AreEqual(libelle, et.ToString());
        }
    }
}