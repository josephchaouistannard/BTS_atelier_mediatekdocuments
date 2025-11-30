
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Genre : hérite de Categorie
    /// </summary>
    public class Genre : Categorie
    {
        public Genre(string id, string libelle) : base(id, libelle)
        {
        }

    }
}
