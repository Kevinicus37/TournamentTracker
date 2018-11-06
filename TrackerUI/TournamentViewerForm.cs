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
            
            if (m != null)
            {
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
                    teamNames[1] = "<Bye>";
                    scores[1] = "";
                }

                teamOneNameLabel.Text = teamNames[0];
                teamTwoNameLabel.Text = teamNames[1];

                teamOneScoreValue.Text = scores[0];
                teamTwoScoreValue.Text = scores[1]; 
            }
        }

        private void WireUpLists()
        {
            
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
            if (unplayedOnlyCheckBox.Checked)
            {
                matchups = matchups.Where(m => m.Winner == null).ToList();
            }
            WireUpLists();

            DisplayMatchupInfo();
        }

        private void DisplayMatchupInfo()
        {
            bool isVisible = (matchups.Count > 0);
            
                teamOneNameLabel.Visible = isVisible;
                teamOneScoreLabel.Visible = isVisible;
                teamOneScoreValue.Visible = isVisible;
                teamTwoNameLabel.Visible = isVisible;
                teamTwoScoreLabel.Visible = isVisible;
                teamTwoScoreValue.Visible = isVisible;
                vsLabel.Visible = isVisible;
                scoreButton.Visible = isVisible;
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

        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        private string ValidateData()
        {
            string output = "";

            double scoreOne = 0;
            double scoreTwo = 0;

            bool scoreOneValid = double.TryParse(teamOneScoreValue.Text, out scoreOne);
            bool scoreTwoValid = double.TryParse(teamTwoScoreValue.Text, out scoreTwo);

            if (!scoreOneValid || !scoreTwoValid)
            {
                output = "Your data is not valid";
            }
            else if (scoreOne == scoreTwo)
            {
                output = "This application does not handle ties";
            }

            return output;
        }
        
        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateData();

            if (errorMessage.Length > 0)
            {
                MessageBox.Show($"Input Error: {errorMessage}");
                return;
            }

            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            
            if (m.Entries.Count > 1)
            {
                double score = 0;

                bool isValidScore = double.TryParse(teamOneScoreValue.Text, out score);
                if (isValidScore)
                {
                    m.Entries[0].Score = score;
                }
                else
                {
                    MessageBox.Show("Please enter a valid score for team one.");
                    return;
                }

                isValidScore = double.TryParse(teamTwoScoreValue.Text, out score);
                if (isValidScore)
                {
                    m.Entries[1].Score = score;
                }
                else
                {
                    MessageBox.Show("Please enter a valid score for team two.");
                    return;
                }
            }

            try
            {
                TournamentLogic.UpdateTournamentResults(tournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The application had the following error: {ex.Message}");
                return;
            }
            
            LoadMatchups();
        }
    }
}
