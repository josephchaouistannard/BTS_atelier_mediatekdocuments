using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier qui répresente une ligne dans la table commandedocument de la BDD
    /// </summary>
    public class CommandeDocument
    {
        public string Id { get; }
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }
        public int IdSuivi { get; }
        public string EtSuivi { get; }
        public DateTime DateCommande { get; }
        public double Montant { get; }

        public CommandeDocument(string id, int nbExemplaire, 
            string idLivreDvd, int idSuivi, string suivi, 
            DateTime dateCommande, double montant)
        {
            Id = id;
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
            IdSuivi = idSuivi;
            EtSuivi = suivi;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }
}
