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
    public class LivreTests
    {
        [TestMethod()]
        public void LivreTest()
        {
            // Livre
            string isbn = "978-2070612758";
            string auteur = "Victor Hugo";
            string collection = "Folio";

            // classe mère
            string id = "10003";
            string titre = "Les Misérables";
            string image = "Couverture.jpg";
            string idGenre = "ROM";
            string genre = "Roman";
            string idPublic = "ADU";
            string lePublic = "Adultes";
            string idRayon = "R01";
            string rayon = "Littérature";

            Livre livre = new Livre(
                id, titre, image,
                isbn, auteur, collection,
                idGenre, genre, idPublic, lePublic, idRayon, rayon
            );

            Assert.AreEqual(isbn, livre.Isbn);
            Assert.AreEqual(auteur, livre.Auteur);
            Assert.AreEqual(collection, livre.Collection);

            // classe mère
            Assert.AreEqual(id, livre.Id);
            Assert.AreEqual(titre, livre.Titre);
            Assert.AreEqual(image, livre.Image);
            Assert.AreEqual(idGenre, livre.IdGenre);
            Assert.AreEqual(genre, livre.Genre);
            Assert.AreEqual(idPublic, livre.IdPublic);
            Assert.AreEqual(lePublic, livre.Public);
            Assert.AreEqual(idRayon, livre.IdRayon);
            Assert.AreEqual(rayon, livre.Rayon);
        }
    }
}