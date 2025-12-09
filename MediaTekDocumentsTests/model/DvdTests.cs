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
    public class DvdTests
    {
        [TestMethod()]
        public void DvdTest()
        {
            // classe mère
            string id = "20001";
            string titre = "Film Titre";
            string image = "chemin/vers/dvd_image.jpg";
            string idGenre = "FIC";
            string genre = "Fiction";
            string idPublic = "ADU";
            string lePublic = "Adultes";
            string idRayon = "R02";
            string rayon = "Rayon DVD";

            // Dvd
            int duree = 120;
            string realisateur = "John";
            string synopsis = "Ceci est un bref résumé du film.";

            Dvd dvd = new Dvd(
                id, titre, image,
                duree, realisateur, synopsis,
                idGenre, genre, idPublic, lePublic, idRayon, rayon
            );

            Assert.AreEqual(duree, dvd.Duree);
            Assert.AreEqual(realisateur, dvd.Realisateur);
            Assert.AreEqual(synopsis, dvd.Synopsis);

            // classe mère
            Assert.AreEqual(id, dvd.Id);
            Assert.AreEqual(titre, dvd.Titre);
            Assert.AreEqual(image, dvd.Image);
            Assert.AreEqual(idGenre, dvd.IdGenre);
            Assert.AreEqual(genre, dvd.Genre);
            Assert.AreEqual(idPublic, dvd.IdPublic);
            Assert.AreEqual(lePublic, dvd.Public);
            Assert.AreEqual(idRayon, dvd.IdRayon);
            Assert.AreEqual(rayon, dvd.Rayon);
        }
    }
}