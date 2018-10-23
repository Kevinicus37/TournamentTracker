using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;
namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {

        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamsFile = "TeamModels.csv";
        private const string TeamMembersFile = "TeamMemberModels.csv";
        private const string TournamentsFile = "TournamentModels.csv";

        // TODO - Make CreatePrize method actually save to a Text file
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Load the textFile
            // Convert to List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find new id (Max id of existing prizes + 1)
            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            // Add record with new id
            prizes.Add(model);

            // Convert Prizes to List<string>
            // Add List<string> to text file
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }

        public PersonModel CreatePerson(PersonModel model)
        {
            // Load the textFile
            // Convert to List<PrizeModel>
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find new id (Max id of existing prizes + 1)
            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            // Add record with new id
            people.Add(model);

            // Convert Prizes to List<string>
            // Add List<string> to text file
            people.SaveToPeopleFile(PeopleFile);


            return model;
        }

        /// <summary>
        /// Creates a Team entry on the text file
        /// </summary>
        /// <param name="model">Represents the team to be added</param>
        /// <returns>An updated team with the stored id.</returns>
        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.Max(t => t.Id) + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamsFile(TeamsFile);

            return model;
        }

        /// <summary>
        /// Gets all the Members stored and available.
        /// </summary>
        /// <returns>A list of All Available Members</returns>
        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeams_All()
        {
            return TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamsFile, PeopleFile, PrizesFile);

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments.Max(t => t.Id) + 1;
            }

            model.Id = currentId;

            tournaments.Add(model);

            tournaments.SaveToTournamentsFile(TournamentsFile);
        }
    }
}
