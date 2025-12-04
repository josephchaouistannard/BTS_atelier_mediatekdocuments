using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier qui répresente une ligne dans la table abonnement de la BDD
    /// </summary>
    public class Abonnement
    {
        public string Id { get; }
        public DateTime DateCommande { get; }
        public DateTime DateFinAbonnement { get; }
        public double Montant { get; }
        public string IdRevue { get; }

        public Abonnement(string id, DateTime dateCommande, 
            DateTime dateFinAbonnement, double montant, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            DateFinAbonnement = dateFinAbonnement;
            Montant = montant;
            IdRevue = idRevue;
        }
    }
}
