using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    public partial class FrmConnexion : Form
    {
        private readonly FrmConnexionController controller;
        private const int ADMIN = 1;
        private const int PRETS = 2;
        private const int CULTURE = 3;
        FrmMediatek form;

        public FrmConnexion()
        {
            InitializeComponent();
            this.controller = new FrmConnexionController();
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            List<Utilisateur> utilisateurs = controller.ConnexionUtilisateur(txbLogin.Text, txbMdp.Text);
            if (utilisateurs.Count > 0)
            {
                if (utilisateurs[0].IdService == CULTURE)
                {
                    lblErreur.Text = "Vous n'êtes pas habilité à utiliser l'application";
                } else
                {
                    lblErreur.Text = "";
                    form = new FrmMediatek(utilisateurs[0]);
                    form.Show();
                    this.Hide();
                }
                    
            } else
            {
                lblErreur.Text = "Identifiants incorrects";
            }
        }
    }
}
