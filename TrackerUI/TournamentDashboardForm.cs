using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentDashboardForm : Form
    {
        List<TournamentModel> tournaments = GlobalConfig.Connection.GetTournaments_All();
        
        public TournamentDashboardForm()
        {
            InitializeComponent();
            WireUpList();
        }

        private void WireUpList()
        {
            selectTournamentDropDown.DataSource = null;
            selectTournamentDropDown.DataSource = tournaments;
            selectTournamentDropDown.DisplayMember = "TournamentName";
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm frm = new CreateTournamentForm();
            frm.OnCreateTournamentFormClose += Frm_OnCreateTournamentFormClose;
            frm.Show();
        }

        private void Frm_OnCreateTournamentFormClose(object sender, DateTime e)
        {
            RefreshForm();
        }

        private void loadTournamentButton_Click(object sender, EventArgs e)
        {
            TournamentViewerForm frm = new TournamentViewerForm((TournamentModel) selectTournamentDropDown.SelectedItem);
            frm.OnTournamentViewerFormClosed += TournamentViewer_OnViewerClose;
            frm.Show();
        }

        private void TournamentViewer_OnViewerClose(object sender, DateTime e)
        {
            RefreshForm();
        }

        private void RefreshForm()
        {
            tournaments = GlobalConfig.Connection.GetTournaments_All();
            WireUpList();
        }
    }
}
