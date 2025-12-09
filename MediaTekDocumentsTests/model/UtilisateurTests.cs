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
    public class UtilisateurTests
    {
        [TestMethod()]
        public void UtilisateurTest()
        {
            int id = 42;
            string login = "admin";
            int idService = 1;
            string libelle = "Admin";

            Utilisateur utilisateur = new Utilisateur(id, login, idService, libelle);

            Assert.AreEqual(id, utilisateur.Id);
            Assert.AreEqual(login, utilisateur.Login);
            Assert.AreEqual(idService, utilisateur.IdService);
            Assert.AreEqual(libelle, utilisateur.LibelleService);
        }
    }
}