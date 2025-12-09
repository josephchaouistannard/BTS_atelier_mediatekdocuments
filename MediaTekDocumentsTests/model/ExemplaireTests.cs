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
    public class ExemplaireTests
    {
        [TestMethod()]
        public void ExemplaireTest()
        {
            int numero = 5;
            DateTime dateAchat = new DateTime(2025, 12, 1);
            string photo = "Photo.jpg";
            string idEtat = "00001";
            string libelle = "Neuf";
            string idDocument = "10002";

            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, libelle, idDocument);

            Assert.AreEqual(numero, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual(photo, exemplaire.Photo);
            Assert.AreEqual(idEtat, exemplaire.IdEtat);
            Assert.AreEqual(libelle, exemplaire.LibelleEtat);
            Assert.AreEqual(idDocument, exemplaire.Id);
        }
    }
}