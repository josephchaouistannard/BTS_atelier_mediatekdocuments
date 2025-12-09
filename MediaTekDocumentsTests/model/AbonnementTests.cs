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
    public class AbonnementTests
    {
        [TestMethod()]
        public void AbonnementTest()
        {
            string id = "00001";
            DateTime dateCommande = new DateTime(2025, 12, 1);
            DateTime dateFinAbonnement = new DateTime(2026, 12, 1);
            double montant = 100.45;
            string idRevue = "10007";

            Abonnement ab = new Abonnement(id, dateCommande, dateFinAbonnement, montant, idRevue);

            Assert.AreEqual(ab.Id, id);
            Assert.AreEqual(ab.DateCommande, dateCommande);
            Assert.AreEqual(ab.DateFinAbonnement, dateFinAbonnement);
            Assert.AreEqual(ab.Montant, montant);
            Assert.AreEqual(ab.IdRevue, idRevue);

        }
    }
}