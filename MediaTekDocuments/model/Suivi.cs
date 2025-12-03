
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi (étape de suivi pour les commandes de livre ou dvd)
    /// </summary>
    public class Suivi
    {
        public int Id { get; }
        public string Libelle { get; }

        public Suivi(int id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        /// <summary>
        /// Récupération du libellé pour l'affichage dans les combos
        /// </summary>
        /// <returns>Libelle</returns>
        public override string ToString()
        {
            return this.Libelle;
        }

    }
}
