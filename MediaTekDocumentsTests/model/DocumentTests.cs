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
    public class DocumentTests
    {
        [TestMethod()]
        public void DocumentTest()
        {
            string id = "10001";
            string titre = "Le Petit Prince";
            string image = "chemin/vers/image.jpg";
            string idGenre = "BD";
            string genre = "Bande Dessinée";
            string idPublic = "ADO";
            string lePublic = "Adolescents";
            string idRayon = "001";
            string rayon = "Littérature générale";

            Document doc = new Document(
                id,
                titre,
                image,
                idGenre,
                genre,
                idPublic,
                lePublic,
                idRayon,
                rayon
            );

            Assert.AreEqual(id, doc.Id);
            Assert.AreEqual(titre, doc.Titre);
            Assert.AreEqual(image, doc.Image);
            Assert.AreEqual(idGenre, doc.IdGenre);
            Assert.AreEqual(genre, doc.Genre);
            Assert.AreEqual(idPublic, doc.IdPublic);
            Assert.AreEqual(lePublic, doc.Public);
            Assert.AreEqual(idRayon, doc.IdRayon);
            Assert.AreEqual(rayon, doc.Rayon);
        }
    }
}