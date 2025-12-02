using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Ajouter un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool AjouterLivre(Livre livre)
        {
            return access.AjouterLivre(livre);
        }

        /// <summary>
        /// Modifier un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool ModifierLivre(Livre livre)
        {
            return access.ModifierLivre(livre);
        }

        /// <summary>
        /// Supprimer un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool SupprimerLivre(Livre livre)
        {
            return access.SupprimerLivre(livre);
        }

        /// <summary>
        /// Ajouter un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool AjouterDvd(Dvd dvd)
        {
            return access.AjouterDvd(dvd);
        }

        /// <summary>
        /// Modifier un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool ModifierDvd(Dvd dvd)
        {
            return access.ModifierDvd(dvd);
        }

        /// <summary>
        /// Supprimer un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            return access.SupprimerDvd(dvd);
        }

        /// <summary>
        /// Ajouter une revue dans la BDD
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool AjouterRevue(Revue revue)
        {
            return access.AjouterRevue(revue);
        }

        /// <summary>
        /// Modifier une revue dans la BDD
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool ModifierRevue(Revue revue)
        {
            return access.ModifierRevue(revue);
        }

        /// <summary>
        /// Supprimer une revue dans la BDD
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool SupprimerRevue(Revue revue)
        {
            return access.SupprimerRevue(revue);
        }
    }
}
