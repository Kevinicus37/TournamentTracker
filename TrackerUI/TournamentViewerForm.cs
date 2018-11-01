using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        List<int> rounds = new List<int>();
        List<MatchupModel> matchups = new List<MatchupModel>();


        public TournamentViewerForm(TournamentModel t)
        {
            InitializeComponent();

            tournament = t;

            LoadFormData();
        }

        private void LoadFormData()
        {
            tournamentName.Text = tournament.TournamentName;
            LoadRounds();
            
        }

        private void LoadMatchup()
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            string[] teamNames = new string[2];
            string[] scores = new string[2];

            
            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (m.Entries[i].TeamCompeting != null)
                {
                    teamNames[i] = m.Entries[i].TeamCompeting.TeamName;
                    scores[i] = m.Entries[i].Score.ToString();
                }
                else
                {
                    teamNames[i] = "Unknown";
                    scores[i] = "";
                }
            }

            if (m.Entries.Count < 2)
            {
                teamNames[1] = "Bye";
                scores[1] = "";
            }
            
            teamOneNameLabel.Text = teamNames[0];
            teamTwoNameLabel.Text = teamNames[1];

            teamOneScoreValue.Text = scores[0];
            teamTwoScoreValue.Text = scores[1];
        }

        private void WireUpLists()
        {
            //roundDropDown.DataSource = null;
            roundDropDown.DataSource = rounds;

            matchupListBox.DataSource = null;
            matchupListBox.DataSource = matchups;
            matchupListBox.DisplayMember = "DisplayName";
        }

        private void LoadRounds()
        {
            rounds = new List<int>();

            for (int i = 1; i <= tournament.Rounds.Count; i++)
            {
                rounds.Add(i);
            }

            WireUpLists();
        }

        private void LoadMatchups()
        {
            int round = (int)roundDropDown.SelectedItem;
            matchups = tournament.Rounds[round - 1];
            WireUpLists();
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roundDropDown.SelectedItem != null)
            {
                LoadMatchups();
            }
            
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (matchupListBox.SelectedItem != null)
            {
                LoadMatchup(); 
            }
        }
    }
}
