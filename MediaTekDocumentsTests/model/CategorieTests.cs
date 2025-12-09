using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class CategorieTests
    {
        [TestMethod()]
        public void CategorieTest()
        {
            string id = "test";
            string libelle = "test";

            Categorie cat = new Categorie(id, libelle);

            Assert.AreEqual(id, cat.Id);
            Assert.AreEqual(libelle, cat.Libelle);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            string id = "test";
            string libelle = "test";

            Categorie cat = new Categorie(id, libelle);

            Assert.AreEqual(cat.ToString(), libelle);
        }
    }
}