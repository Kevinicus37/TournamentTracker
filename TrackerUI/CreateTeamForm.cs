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
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();

        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();

        private ITeamRequester callingForm;

        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();

            callingForm = caller;
            // CreateSampleData();

            WireUpLists();
        }
        
        /// <summary>
        /// Creates sample data for testing
        /// </summary>
        private void CreateSampleData()
        {


            availableTeamMembers.Add(new PersonModel { FirstName = "Kevin", LastName = "Childs" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Kevinicus", LastName = "Chldsplay" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "Oubon", LastName = "Phongsavath" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Katie", LastName = "Sirridge" });

        }

        /// <summary>
        /// Sets up the datasource and display members for the dropdown and listbox
        /// </summary>
        private void WireUpLists()
        {

            // TODO - Find a better way to refresh data

            selectTeamMemberDropDown.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        /// <summary>
        /// Creates a new member from user input and saves the new member
        /// to the db or text file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createMemberButton_Click(object sender, EventArgs e)
        {
            // create and save member if the user input is valid
            if (ValidateForm())
            {
                PersonModel p = new PersonModel
                {
                    FirstName = firstNameValue.Text,
                    LastName = lastNameValue.Text,
                    EmailAddress = emailAddressValue.Text,
                    CellPhoneNumber = cellPhoneValue.Text
                };

                // Add new Person to db or text file
                p = GlobalConfig.Connection.CreatePerson(p);

                // Add new member to current team 
                selectedTeamMembers.Add(p);
                WireUpLists();

                // Clear fields for new member creation
                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailAddressValue.Text = "";
                cellPhoneValue.Text = "";

            }
            else
            {
                // Probably want to use a comment a little less abrasive.  But this is for learning
                // so let's have fun with it.
                MessageBox.Show("You done fucked up.  Try again, dumbass.");
            }
        }

        /// <summary>
        /// Validates user input
        /// </summary>
        /// <returns>A bool indicating whether or not the input is valid</returns>
        private bool ValidateForm()
        {
            // TODO - Add validation to the form.
            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }

            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }

            if (emailAddressValue.Text.Length == 0)
            {
                return false;
            }

            if (cellPhoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Selects a team member to add to the team
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel) selectTeamMemberDropDown.SelectedItem;

            if (p != null)
            {
                selectedTeamMembers.Add(p);
                availableTeamMembers.Remove(p);
                WireUpLists(); 
            }
            
        }

        /// <summary>
        /// Removes a member from the team
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeTeamMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);
                WireUpLists();
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new TeamModel();

            if (ValidateTeamData())
            {
                t.TeamName = teamNameValue.Text;
                t.TeamMembers = selectedTeamMembers;

                t = GlobalConfig.Connection.CreateTeam(t);

                callingForm.TeamComplete(t);

                this.Close();
            }
            else
            {
                MessageBox.Show("You done screwed up again.  Fill your boxes, bitch!");
            }

            // TODO - If form isn't closed after this - reset the form.   
        }

        private bool ValidateTeamData()
        {
            if (String.IsNullOrEmpty(teamNameValue.Text))
            {
                return false;
            }

            if (selectedTeamMembers.Count < 1)
            {
                return false;
            }

            return true;
        }
    }
}
