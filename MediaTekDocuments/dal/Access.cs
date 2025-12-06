using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using MediaTekDocuments.manager;
using MediaTekDocuments.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow.CommonModels;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";
        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne un livre par son id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Livre GetLivre(string id)
        {
            String jsonId = convertToJson("id", id);
            List<Livre> liste = TraitementRecup<Livre>(GET, "livre/" + jsonId, null);
            if (liste.Count > 0)
            {
                return liste[0];
            }
            return null;            
        }
            
        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne un dvd par son id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dvd GetDvd(string id)
        {
            String jsonId = convertToJson("id", id);
            List<Dvd> liste = TraitementRecup<Dvd>(GET, "dvd/" + jsonId, null);
            if (liste.Count > 0)
            {
                return liste[0];
            }
            return null;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns></returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Get tous les commandes, soit pour les livres soit pour les DVDs
        /// </summary>
        /// <param name="type">"livre" ou "dvd"</param>
        /// <returns></returns>
        public List<CommandeDocument> GetAllCommandesDocument(string type)
        {
            String jsonType = convertToJson("type", type);
            List<CommandeDocument> lesCommandesDocument = TraitementRecup<CommandeDocument>(GET, "commandes/" + jsonType, null);
            return lesCommandesDocument;
        }

        /// <summary>
        /// Get les commandes pour un livre ou dvd spécifique
        /// </summary>
        /// <param name="numDoc"></param>
        /// <returns></returns>
        public List<CommandeDocument> GetCommandesDocument(string numDoc)
        {
            String jsonId = convertToJson("id", numDoc);
            List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(GET, "commandes/" + jsonId, null);
            return liste;
        }

        /// <summary>
        /// Enregistrer une commande d'un livre ou dvd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool AjouterCommandeDocument(CommandeDocument commande)
        {
            String jsonCommande = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());
            Console.WriteLine(jsonCommande);
            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commande", "champs=" + jsonCommande);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modifier une commande d'un livre ou dvd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool ModifierCommandeDocument(CommandeDocument commande)
        {
            String jsonCommande = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());
            Console.WriteLine(jsonCommande);
            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(PUT, "commande", "champs=" + jsonCommande);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprimer une commande d'un livre ou dvd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool SupprimerCommandeDocument(CommandeDocument commande)
        {
            String jsonId = convertToJson("id", commande.Id);
            try
            {
                List<Livre> liste = TraitementRecup<Livre>(DELETE, "commande/" + jsonId, null);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Get toutes les étapes de suivi pour les commandes
        /// </summary>
        /// <returns></returns>
        public List<Suivi> GetAllSuivi()
        {
            List<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi", null);
            return lesSuivis;
        }

        /// <summary>
        /// Get toutes les etats d'exemplaires
        /// </summary>
        /// <returns></returns>
        public List<Etat> GetAllEtats()
        {
            List<Etat> lesEtats = TraitementRecup<Etat>(GET, "etat", null);
            return lesEtats;
        }

        /// <summary>
        /// Retourne les exemplaires d'un document
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// Modifie un exemplaire dans la BDD
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns></returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            Console.WriteLine(jsonExemplaire);
            try
            {
                List<Livre> liste = TraitementRecup<Livre>(PUT, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprime un exemplaire dans la BDD
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns></returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(DELETE, "exemplaire/" + jsonExemplaire, null);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ajouter un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool AjouterLivre(Livre livre)
        {
            String jsonLivre = JsonConvert.SerializeObject(livre, new CustomDateTimeConverter());
            Console.WriteLine(jsonLivre);
            try
            {
                List<Livre> liste = TraitementRecup<Livre>(POST, "livre", "champs=" + jsonLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modifier un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool ModifierLivre(Livre livre)
        {
            String jsonLivre = JsonConvert.SerializeObject(livre, new CustomDateTimeConverter());
            Console.WriteLine(jsonLivre);
            try
            {
                List<Livre> liste = TraitementRecup<Livre>(PUT, "livre", "champs=" + jsonLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprimer un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool SupprimerLivre(Livre livre)
        {
            String jsonLivre = JsonConvert.SerializeObject(livre, new CustomDateTimeConverter());
            try
            {
                List<Livre> liste = TraitementRecup<Livre>(DELETE, "livre", "champs=" + jsonLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ajouter un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool AjouterDvd(Dvd dvd)
        {
            String jsonDvd = JsonConvert.SerializeObject(dvd, new CustomDateTimeConverter());
            Console.WriteLine(jsonDvd);
            try
            {
                List<Dvd> liste = TraitementRecup<Dvd>(POST, "dvd", "champs=" + jsonDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modifier un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool ModifierDvd(Dvd dvd)
        {
            String jsonDvd = JsonConvert.SerializeObject(dvd, new CustomDateTimeConverter());
            Console.WriteLine(jsonDvd);
            try
            {
                List<Dvd> liste = TraitementRecup<Dvd>(PUT, "dvd", "champs=" + jsonDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprimer un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            String jsonDvd = JsonConvert.SerializeObject(dvd, new CustomDateTimeConverter());
            Console.WriteLine(jsonDvd);
            try
            {
                List<Dvd> liste = TraitementRecup<Dvd>(DELETE, "dvd", "champs=" + jsonDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ajouter une revue dans la BDD
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool AjouterRevue(Revue revue)
        {
            String jsonRevue = JsonConvert.SerializeObject(revue, new CustomDateTimeConverter());
            Console.WriteLine(jsonRevue);
            try
            {
                List<Revue> liste = TraitementRecup<Revue>(POST, "revue", "champs=" + jsonRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modifier une revue dans la BDD
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool ModifierRevue(Revue revue)
        {
            String jsonRevue = JsonConvert.SerializeObject(revue, new CustomDateTimeConverter());
            Console.WriteLine(jsonRevue);
            try
            {
                List<Revue> liste = TraitementRecup<Revue>(PUT, "revue", "champs=" + jsonRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprimer une revue dans la BDD
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool SupprimerRevue(Revue revue)
        {
            String jsonRevue = JsonConvert.SerializeObject(revue, new CustomDateTimeConverter());
            Console.WriteLine(jsonRevue);
            try
            {
                List<Revue> liste = TraitementRecup<Revue>(DELETE, "revue", "champs=" + jsonRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Retourne tous les abonnements revues
        /// </summary>
        /// <returns></returns>
        public List<Abonnement> GetAllAbonnements()
        {
            List<Abonnement> liste = TraitementRecup<Abonnement>(GET, "abonnements", null);
            return liste;
        }
        /// <summary>
        /// Retourne tous les abonnements d'une revue spécifique
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Abonnement> GetAbonnementsRevue(string id)
        {
            String jsonId = convertToJson("id", id);
            List<Abonnement> liste = TraitementRecup<Abonnement>(GET, "abonnements/" + jsonId, null);
            return liste;
        }
        /// <summary>
        /// Retourne les abonnements dont la date de fin est dans moins de 30 jours
        /// </summary>
        /// <returns></returns>
        public List<Abonnement> GetAbonnementsAvecFinProche()
        {
            String jsonFin = convertToJson("fin", "1");
            List<Abonnement> liste = TraitementRecup<Abonnement>(GET, "abonnements/" + jsonFin, null);
            return liste;
        }
        /// <summary>
        /// Retourne une revue spécifiée par son id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Revue GetRevue(string id)
        {
            String jsonId = convertToJson("id", id);
            List<Revue> liste = TraitementRecup<Revue>(GET, "revue/" + jsonId, null);
            if (liste.Count > 0)
            {
                return liste[0];
            }
            return null;
        }
        /// <summary>
        /// Enregistre un abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            String jsonAbonnement = JsonConvert.SerializeObject(abonnement, new CustomDateTimeConverter());
            Console.WriteLine(jsonAbonnement);
            try
            {
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement", "champs=" + jsonAbonnement);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// Demande la suppression d'un abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            String jsonId = convertToJson("id", abonnement.Id);
            try
            {
                List<Abonnement> liste = TraitementRecup<Abonnement>(DELETE, "abonnement/" + jsonId, null);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message, String parametres)
        {
            Console.WriteLine("REQUETE : " + methode + ", " + message + ", " + parametres);
            // trans
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
                Console.WriteLine("Erreur complète: " + e.ToString());
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}
