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
    public class RevueTests
    {
        [TestMethod()]
        public void RevueTest()
        {
            // Revue
            string periodicite = "Mensuelle";
            int delaiMiseADispo = 15;

            // classe mère
            string id = "40001";
            string titre = "Science et Vie";
            string image = "Magazine.png";
            string idGenre = "SCI";
            string genre = "Scientifique";
            string idPublic = "ADU";
            string lePublic = "Adultes";
            string idRayon = "R03";
            string rayon = "Presse";

            Revue revue = new Revue(
                id, titre, image,
                idGenre, genre, idPublic, lePublic, idRayon, rayon,
                periodicite, delaiMiseADispo
            );

            Assert.AreEqual(periodicite, revue.Periodicite);
            Assert.AreEqual(delaiMiseADispo, revue.DelaiMiseADispo);

            // classe mère
            Assert.AreEqual(id, revue.Id);
            Assert.AreEqual(titre, revue.Titre);
            Assert.AreEqual(image, revue.Image);
            Assert.AreEqual(idGenre, revue.IdGenre);
            Assert.AreEqual(genre, revue.Genre);
            Assert.AreEqual(idPublic, revue.IdPublic);
            Assert.AreEqual(lePublic, revue.Public);
            Assert.AreEqual(idRayon, revue.IdRayon);
            Assert.AreEqual(rayon, revue.Rayon);
        }
    }
}