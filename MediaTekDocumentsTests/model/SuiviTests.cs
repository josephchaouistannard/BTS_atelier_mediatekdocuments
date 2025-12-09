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
    public class SuiviTests
    {
        [TestMethod()]
        public void SuiviTest()
        {
            int id = 2;
            string libelle = "test";

            Suivi unSuivi = new Suivi(id, libelle);

            Assert.AreEqual(id, unSuivi.Id);
            Assert.AreEqual(libelle, unSuivi.Libelle);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            int id = 2;
            string libelle = "test";

            Suivi unSuivi = new Suivi(id, libelle);

            Assert.AreEqual(unSuivi.ToString(), libelle);
        }
    }
}