using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gherkin.Ast;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgSuivis = new BindingSource();
        private readonly BindingSource bdgEtats = new BindingSource();
        private readonly Utilisateur utilisateur;
        private const int ADMIN = 1;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        public FrmMediatek(Utilisateur utilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.utilisateur = utilisateur;
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        
        /// <summary>
        /// Remplir un combo avec les étapes de suivi de commande
        /// </summary>
        /// <param name="lesSuivis"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        private void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        
        /// <summary>
        /// Remplir un combo avec les etats d'exemplaires
        /// </summary>
        /// <param name="lesEtats"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        private void RemplirComboEtat(List<Etat> lesEtats, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesEtats;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Fonction commune pour prendre le chemin d'un image pour l'associer avec un document
        /// </summary>
        /// <param name="txbChemin"></param>
        /// <param name="image"></param>
        private void parcourirImage(TextBox txbChemin, PictureBox image)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbChemin.Text = filePath;
            try
            {
                image.Image = Image.FromFile(filePath);
            }
            catch
            {
                image.Image = null;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        private readonly BindingSource bdgExemplairesLivresListe = new BindingSource();
        private List<Exemplaire> lesExemplairesLivres = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxLivresExemplairesEtat);
            RemplirLivresListeComplete();
            btnLivresExemplairesEnregistrer.Enabled = false;
            btnLivresExemplairesSupprimer.Enabled = false;
            cbxLivresExemplairesEtat.Enabled = false;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Clic sur les titres de de colonne de la liste d'exemplaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresExemplaires_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvLivresExemplaires.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "LibelleEtat":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.LibelleEtat).ToList();
                    break;
            }
            RemplirExemplairesLivresListe(sortedList);
        }

        /// <summary>
        /// Changement de sélection dans la liste d'exemplaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresExemplaires_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresExemplaires.SelectedRows.Count > 0)
            {
                cbxLivresExemplairesEtat.SelectedIndex = int.Parse((bdgExemplairesLivresListe.Current as Exemplaire).IdEtat) - 1;
                btnLivresExemplairesEnregistrer.Enabled = true;
                btnLivresExemplairesSupprimer.Enabled = true;
                cbxLivresExemplairesEtat.Enabled = true;
            } else
            {
                cbxLivresExemplairesEtat.SelectedIndex = -1;
                btnLivresExemplairesEnregistrer.Enabled = false;
                btnLivresExemplairesSupprimer.Enabled = false;
                cbxLivresExemplairesEtat.Enabled = false;
            }
        }

        /// <summary>
        /// Clic sur le bouton pour enregistrer des modifications à un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresExemplairesEnregistrer_Click(object sender, EventArgs e)
        {
            if (cbxLivresExemplairesEtat.SelectedIndex != -1)
            {
                try
                {
                    Exemplaire exemplaire = bdgExemplairesLivresListe.Current as Exemplaire;
                    string nouveauEtat = (cbxLivresExemplairesEtat.SelectedIndex + 1).ToString("D5");
                    exemplaire.IdEtat = nouveauEtat;
                    controller.ModifierExemplaire(exemplaire);
                    RemplirExemplairesLivresListe(controller.GetExemplairesDocument(exemplaire.Id));
                } catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }

        /// <summary>
        /// Clic sur le bouton pour supprimer un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresExemplairesSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvLivresExemplaires.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir un exemplaire", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Exemplaire exemplaire = bdgExemplairesLivresListe.Current as Exemplaire;
                DialogResult result = MessageBox.Show(
                    "Supprimer exemplaire " + exemplaire.Numero,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerExemplaire(exemplaire);
                        RemplirExemplairesLivresListe(controller.GetExemplairesDocument(exemplaire.Id));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Remplir la liste d'exemplaires
        /// </summary>
        /// <param name="exemplaires"></param>
        private void RemplirExemplairesLivresListe(List<Exemplaire> exemplaires)
        {
            lesExemplairesLivres = exemplaires;
            bdgExemplairesLivresListe.DataSource = exemplaires;
            dgvLivresExemplaires.DataSource = bdgExemplairesLivresListe;
            dgvLivresExemplaires.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresExemplaires.Columns["idEtat"].Visible = false;
            dgvLivresExemplaires.Columns["id"].Visible = false;
            dgvLivresExemplaires.Columns["photo"].Visible = false;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);

                    RemplirExemplairesLivresListe(controller.GetExemplairesDocument(livre.Id));
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        /// <summary>
        /// Activer ou désactiver l'ajout ou modification de Livres. Désactive la zone de recherche.
        /// </summary>
        /// <param name="active">true pour activer</param>
        public void ModeAjoutModifLivre(bool active, string mode = "")
        {
            // zone de modification
            if (mode == "ajout")
            {
                btnLivresEnregistrer.Text = "Ajouter";
            }
            else if (mode == "modif")
            {
                btnLivresEnregistrer.Text = "Modifier";
            }
            else
            {
                btnLivresEnregistrer.Text = "Enregistrer";
            }

            grpLivresInfos.Enabled = active;
            foreach (Control c in grpLivresInfos.Controls)
            {
                if (c is TextBox textBox)
                {
                    textBox.ReadOnly = false;
                }
            }
            btnLivresAnnuler.Visible = active;
            btnLivresEnregistrer.Visible = active;
            txbLivresNumero.ReadOnly = true; // numéro jamais modifié
            if (active)
            {
                txbLivresTitre.Focus();
            }

            // zone de recherche
            grpLivresRecherche.Enabled = !(active);
        }


        /// <summary>
        /// Clic sur bouton Ajouter dans partie Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresAjouter_Click(object sender, EventArgs e)
        {
            VideLivresInfos();
            ModeAjoutModifLivre(true, "ajout");
        }

        /// <summary>
        /// Clic sur bouton Modifier dans partie Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresModifier_Click(object sender, EventArgs e)
        {
            // vérfier sélection d'un DVD dans liste
            if (dgvLivresListe.SelectedRows.Count == 1)
            {
                ModeAjoutModifLivre(true, "modif");
            }
        }

        /// <summary>
        /// Clic sur bouton Enregistrer dans partie Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresEnregistrer_Click(object sender, EventArgs e)
        {
            string mode = btnLivresEnregistrer.Text;
            if (mode == "Ajouter" || mode == "Modifier")
            {
                string validationMessage = "";
                Genre genre = bdgGenres.Cast<Genre>()
                    .FirstOrDefault(g => g.Libelle.Equals(txbLivresGenre.Text));
                if (genre is null)
                {
                    validationMessage += "Saisir un genre valid. ";
                }
                Public lePublic = bdgPublics.Cast<Public>()
                    .FirstOrDefault(p => p.Libelle.Equals(txbLivresPublic.Text));
                if (lePublic is null)
                {
                    validationMessage += "Saisir un public valid. ";
                }
                Rayon rayon = bdgRayons.Cast<Rayon>()
                    .FirstOrDefault(r => r.Libelle.Equals(txbLivresRayon.Text));
                if (rayon is null)
                {
                    validationMessage += "Saisir un rayon valid. ";
                }
                
                
                if (validationMessage != "")
                {
                    MessageBox.Show(
                        validationMessage,
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    validationMessage = "";

                } else
                {
                    try
                    {
                        if (mode == "Ajouter")
                        {
                            Livre livre = new Livre(
                                id: null,
                                titre: txbLivresTitre.Text,
                                image: txbLivresImage.Text,
                                isbn: txbLivresIsbn.Text,
                                auteur: txbLivresAuteur.Text,
                                collection: txbLivresCollection.Text,
                                idGenre: genre.Id,
                                genre: genre.Libelle,
                                idPublic: lePublic.Id,
                                lePublic: lePublic.Libelle,
                                idRayon: rayon.Id,
                                rayon: rayon.Libelle
                            );
                            this.controller.AjouterLivre(livre);
                        }
                        else
                        {
                            Livre livre = new Livre(
                            id: ((Livre)bdgLivresListe.Current).Id,
                            titre: txbLivresTitre.Text,
                            image: txbLivresImage.Text,
                            isbn: txbLivresIsbn.Text,
                            auteur: txbLivresAuteur.Text,
                            collection: txbLivresCollection.Text,
                            idGenre: genre.Id,
                            genre: genre.Libelle,
                            idPublic: lePublic.Id,
                            lePublic: lePublic.Libelle,
                            idRayon: rayon.Id,
                            rayon: rayon.Libelle
                            );
                            this.controller.ModifierLivre(livre);
                        }
                        ModeAjoutModifLivre(false);
                        TabLivres_Enter(null, null);
                    } catch(Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    
                }
            }                
        }

        /// <summary>
        /// Clic sur bouton Annuler dans partie Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresAnnuler_Click(object sender, EventArgs e)
        {
            VideLivresInfos(); // Vider si jamais il n'y a rien dans DGV
            ModeAjoutModifLivre(false);
        }

        /// <summary>
        /// Clic sur bouton Supprimer dans partie Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir un livre", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else
            {
                Livre livre = bdgLivresListe.Current as Livre;
                DialogResult result = MessageBox.Show(
                    "Supprimer livre " + livre.Titre,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerLivre(livre);
                        TabLivres_Enter(null, null);
                    } catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Clic sur bouton parcourir dans Livres, pour chercher une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresParcourir_Click(object sender, EventArgs e)
        {
            parcourirImage(txbLivresImage, pcbLivresImage);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        private readonly BindingSource bdgExemplairesDvdListe = new BindingSource();
        private List<Exemplaire> lesExemplairesDvd = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxDvdExemplairesEtat);
            RemplirDvdListeComplete();
            btnDvdExemplairesEnregistrer.Enabled = false;
            btnDvdExemplairesSupprimer.Enabled = false;
            cbxDvdExemplairesEtat.Enabled = false;
        }

        /// <summary>
        /// Clic sur les titres de de colonne de la liste d'exemplaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplaires_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvDvdExemplaires.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "LibelleEtat":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.LibelleEtat).ToList();
                    break;
            }
            RemplirExemplairesDvdListe(sortedList);
        }

        /// <summary>
        /// Changement de sélection dans la liste d'exemplaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplaires_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdExemplaires.SelectedRows.Count > 0)
            {
                cbxDvdExemplairesEtat.SelectedIndex = int.Parse((bdgExemplairesDvdListe.Current as Exemplaire).IdEtat) - 1;
                btnDvdExemplairesEnregistrer.Enabled = true;
                btnDvdExemplairesSupprimer.Enabled = true;
                cbxDvdExemplairesEtat.Enabled = true;
            }
            else
            {
                cbxDvdExemplairesEtat.SelectedIndex = -1;
                btnDvdExemplairesEnregistrer.Enabled = false;
                btnDvdExemplairesSupprimer.Enabled = false;
                cbxDvdExemplairesEtat.Enabled = false;
            }
        }

        /// <summary>
        /// Clic sur le bouton pour enregistrer des modifications à un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdExemplairesEnregistrer_Click(object sender, EventArgs e)
        {
            if (cbxDvdExemplairesEtat.SelectedIndex != -1)
            {
                try
                {
                    Exemplaire exemplaire = bdgExemplairesDvdListe.Current as Exemplaire;
                    string nouveauEtat = (cbxDvdExemplairesEtat.SelectedIndex + 1).ToString("D5");
                    exemplaire.IdEtat = nouveauEtat;
                    controller.ModifierExemplaire(exemplaire);
                    RemplirExemplairesDvdListe(controller.GetExemplairesDocument(exemplaire.Id));
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }

        /// <summary>
        /// Clic sur le bouton pour supprimer un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdExemplairesSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvDvdListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir un exemplaire", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Exemplaire exemplaire = bdgExemplairesDvdListe.Current as Exemplaire;
                DialogResult result = MessageBox.Show(
                    "Supprimer exemplaire " + exemplaire.Numero,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerExemplaire(exemplaire);
                        RemplirExemplairesDvdListe(controller.GetExemplairesDocument(exemplaire.Id));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Remplir la liste d'exemplaires
        /// </summary>
        /// <param name="exemplaires"></param>
        private void RemplirExemplairesDvdListe(List<Exemplaire> exemplaires)
        {
            lesExemplairesDvd = exemplaires;
            bdgExemplairesDvdListe.DataSource = exemplaires;
            dgvDvdExemplaires.DataSource = bdgExemplairesDvdListe;
            dgvDvdExemplaires.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdExemplaires.Columns["idEtat"].Visible = false;
            dgvDvdExemplaires.Columns["id"].Visible = false;
            dgvDvdExemplaires.Columns["photo"].Visible = false;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);

                    RemplirExemplairesDvdListe(controller.GetExemplairesDocument(dvd.Id));
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        /// <summary>
        /// Activer ou désactiver l'ajout ou modification de DVD. Désactive la zone de recherche.
        /// </summary>
        /// <param name="active">true pour activer</param>
        public void ModeAjoutModifDvd(bool active, string mode = "")
        {
            // zone de modification
            if (mode == "ajout")
            {
                btnDvdEnregistrer.Text = "Ajouter";
            }
            else if (mode == "modif")
            {
                btnDvdEnregistrer.Text = "Modifier";
            }
            else
            {
                btnDvdEnregistrer.Text = "Enregistrer";
            }

            grpDvdInfos.Enabled = active;
            foreach (Control c in grpDvdInfos.Controls)
            {
                if (c is TextBox textBox)
                {
                    textBox.ReadOnly = false;
                }
            }
            btnDvdAnnuler.Visible = active;
            btnDvdEnregistrer.Visible = active;
            txbDvdNumero.ReadOnly = true; // numéro jamais modifié
            if (active)
            {
                txbDvdTitre.Focus();
            }

            // zone de recherche
            grpDvdRecherche.Enabled = !(active);

        }

        /// <summary>
        /// Clic sur bouton Ajouter dans partie DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdAjouter_Click(object sender, EventArgs e)
        {
            VideDvdInfos();
            ModeAjoutModifDvd(true, "ajout");
        }

        /// <summary>
        /// Clic sur bouton Modifier dans partie DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdModifier_Click(object sender, EventArgs e)
        {
            // vérfier sélection d'un DVD dans liste
            if (dgvDvdListe.SelectedRows.Count == 1)
            {
                ModeAjoutModifDvd(true, "modif");
            }

            
        }

        /// <summary>
        /// Clic sur bouton Enregistrer dans partie DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdEnregistrer_Click(object sender, EventArgs e)
        {
            string mode = btnDvdEnregistrer.Text;
            if (mode == "Ajouter" || mode == "Modifier")
            {
                string validationMessage = "";
                Genre genre = bdgGenres.Cast<Genre>()
                    .FirstOrDefault(g => g.Libelle.Equals(txbDvdGenre.Text));
                if (genre is null)
                {
                    validationMessage += "Saisir un genre valid. ";
                }
                Public lePublic = bdgPublics.Cast<Public>()
                    .FirstOrDefault(p => p.Libelle.Equals(txbDvdPublic.Text));
                if (lePublic is null)
                {
                    validationMessage += "Saisir un public valid. ";
                }
                Rayon rayon = bdgRayons.Cast<Rayon>()
                    .FirstOrDefault(r => r.Libelle.Equals(txbDvdRayon.Text));
                if (rayon is null)
                {
                    validationMessage += "Saisir un rayon valid. ";
                }

                int? duree = null;
                if (int.TryParse(txbDvdDuree.Text, out int result))
                {
                    duree = result;
                }

                if (validationMessage != "")
                {
                    MessageBox.Show(
                        validationMessage,
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    validationMessage = "";

                }
                else
                {
                    try
                    {
                        
                        if (mode == "Ajouter")
                        {
                            Dvd dvd = new Dvd(
                                id: null,
                                titre: txbDvdTitre.Text,
                                image: txbDvdImage.Text,
                                duree: duree ?? 0,
                                realisateur: txbDvdRealisateur.Text,
                                synopsis: txbDvdSynopsis.Text,
                                idGenre: genre.Id,
                                genre: genre.Libelle,
                                idPublic: lePublic.Id,
                                lePublic: lePublic.Libelle,
                                idRayon: rayon.Id,
                                rayon: rayon.Libelle
                            );
                            this.controller.AjouterDvd(dvd);
                        }
                        else
                        {
                            Dvd dvd = new Dvd(
                                id: ((Dvd)bdgDvdListe.Current).Id,
                                titre: txbDvdTitre.Text,
                                image: txbDvdImage.Text,
                                duree: duree ?? 0,
                                realisateur: txbDvdRealisateur.Text,
                                synopsis: txbDvdSynopsis.Text,
                                idGenre: genre.Id,
                                genre: genre.Libelle,
                                idPublic: lePublic.Id,
                                lePublic: lePublic.Libelle,
                                idRayon: rayon.Id,
                                rayon: rayon.Libelle
                            );
                            this.controller.ModifierDvd(dvd);
                        }
                        ModeAjoutModifDvd(false);
                        tabDvd_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                }
            }
        }

        /// <summary>
        /// Clic sur bouton Annuler dans partie DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdAnnuler_Click(object sender, EventArgs e)
        {
            VideDvdInfos(); // Vider si jamais il n'y a rien dans DGV
            ModeAjoutModifDvd(false);
        }

        /// <summary>
        /// Clic sur bouton Supprimer dans partie DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvDvdListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir un dvd", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Dvd dvd = bdgDvdListe.Current as Dvd;
                DialogResult result = MessageBox.Show(
                    "Supprimer dvd " + dvd.Titre,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerDvd(dvd);
                        tabDvd_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Clic sur bouton parcourir dans Dvd, pour chercher une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdParcourir_Click(object sender, EventArgs e)
        {
            parcourirImage(txbDvdImage, pcbDvdImage);
        }

        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        /// <summary>
        /// Activer ou désactiver l'ajout ou modification de Revue. Désactive la zone de recherche.
        /// </summary>
        /// <param name="active">true pour activer</param>
        public void ModeAjoutModifRevue(bool active, string mode = "")
        {
            // zone de modification
            if (mode == "ajout")
            {
                btnRevuesEnregistrer.Text = "Ajouter";
            }
            else if (mode == "modif")
            {
                btnRevuesEnregistrer.Text = "Modifier";
            }
            else
            {
                btnRevuesEnregistrer.Text = "Enregistrer";
            }

            grpRevuesInfos.Enabled = active;
            foreach (Control c in grpRevuesInfos.Controls)
            {
                if (c is TextBox textBox)
                {
                    textBox.ReadOnly = false;
                }
            }
            btnRevuesAnnuler.Visible = active;
            btnRevuesEnregistrer.Visible = active;
            txbRevuesNumero.ReadOnly = true; // numéro jamais modifié
            if (active)
            {
                txbRevuesTitre.Focus();
            }

            // zone de recherche
            grpRevuesRecherche.Enabled = !(active);

        }

        /// <summary>
        /// Clic sur bouton Ajouter dans partie Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAjouter_Click(object sender, EventArgs e)
        {
            VideRevuesInfos();
            ModeAjoutModifRevue(true, "ajout");
        }

        /// <summary>
        /// Clic sur bouton Modifier dans partie Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesModifier_Click(object sender, EventArgs e)
        {
            // vérfier sélection d'une Revue dans liste
            if (dgvRevuesListe.SelectedRows.Count == 1)
            {
                ModeAjoutModifRevue(true, "modif");
            }
        }

        /// <summary>
        /// Clic sur bouton Enregistrer dans partie Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesEnregistrer_Click(object sender, EventArgs e)
        {
            string mode = btnRevuesEnregistrer.Text;
            if (mode == "Ajouter" || mode == "Modifier")
            {
                string validationMessage = "";
                Genre genre = bdgGenres.Cast<Genre>()
                    .FirstOrDefault(g => g.Libelle.Equals(txbRevuesGenre.Text));
                if (genre is null)
                {
                    validationMessage += "Saisir un genre valid. ";
                }
                Public lePublic = bdgPublics.Cast<Public>()
                    .FirstOrDefault(p => p.Libelle.Equals(txbRevuesPublic.Text));
                if (lePublic is null)
                {
                    validationMessage += "Saisir un public valid. ";
                }
                Rayon rayon = bdgRayons.Cast<Rayon>()
                    .FirstOrDefault(r => r.Libelle.Equals(txbRevuesRayon.Text));
                if (rayon is null)
                {
                    validationMessage += "Saisir un rayon valid. ";
                }

                int? delaiDispo = null;
                if (int.TryParse(txbRevuesDateMiseADispo.Text, out int result))
                {
                    delaiDispo = result;
                }

                if (validationMessage != "")
                {
                    MessageBox.Show(
                        validationMessage,
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    validationMessage = "";

                }
                else
                {
                    try
                    {
                        if (mode == "Ajouter")
                        {
                            Revue revue = new Revue(
                                id: null,
                                titre: txbRevuesTitre.Text,
                                image: txbRevuesImage.Text,
                                periodicite: txbRevuesPeriodicite.Text,
                                delaiMiseADispo: delaiDispo ?? 0,
                                idGenre: genre.Id,
                                genre: genre.Libelle,
                                idPublic: lePublic.Id,
                                lePublic: lePublic.Libelle,
                                idRayon: rayon.Id,
                                rayon: rayon.Libelle
                            );
                            this.controller.AjouterRevue(revue);
                        }
                        else
                        {
                            Revue revue = new Revue(
                            id: ((Revue)bdgRevuesListe.Current).Id,
                            titre: txbRevuesTitre.Text,
                            image: txbRevuesImage.Text,
                            periodicite: txbRevuesPeriodicite.Text,
                            delaiMiseADispo: delaiDispo ?? 0,
                            idGenre: genre.Id,
                            genre: genre.Libelle,
                            idPublic: lePublic.Id,
                            lePublic: lePublic.Libelle,
                            idRayon: rayon.Id,
                            rayon: rayon.Libelle
                            );
                            this.controller.ModifierRevue(revue);
                        }
                        ModeAjoutModifRevue(false);
                        tabRevues_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                }
            }
        }

        /// <summary>
        /// Clic sur bouton Annuler dans partie Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnuler_Click(object sender, EventArgs e)
        {
            VideRevuesInfos(); // Vider si jamais il n'y a rien dans DGV
            ModeAjoutModifRevue(false);
        }

        private void btnRevuesSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir une revue", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Revue revue = bdgRevuesListe.Current as Revue;
                DialogResult result = MessageBox.Show(
                    "Supprimer revue " + revue.Titre,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerRevue(revue);
                        tabRevues_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Clic sur bouton parcourir dans Revues, pour chercher une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesParcourir_Click(object sender, EventArgs e)
        {
            parcourirImage(txbRevuesImage, pcbRevuesImage);
        }
        #endregion

        #region Onglet Parutions
        private readonly BindingSource bdgRevuesExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplairesRevues = new List<Exemplaire>();

        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxReceptionExemplairesEtat);
            btnReceptionExemplairesEnregistrer.Enabled = false;
            btnReceptionExemplairesSupprimer.Enabled = false;
            cbxReceptionExemplairesEtat.Enabled = false;
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                lesExemplairesRevues = exemplaires;
                bdgRevuesExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgRevuesExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["Photo"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgRevuesExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplairesRevues = controller.GetExemplairesDocument(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplairesRevues);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string etat = "neuf";
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, etat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplairesRevues.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplairesRevues.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "LibelleEtat":
                    sortedList = lesExemplairesRevues.OrderBy(o => o.LibelleEtat).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image et l'état de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgRevuesExemplairesListe.List[bdgRevuesExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    cbxReceptionExemplairesEtat.SelectedIndex = int.Parse(exemplaire.IdEtat) - 1;
                    btnReceptionExemplairesEnregistrer.Enabled = true;
                    btnReceptionExemplairesSupprimer.Enabled = true;
                    cbxReceptionExemplairesEtat.Enabled = true;

                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                cbxReceptionExemplairesEtat.SelectedIndex = -1;
                btnReceptionExemplairesEnregistrer.Enabled = false;
                btnReceptionExemplairesSupprimer.Enabled = false;
                cbxReceptionExemplairesEtat.Enabled = false;

                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        /// <summary>
        /// Clic sur le bouton pour enregistrer des modifications à un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplairesEnregistrer_Click(object sender, EventArgs e)
        {
            if (cbxReceptionExemplairesEtat.SelectedIndex != -1)
            {
                try
                {
                    Exemplaire exemplaire = bdgRevuesExemplairesListe.Current as Exemplaire;
                    string nouveauEtat = (cbxReceptionExemplairesEtat.SelectedIndex + 1).ToString("D5");
                    exemplaire.IdEtat = nouveauEtat;
                    controller.ModifierExemplaire(exemplaire);
                    RemplirReceptionExemplairesListe(controller.GetExemplairesDocument(exemplaire.Id));
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }

        /// <summary>
        /// Clic sur le bouton pour supprimer un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplairesSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir un exemplaire", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Exemplaire exemplaire = bdgRevuesExemplairesListe.Current as Exemplaire;
                DialogResult result = MessageBox.Show(
                    "Supprimer exemplaire " + exemplaire.Numero,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerExemplaire(exemplaire);
                        RemplirReceptionExemplairesListe(controller.GetExemplairesDocument(exemplaire.Id));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
        #endregion

        #region OngletCommandesLivres
        private readonly BindingSource bdgCommandesLivreListe = new BindingSource();
        private List<CommandeDocument> lesCommandesLivre = new List<CommandeDocument>();

        /// <summary>
        /// Remplir data grid view à partir de la liste donné en paramètre
        /// </summary>
        /// <param name="commandesDocument"></param>
        private void RemplirCommandesLivreListe(List<CommandeDocument> commandesDocument)
        {
            bdgCommandesLivreListe.DataSource = commandesDocument;
            dgvCommandesLivresListe.DataSource = bdgCommandesLivreListe;
            dgvCommandesLivresListe.Columns["idSuivi"].Visible = false;
            dgvCommandesLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// Evenement d'ouverture de l'onglet Commandes Livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesLivre_Enter(object sender, EventArgs e)
        {
            lesCommandesLivre = controller.GetAllCommandesDocument("livre");
            RemplirComboSuivi(controller.GetAllSuivi(), bdgSuivis, cbxCommandesLivresComSuivi);
            RemplirCommandesLivreListe(lesCommandesLivre);
            ModeModifCommandeLivre(false);
            ModeAjoutCommandeLivre(false);
        }

        /// <summary>
        /// Afficher les informations d'un livre
        /// </summary>
        /// <param name="id">id du livre</param>
        private void AfficheCommandeLivreInfos(string id)
        {
            try
            {
                Livre livre = controller.GetLivre(id);
                txbCommandesLivresInfosAuteur.Text = livre.Auteur;
                txbCommandesLivresInfosCollection.Text = livre.Collection;
                txbCommandesLivresInfosIsbn.Text = livre.Isbn;
                txbCommandesLivresInfosNumDoc.Text = livre.Id;
                txbCommandesLivresInfosGenre.Text = livre.Genre;
                txbCommandesLivresInfosPublic.Text = livre.Public;
                txbCommandesLivresInfosRayon.Text = livre.Rayon;
                txbCommandesLivresInfosTitre.Text = livre.Titre;
            }
            catch
            {
                ViderZoneCommandesLivreInfos();
                MessageBox.Show("Document non trouvé");
            }
        }

        /// <summary>
        /// Vider la zone d'affichage du livre
        /// </summary>
        private void ViderZoneCommandesLivreInfos()
        {
            txbCommandesLivresInfosAuteur.Text = "";
            txbCommandesLivresInfosCollection.Text = "";
            txbCommandesLivresInfosIsbn.Text = "";
            txbCommandesLivresInfosNumDoc.Text = "";
            txbCommandesLivresInfosGenre.Text = "";
            txbCommandesLivresInfosPublic.Text = "";
            txbCommandesLivresInfosRayon.Text = "";
            txbCommandesLivresInfosTitre.Text = "";
        }

        /// <summary>
        /// Evénement de changement de sélection dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            RemplirZoneCommandesLivreCom();
        }

        /// <summary>
        /// Remplir la zone de détail de commande
        /// </summary>
        private void RemplirZoneCommandesLivreCom()
        {
            CommandeDocument commande = bdgCommandesLivreListe.Current as CommandeDocument;
            if (!(commande is null))
            {
                txbCommandesLivresComId.Text = commande.Id;
                dtpCommandesLivresComDate.Value = commande.DateCommande;
                txbCommandesLivresComMontant.Text = commande.Montant.ToString();
                numCommandesLivresComExemplaires.Value = commande.NbExemplaire;
                txbCommandesLivresComNumDoc.Text = commande.IdLivreDvd;
                cbxCommandesLivresComSuivi.SelectedIndex = commande.IdSuivi - 1;
            }
            else
            {
                txbCommandesLivresComId.Text = "";
                dtpCommandesLivresComDate.Value = DateTime.Now;
                txbCommandesLivresComMontant.Text = "";
                numCommandesLivresComExemplaires.Value = 0;
                txbCommandesLivresComNumDoc.Text = "";
                cbxCommandesLivresComSuivi.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Clic sur bouton rechercher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresRechercher_Click(object sender, EventArgs e)
        {
            if (!txbCommandesLivresNumeroDocument.Text.Equals(""))
            {
                AfficheCommandeLivreInfos(txbCommandesLivresNumeroDocument.Text);
                lesCommandesLivre = controller.GetCommandesDocument(txbCommandesLivresNumeroDocument.Text);
                RemplirCommandesLivreListe(lesCommandesLivre);
            }
            else
            {
                lesCommandesLivre = controller.GetAllCommandesDocument("livre");
                RemplirCommandesLivreListe(lesCommandesLivre);
                ViderZoneCommandesLivreInfos();
            }
        }

        /// <summary>
        /// Clic sur les titres de colonne dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesCommandesLivre.OrderBy(o => o.Id).ToList();
                    break;
                case "IdLivreDvd":
                    sortedList = lesCommandesLivre.OrderBy(o => o.IdLivreDvd).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCommandesLivre.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandesLivre.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesLivre.OrderBy(o => o.Montant).ToList();
                    break;
                case "EtSuivi":
                    sortedList = lesCommandesLivre.OrderBy(o => o.IdSuivi).ToList();
                    break;
            }
            RemplirCommandesLivreListe(sortedList);
        }

        /// <summary>
        /// Activer ou désactiver le mode ajout
        /// </summary>
        /// <param name="active"></param>
        private void ModeAjoutCommandeLivre(bool active)
        {
            grpCommandesLivresAjoutModif.Enabled = active;
            grpCommandesLivresRecherche.Enabled = !active;
            if (active)
            {
                foreach (Control ctrl in grpCommandesLivresAjoutModif.Controls)
                {
                    ctrl.Enabled = true;
                }
                btnCommandesLivresEnregistrer.Text = "Ajouter";
                txbCommandesLivresComId.Enabled = false;
                cbxCommandesLivresComSuivi.SelectedIndex = 0;
                cbxCommandesLivresComSuivi.Enabled = false;
                dtpCommandesLivresComDate.Value = DateTime.Now;
                txbCommandesLivresComMontant.Text = "0,00";
                txbCommandesLivresComNumDoc.Text = "0XXXX";
                numCommandesLivresComExemplaires.Value = 1;
            }

        }

        /// <summary>
        /// Ajouter ou désactiver le mode modification
        /// </summary>
        /// <param name="active"></param>
        private void ModeModifCommandeLivre(bool active)
        {
            grpCommandesLivresAjoutModif.Enabled = active;
            grpCommandesLivresRecherche.Enabled = !active;

            if (active)
            {
                btnCommandesLivresEnregistrer.Text = "Modifier";
                foreach (Control ctrl in grpCommandesLivresAjoutModif.Controls)
                {
                    ctrl.Enabled = false;
                }
                cbxCommandesLivresComSuivi.Enabled = active;
                btnCommandesLivresAnnuler.Enabled = active;
                btnCommandesLivresEnregistrer.Enabled = active;
            }
        }

        /// <summary>
        /// Clic sur bouton Ajouter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresAjouter_Click(object sender, EventArgs e)
        {
            ModeAjoutCommandeLivre(true);
        }

        /// <summary>
        /// Clic sur bouton Modifier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresModifier_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListe.SelectedRows.Count > 0)
            {
                ModeModifCommandeLivre(true);
            }
            else
            {
                MessageBox.Show("Veuillez choisir une commande dans la liste.");
            }

        }

        /// <summary>
        /// Clic sur bouton Supprimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir une commande", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                CommandeDocument commande = bdgCommandesLivreListe.Current as CommandeDocument;
                DialogResult result = MessageBox.Show(
                    "Supprimer commande : " + commande.Id,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerCommandeDocument(commande);
                        tabCommandesLivre_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Clic sur bouton Annuler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresAnnuler_Click(object sender, EventArgs e)
        {
            ModeAjoutCommandeLivre(false);
            ModeModifCommandeLivre(false);
        }

        /// <summary>
        /// Clic sur bouton Enregistrer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresEnregistrer_Click(object sender, EventArgs e)
        {
            string mode = btnCommandesLivresEnregistrer.Text;
            if (mode == "Ajouter" || mode == "Modifier")
            {
                string validationMessage = "";
                Console.WriteLine(cbxCommandesLivresComSuivi.Text);
                Suivi suivi = bdgSuivis.Cast<Suivi>()
                    .FirstOrDefault(r => r.Libelle.Equals(cbxCommandesLivresComSuivi.Text));
                if (suivi is null)
                {
                    validationMessage += "Choisir une étape de suivi valide. ";
                }
                string montant = txbCommandesLivresComMontant.Text;
                if (mode == "Ajouter" && !Regex.IsMatch(montant, @"^\d+(,\d{1,2})?$"))
                {
                    validationMessage += "Veuillez saisir un montant dans format 'XXXX,XX' ";
                }
                string numDoc = txbCommandesLivresComNumDoc.Text;
                if (mode == "Ajouter" && !Regex.IsMatch(numDoc, @"^0\d{4}$"))
                {
                    validationMessage += "Veuillez saisir un numéro de document dans format '0XXXX' ";
                }

                if (validationMessage != "")
                {
                    MessageBox.Show(
                        validationMessage,
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    validationMessage = "";

                }
                else
                {
                    try
                    {
                        if (mode == "Ajouter")
                        {
                            CommandeDocument commande = new CommandeDocument(
                                id: null,
                                nbExemplaire: (int)numCommandesLivresComExemplaires.Value,
                                idLivreDvd: txbCommandesLivresComNumDoc.Text,
                                idSuivi: suivi.Id,
                                suivi: suivi.Libelle,
                                dateCommande: dtpCommandesLivresComDate.Value,
                                montant: Double.Parse(txbCommandesLivresComMontant.Text)
                            );
                            this.controller.AjouterCommandeDocument(commande);
                        }
                        else
                        {
                            CommandeDocument commande = new CommandeDocument(
                                id: txbCommandesLivresComId.Text,
                                nbExemplaire: (int)numCommandesLivresComExemplaires.Value,
                                idLivreDvd: txbCommandesLivresComNumDoc.Text,
                                idSuivi: suivi.Id,
                                suivi: suivi.Libelle,
                                dateCommande: dtpCommandesLivresComDate.Value,
                                montant: Double.Parse(txbCommandesLivresComMontant.Text)
                            );
                            this.controller.ModifierCommandeDocument(commande);
                        }
                        ModeAjoutCommandeLivre(false);
                        ModeModifCommandeLivre(false);
                        tabCommandesLivre_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                }
            }
        }

        #endregion


        #region OnlgetCommandesDvd

        private readonly BindingSource bdgCommandesDvdListe = new BindingSource();
        private List<CommandeDocument> lesCommandesDvd = new List<CommandeDocument>();

        /// <summary>
        /// Remplir la data grid view des commandes
        /// </summary>
        /// <param name="commandesDocument"></param>
        private void RemplirCommandesDvdListe(List<CommandeDocument> commandesDocument)
        {
            bdgCommandesDvdListe.DataSource = commandesDocument;
            dgvCommandesDvdListe.DataSource = bdgCommandesDvdListe;
            dgvCommandesDvdListe.Columns["idSuivi"].Visible = false;
            dgvCommandesDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// Ouverture de l'onglet Commandes Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesCommandesDvd = controller.GetAllCommandesDocument("dvd");
            RemplirComboSuivi(controller.GetAllSuivi(), bdgSuivis, cbxCommandesDvdComSuivi);
            RemplirCommandesDvdListe(lesCommandesDvd);
            ModeModifCommandeDvd(false);
            ModeAjoutCommandeDvd(false);
        }

        /// <summary>
        /// Afficher les détails d'un DVD
        /// </summary>
        /// <param name="id"></param>
        private void AfficheCommandeDvdInfos(string id)
        {
            try
            {
                Dvd dvd = controller.GetDvd(id);
                txbCommandesDvdInfosRealisateur.Text = dvd.Realisateur;
                txbCommandesDvdInfosSynopsis.Text = dvd.Synopsis;
                txbCommandesDvdInfosDuree.Text = dvd.Duree.ToString();
                txbCommandesDvdInfosNumDoc.Text = dvd.Id;
                txbCommandesDvdInfosGenre.Text = dvd.Genre;
                txbCommandesDvdInfosPublic.Text = dvd.Public;
                txbCommandesDvdInfosRayon.Text = dvd.Rayon;
                txbCommandesDvdInfosTitre.Text = dvd.Titre;
            }
            catch
            {
                ViderZoneCommandesDvdInfos();
                MessageBox.Show("Document non trouvé");
            }
        }

        /// <summary>
        /// Vider les détails de DVD
        /// </summary>
        private void ViderZoneCommandesDvdInfos()
        {
            txbCommandesDvdInfosRealisateur.Text = "";
            txbCommandesDvdInfosSynopsis.Text = "";
            txbCommandesDvdInfosDuree.Text = "";
            txbCommandesDvdInfosNumDoc.Text = "";
            txbCommandesDvdInfosGenre.Text = ""; ;
            txbCommandesDvdInfosPublic.Text = "";
            txbCommandesDvdInfosRayon.Text = "";
            txbCommandesDvdInfosTitre.Text = "";
        }

        /// <summary>
        /// Remplir les détails d'une commande DVD
        /// </summary>
        private void RemplirZoneCommandesDvdCom()
        {
            CommandeDocument commande = bdgCommandesDvdListe.Current as CommandeDocument;
            if (!(commande is null))
            {
                txbCommandesDvdComId.Text = commande.Id;
                dtpCommandesDvdComDate.Value = commande.DateCommande;
                txbCommandesDvdComMontant.Text = commande.Montant.ToString();
                numCommandesDvdComNbExemplaire.Value = commande.NbExemplaire;
                txbCommandesDvdComNumDoc.Text = commande.IdLivreDvd;
                cbxCommandesDvdComSuivi.SelectedIndex = commande.IdSuivi - 1;
            }
            else
            {
                txbCommandesDvdComId.Text = "";
                dtpCommandesDvdComDate.Value = DateTime.Now;
                txbCommandesDvdComMontant.Text = "";
                numCommandesDvdComNbExemplaire.Value = 0;
                txbCommandesDvdComNumDoc.Text = "";
                cbxCommandesDvdComSuivi.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Activer ou désactiver le mode ajout
        /// </summary>
        /// <param name="active"></param>
        private void ModeAjoutCommandeDvd(bool active)
        {
            grpCommandesDvdCom.Enabled = active;
            grpCommandesDvdRecherche.Enabled = !active;
            if (active)
            {
                foreach (Control ctrl in grpCommandesDvdCom.Controls)
                {
                    ctrl.Enabled = true;
                }
                btnCommandesDvdEnregistrer.Text = "Ajouter";
                txbCommandesDvdComId.Enabled = false;
                cbxCommandesDvdComSuivi.SelectedIndex = 0;
                cbxCommandesDvdComSuivi.Enabled = false;
                dtpCommandesDvdComDate.Value = DateTime.Now;
                txbCommandesDvdComMontant.Text = "0,00";
                txbCommandesDvdComNumDoc.Text = "2XXXX";
                numCommandesDvdComNbExemplaire.Value = 1;
            }

        }

        /// <summary>
        /// Activer ou désactiver le mode modification
        /// </summary>
        /// <param name="active"></param>
        private void ModeModifCommandeDvd(bool active)
        {
            grpCommandesDvdCom.Enabled = active;
            grpCommandesDvdRecherche.Enabled = !active;

            if (active)
            {
                btnCommandesDvdEnregistrer.Text = "Modifier";
                foreach (Control ctrl in grpCommandesDvdCom.Controls)
                {
                    ctrl.Enabled = false;
                }
                cbxCommandesDvdComSuivi.Enabled = active;
                btnCommandesDvdAnnuler.Enabled = active;
                btnCommandesDvdEnregistrer.Enabled = active;
            }
        }

        /// <summary>
        /// Clic sur bouton rechercher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdRechercher_Click(object sender, EventArgs e)
        {
            if (!txbCommandesDvdNumeroDocument.Text.Equals(""))
            {
                AfficheCommandeDvdInfos(txbCommandesDvdNumeroDocument.Text);
                lesCommandesDvd = controller.GetCommandesDocument(txbCommandesDvdNumeroDocument.Text);
                RemplirCommandesDvdListe(lesCommandesDvd);
            }
            else
            {
                lesCommandesDvd = controller.GetAllCommandesDocument("dvd");
                RemplirCommandesDvdListe(lesCommandesDvd);
                ViderZoneCommandesDvdInfos();
            }
        }

        /// <summary>
        /// Clic sur bouton Ajouter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdAjouter_Click(object sender, EventArgs e)
        {
            ModeAjoutCommandeDvd(true);
        }

        /// <summary>
        /// Clic sur bouton Modifier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdModifier_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvdListe.SelectedRows.Count > 0)
            {
                ModeModifCommandeDvd(true);
            }
            else
            {
                MessageBox.Show("Veuillez choisir une commande dans la liste.");
            }
        }

        /// <summary>
        /// Clic sur bouton Supprimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvdListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir une commande", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                CommandeDocument commande = bdgCommandesDvdListe.Current as CommandeDocument;
                DialogResult result = MessageBox.Show(
                    "Supprimer commande : " + commande.Id,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerCommandeDocument(commande);
                        tabCommandesDvd_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Clic sur bouton Annuler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdAnnuler_Click(object sender, EventArgs e)
        {
            ModeAjoutCommandeDvd(false);
            ModeModifCommandeDvd(false);
        }

        /// <summary>
        /// Clic sur bouton Enregistrer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdEnregistrer_Click(object sender, EventArgs e)
        {
            string mode = btnCommandesDvdEnregistrer.Text;
            if (mode == "Ajouter" || mode == "Modifier")
            {
                string validationMessage = "";
                Suivi suivi = bdgSuivis.Cast<Suivi>()
                    .FirstOrDefault(r => r.Libelle.Equals(cbxCommandesDvdComSuivi.Text));
                if (suivi is null)
                {
                    validationMessage += "Choisir une étape de suivi valide. ";
                }
                string montant = txbCommandesDvdComMontant.Text;
                if (mode == "Ajouter" && !Regex.IsMatch(montant, @"^\d+(,\d{1,2})?$"))
                {
                    validationMessage += "Veuillez saisir un montant dans format 'XXXX,XX' ";
                }
                string numDoc = txbCommandesDvdComNumDoc.Text;
                if (mode == "Ajouter" && !Regex.IsMatch(numDoc, @"^2\d{4}$"))
                {
                    validationMessage += "Veuillez saisir un numéro de document dans format '0XXXX' ";
                }

                if (validationMessage != "")
                {
                    MessageBox.Show(
                        validationMessage,
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    validationMessage = "";

                }
                else
                {
                    try
                    {
                        if (mode == "Ajouter")
                        {
                            CommandeDocument commande = new CommandeDocument(
                                id: null,
                                nbExemplaire: (int)numCommandesDvdComNbExemplaire.Value,
                                idLivreDvd: txbCommandesDvdComNumDoc.Text,
                                idSuivi: suivi.Id,
                                suivi: suivi.Libelle,
                                dateCommande: dtpCommandesDvdComDate.Value,
                                montant: Double.Parse(txbCommandesDvdComMontant.Text)
                            );
                            this.controller.AjouterCommandeDocument(commande);
                        }
                        else
                        {
                            CommandeDocument commande = new CommandeDocument(
                                id: txbCommandesDvdComId.Text,
                                nbExemplaire: (int)numCommandesDvdComNbExemplaire.Value,
                                idLivreDvd: txbCommandesDvdComNumDoc.Text,
                                idSuivi: suivi.Id,
                                suivi: suivi.Libelle,
                                dateCommande: dtpCommandesDvdComDate.Value,
                                montant: Double.Parse(txbCommandesDvdComMontant.Text)
                            );
                            this.controller.ModifierCommandeDocument(commande);
                        }
                        ModeAjoutCommandeDvd(false);
                        ModeModifCommandeDvd(false);
                        tabCommandesDvd_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                }
            }
        }

        /// <summary>
        /// Clic sur titre de colonne de la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesCommandesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "IdLivreDvd":
                    sortedList = lesCommandesDvd.OrderBy(o => o.IdLivreDvd).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCommandesDvd.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandesDvd.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDvd.OrderBy(o => o.Montant).ToList();
                    break;
                case "EtSuivi":
                    sortedList = lesCommandesDvd.OrderBy(o => o.IdSuivi).ToList();
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        /// <summary>
        /// Changement de sélection dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            RemplirZoneCommandesDvdCom();
        }


        #endregion

        #region OngletAbonnementsRevues
        private readonly BindingSource bdgAbonnementsRevuesListe = new BindingSource();
        private List<Abonnement> lesAbonnementsRevues = new List<Abonnement>();
        /// <summary>
        /// Remplir la data grid view des abonnements
        /// </summary>
        /// <param name="commandesDocument"></param>
        private void RemplirAbonnementsRevuesListe(List<Abonnement> abonnements)
        {
            bdgAbonnementsRevuesListe.DataSource = abonnements;
            dgvAbonnementsRevuesListe.DataSource = bdgAbonnementsRevuesListe;
            dgvAbonnementsRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        /// <summary>
        /// Ouverture de l'onglet Abonnements Revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAbonnementsRevues_Enter(object sender, EventArgs e)
        {
            lesAbonnementsRevues = controller.GetAllAbonnements();
            RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
            ModeAjoutAbonnementRevue(false);
        }
        /// <summary>
        /// Afficher les détails d'une revue
        /// </summary>
        /// <param name="id"></param>
        private void AfficheAbonnementsRevuesInfos(string id)
        {
            try
            {
                Revue revue = controller.GetRevue(id);
                txbAbonnementsRevuesInfosNumRev.Text = revue.Id;
                txbAbonnementsRevuesInfosDelaiMaD.Text = revue.DelaiMiseADispo.ToString(); ;
                txbAbonnementsRevuesInfosPeriodicite.Text = revue.Periodicite;
                txbAbonnementsRevuesInfosGenre.Text = revue.Genre;
                txbAbonnementsRevuesInfosPublic.Text = revue.Public;
                txbAbonnementsRevuesInfosRayon.Text = revue.Rayon;
                txbAbonnementsRevuesInfosTitre.Text = revue.Titre;
            }
            catch
            {
                ViderZoneAbonnementsRevuesInfos();
                MessageBox.Show("Document non trouvé");
            }
        }
        /// <summary>
        /// Vider les détails de revue
        /// </summary>
        private void ViderZoneAbonnementsRevuesInfos()
        {
            txbAbonnementsRevuesInfosNumRev.Text = "";
            txbAbonnementsRevuesInfosDelaiMaD.Text = "";
            txbAbonnementsRevuesInfosPeriodicite.Text = "";
            txbAbonnementsRevuesInfosGenre.Text = "";
            txbAbonnementsRevuesInfosPublic.Text = "";
            txbAbonnementsRevuesInfosRayon.Text = "";
            txbAbonnementsRevuesInfosTitre.Text = "";
        }
        /// <summary>
        /// Remplir les détails d'un abonnement revue
        /// </summary>
        private void RemplirZoneAbonnementsRevuesAb()
        {
            Abonnement abonnement = bdgAbonnementsRevuesListe.Current as Abonnement;
            if (!(abonnement is null))
            {
                txbAbonnementsRevuesAbId.Text = abonnement.Id;
                dtpAbonnementsRevuesAbDateCommande.Value = abonnement.DateCommande;
                dtpAbonnementsRevuesAbDateFinAbonnement.Value = abonnement.DateFinAbonnement;
                txbAbonnementsRevuesAbNumRev.Text = abonnement.IdRevue;
                txbAbonnementsRevuesAbMontant.Text = abonnement.Montant.ToString();
            }
            else
            {
                txbAbonnementsRevuesAbId.Text = "";
                dtpAbonnementsRevuesAbDateCommande.Value = DateTime.Now;
                dtpAbonnementsRevuesAbDateFinAbonnement.Value = DateTime.Now;
                txbAbonnementsRevuesAbNumRev.Text = "";
                txbAbonnementsRevuesAbMontant.Text = "";
            }
        }
        /// <summary>
        /// Activer ou désactiver le mode ajout
        /// </summary>
        /// <param name="active"></param>
        private void ModeAjoutAbonnementRevue(bool active)
        {
            grpAbonnementsRevuesAb.Enabled = active;
            grpAbonnementsRevuesRecherche.Enabled = !active;
            if (active)
            {
                txbAbonnementsRevuesAbId.Enabled = false;
                dtpAbonnementsRevuesAbDateCommande.Enabled = false;
                dtpAbonnementsRevuesAbDateCommande.Value = DateTime.Now;
                dtpAbonnementsRevuesAbDateFinAbonnement.Value = DateTime.Now;
                txbAbonnementsRevuesAbNumRev.Text = "1XXXX";
                txbAbonnementsRevuesAbMontant.Text = "0,00";
            }
        }
        /// <summary>
        /// Clic sur bouton rechercher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementsRevuesRechercher_Click(object sender, EventArgs e)
        {
            if (!txbAbonnementsRevuesNumRev.Text.Equals(""))
            {
                AfficheAbonnementsRevuesInfos(txbAbonnementsRevuesNumRev.Text);
                lesAbonnementsRevues = controller.GetAbonnementsRevue(txbAbonnementsRevuesNumRev.Text);
                RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
            }
            else
            {
                lesAbonnementsRevues = controller.GetAllAbonnements();
                RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
                ViderZoneAbonnementsRevuesInfos();
            }
        }
        /// <summary>
        /// Clic sur bouton Ajouter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementsRevuesAjouter_Click(object sender, EventArgs e)
        {
            ModeAjoutAbonnementRevue(true);
        }
        /// <summary>
        /// Clic sur bouton Supprimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementsRevuesSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvAbonnementsRevuesListe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez choisir un abonnement", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                

                Abonnement abonnement = bdgAbonnementsRevuesListe.Current as Abonnement;
                List<Exemplaire> exemplaires = controller.GetExemplairesDocument(abonnement.IdRevue);

                foreach (Exemplaire exemplaire in exemplaires)
                {
                    if (ParutionDansAbonnement(exemplaire.DateAchat, abonnement.DateCommande, abonnement.DateFinAbonnement))
                    {
                        MessageBox.Show("Il existe des exemplaires ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                DialogResult result = MessageBox.Show(
                    "Supprimer abonnement : " + abonnement.Id,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        this.controller.SupprimerAbonnement(abonnement);
                        tabAbonnementsRevues_Enter(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
        /// <summary>
        /// Clic sur bouton Annuler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementsRevuesAnnuler_Click(object sender, EventArgs e)
        {
            ModeAjoutAbonnementRevue(false);
        }
        /// <summary>
        /// Clic sur bouton Enregistrer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementsRevuesEnregistrer_Click(object sender, EventArgs e)
        {
            string validationMessage = "";
            string montant = txbAbonnementsRevuesAbMontant.Text;
            if (!Regex.IsMatch(montant, @"^\d+(,\d{1,2})?$"))
            {
                validationMessage += "Veuillez saisir un montant dans format 'XXXX,XX' ";
            }
            string numDoc = txbAbonnementsRevuesAbNumRev.Text;
            if (!Regex.IsMatch(numDoc, @"^1\d{4}$"))
            {
                validationMessage += "Veuillez saisir un numéro de document dans format '1XXXX' ";
            }
            if (dtpAbonnementsRevuesAbDateFinAbonnement.Value <= dtpAbonnementsRevuesAbDateCommande.Value)
            {
                validationMessage += "La fin de l'abonnement doit être postérieure à la date de commande ";
            }

            if (validationMessage != "")
            {
                MessageBox.Show(
                    validationMessage,
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                validationMessage = "";

            }
            else
            {
                try
                {
                    Abonnement abonnement = new Abonnement(
                        id: null,
                        dateCommande: dtpAbonnementsRevuesAbDateCommande.Value,
                        dateFinAbonnement: dtpAbonnementsRevuesAbDateFinAbonnement.Value,
                        idRevue: txbAbonnementsRevuesAbNumRev.Text,
                        montant: Double.Parse(txbAbonnementsRevuesAbMontant.Text)
                    );
                    this.controller.AjouterAbonnement(abonnement);
                    ModeAjoutAbonnementRevue(false);
                    tabAbonnementsRevues_Enter(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.ToString(),
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }

            }
        }
        /// <summary>
        /// Clic sur titre de colonne de la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvAbonnementsRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "DateFinAbonnement":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.DateFinAbonnement).ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.Montant).ToList();
                    break;
                case "IdRevue":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.IdRevue).ToList();
                    break;
            }
            RemplirAbonnementsRevuesListe(sortedList);
        }
        /// <summary>
        /// Changement de sélection dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            RemplirZoneAbonnementsRevuesAb();
        }
        /// <summary>
        /// Vérifie si un la date de parution d'un exemplaire est dans la periode d'abonnement
        /// </summary>
        /// <param name="dateExemplaire"></param>
        /// <param name="dateCommandeAb"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <returns>true si dans abonnement, false sinon</returns>
        public bool ParutionDansAbonnement(DateTime dateExemplaire, DateTime dateCommandeAb, DateTime dateFinAbonnement)
        {
            if (dateExemplaire >= dateCommandeAb && dateExemplaire <= dateFinAbonnement)
            {
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Chargement de la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_Load(object sender, EventArgs e)
        {
            List<Abonnement> abonnements = controller.GetAbonnementsAvecFinProche();
            if (abonnements.Count > 0 && utilisateur.IdService == ADMIN)
            {
                string message = "";
                foreach (Abonnement abonnement in abonnements) {
                    Revue revue = controller.GetRevue(abonnement.IdRevue);
                    message += "L'abonnement à '" + revue.Titre + "' se termine bientôt (" + abonnement.DateFinAbonnement.ToShortDateString() + ").\r\n";
                }
                MessageBox.Show(message, "Alerte");
            }

            if (utilisateur.IdService != ADMIN)
            {
                tabOngletsApplication.TabPages.Remove(tabCommandesLivres);
                tabOngletsApplication.TabPages.Remove(tabCommandesDvd);
                tabOngletsApplication.TabPages.Remove(tabReceptionRevue);
                tabOngletsApplication.TabPages.Remove(tabAbonnementsRevues);

                btnLivresAjouter.Visible = false;
                btnLivresModifier.Visible = false;
                btnLivresSupprimer.Visible = false;
                btnLivresParcourir.Visible = false;
                cbxLivresExemplairesEtat.Visible = false;
                btnLivresExemplairesEnregistrer.Visible = false;
                btnLivresExemplairesSupprimer.Visible = false;
                lblLivresExemplairesEtat.Visible = false;

                btnDvdAjouter.Visible = false;
                btnDvdModifier.Visible = false;
                btnDvdSupprimer.Visible = false;
                btnDvdParcourir.Visible = false;
                cbxDvdExemplairesEtat.Visible = false;
                btnDvdExemplairesEnregistrer.Visible = false;
                btnDvdExemplairesSupprimer.Visible = false;
                lblDvdExemplairesEtat.Visible = false;

                btnRevuesAjouter.Visible = false;
                btnRevuesModifier.Visible = false;
                btnRevuesSupprimer.Visible = false;
                btnRevuesParcourir.Visible = false;
            }
            
        }

        /// <summary>
        /// Fermeture de la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
