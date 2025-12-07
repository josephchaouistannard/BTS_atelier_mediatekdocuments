using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaTekDocuments.dal;
using MediaTekDocuments.model;

namespace MediaTekDocuments.controller
{
    class FrmConnexionController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        public FrmConnexionController()
        {
            access = Access.GetInstance();
        }

        public List<Utilisateur> ConnexionUtilisateur(string login, string mdp)
        {
            return access.ConnexionUtilisateur(login, mdp);
        }
    }
}
