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
    public class GenreTests
    {
        [TestMethod()]
        public void GenreTest()
        {
            string id = "test";
            string libelle = "test";

            Genre genre = new Genre(id, libelle);

            Assert.AreEqual(id, genre.Id);
            Assert.AreEqual(libelle, genre.Libelle);
        }
    }
}