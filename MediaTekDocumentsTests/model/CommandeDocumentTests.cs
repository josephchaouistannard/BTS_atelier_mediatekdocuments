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
    public class CommandeDocumentTests
    {
        [TestMethod()]
        public void CommandeDocumentTest()
        {
            string id = "00001";
            int nbExemplaire = 99;
            string idLivreDvd = "20003";
            int idSuivi = 9;
            string suivi = "test";
            DateTime dateCommande = DateTime.Now;
            double montant = 99.99;

            CommandeDocument com = new CommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi, suivi, dateCommande, montant);

            Assert.AreEqual(id, com.Id);
            Assert.AreEqual(nbExemplaire, com.NbExemplaire);
            Assert.AreEqual(idLivreDvd, com.IdLivreDvd);
            Assert.AreEqual(idSuivi, com.IdSuivi);
            Assert.AreEqual(suivi, com.EtSuivi);
            Assert.AreEqual(dateCommande, com.DateCommande);
            Assert.AreEqual(montant, com.Montant);
        }
    }
}