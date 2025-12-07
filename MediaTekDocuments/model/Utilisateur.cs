using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Utilisateur
    /// </summary>
    public class Utilisateur
    {
            public int Id { get; }
            public string Login { get; }
            public int IdService { get; }
            public string LibelleService { get; }

            public Utilisateur(int id, string login, int idService, string libelle)
            {
                this.Id = id;
                this.Login = login;
                this.IdService = idService;
                this.LibelleService = libelle;
            }
    }
}
