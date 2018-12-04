using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);
        void CreatePerson(PersonModel model);
        void CreateTeam(TeamModel model);
        void CreateTournament(TournamentModel model);
        void UpdateMatchupModel(MatchupModel model);
        void UpdateMatchupEntryModel(MatchupEntryModel model);
        void CompleteTournament(TournamentModel model);

        List<TournamentModel> GetTournaments_All();
        List<TeamModel> GetTeams_All();
        List<PersonModel> GetPerson_All();
        List<PrizeModel> GetPrizes_All();
    }
}
