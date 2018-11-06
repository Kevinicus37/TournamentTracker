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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreatePrize(PrizeModel model)
        {
            // Load the textFile
            // Convert to List<PrizeModel>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

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
            prizes.SaveToPrizeFile();
        }

        public void CreatePerson(PersonModel model)
        {
            // Load the textFile
            // Convert to List<PrizeModel>
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

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
            people.SaveToPeopleFile();
        }

        /// <summary>
        /// Creates a Team entry on the text file
        /// </summary>
        /// <param name="model">Represents the team to be added</param>
        /// <returns>An updated team with the stored id.</returns>
        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.Max(t => t.Id) + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamsFile();
        }

        /// <summary>
        /// Gets all the Members stored and available.
        /// </summary>
        /// <returns>A list of All Available Members</returns>
        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeams_All()
        {
            return GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GetTournaments_All();

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments.Max(t => t.Id) + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentsFile();
        }

        public List<TournamentModel> GetTournaments_All()
        {
            return GlobalConfig.TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels();
        }

        public void UpdateMatchupModel(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }

        public void UpdateMatchupEntryModel(MatchupEntryModel model)
        {
            model.UpdateEntryToFile();
        }
    }
}
