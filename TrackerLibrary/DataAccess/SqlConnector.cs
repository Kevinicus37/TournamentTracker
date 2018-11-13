using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";

        public void CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellPhoneNumber", model.CellPhoneNumber);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
            
        }
        
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The Prize Information</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", tm.Id);
                    
                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        public List<TeamModel> GetTeams_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();

                foreach (TeamModel t in output)
                {
                    PopulateTeamMembers(t, connection);
                }
            }

            return output;
        }

        public void PopulateTeamMembers(TeamModel team, IDbConnection connection)
        {
            var p = new DynamicParameters();
            p.Add("@TeamId", team.Id);
            team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {

                SaveTournament(model, connection);
                SaveTournamentEntries(model, connection);
                SaveTournamentPrizes(model, connection);
                SaveTournamentRounds(model, connection);
            }
        }

        private void SaveTournament(TournamentModel model, IDbConnection connection)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
            connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");
        }

        private void SaveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TeamId", tm.Id);
                p.Add("@TournamentId", model.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentPrizes(TournamentModel model, IDbConnection connection)
        {
            foreach (PrizeModel pm in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pm.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);

            }
        }

        private void SaveTournamentRounds(TournamentModel model, IDbConnection connection)
        {
            
                foreach (List<MatchupModel> round in model.Rounds)
                {
                    foreach (MatchupModel match in round)
                    {
                        DynamicParameters p = new DynamicParameters();
                        p.Add("@MatchupRound", match.MatchupRound);
                        p.Add("@TournamentId", model.Id);
                        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

                        match.Id = p.Get<int>("@id");
                        
                        foreach (MatchupEntryModel entry in match.Entries)
                        {
                            p = new DynamicParameters();
                            p.Add("@MatchupId", match.Id);
                            if (entry.ParentMatchup == null)
                            {
                                p.Add("@ParentMatchupId", null);
                            }
                            else
                            {
                                p.Add("@ParentMatchupId", entry.ParentMatchup.Id);

                            }

                            if (entry.TeamCompeting == null)
                            {
                                p.Add("@TeamCompetingId", null);
                            }
                            else
                            {
                                p.Add("@TeamCompetingId", entry.TeamCompeting.Id);

                            }

                            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                            connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);    

                            entry.Id = p.Get<int>("@id");
                        }

                    }
                }
            
            
        }

        public List<TournamentModel> GetTournaments_All()
        {
            // TODO - implement GetTournaments_All for database
            List<TournamentModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                foreach (TournamentModel tmt in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", tmt.Id);

                    // Populate Prizes (GetPrizes by Tournament?)
                    tmt.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    // Populate Teams
                    tmt.EnteredTeams = connection.Query<TeamModel>("dbo.spTeams_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel t in tmt.EnteredTeams)
                    {
                        PopulateTeamMembers(t, connection);
                    }

                    // Populate Rounds
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();
    
                    foreach (MatchupModel matchup in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);
                        
                        matchup.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        List<TeamModel> teams = GetTeams_All();

                        if (matchup.WinnerId > 0)
                        {
                            matchup.Winner = teams.Where(t => t.Id == matchup.WinnerId).First();
                        }

                        foreach (MatchupEntryModel entry in matchup.Entries)
                        {
                            if (entry.TeamCompetingId > 0)
                            {
                                entry.TeamCompeting = teams.Where(t => t.Id == entry.TeamCompetingId).First();
                            }

                            if (entry.ParentMatchupId > 0)
                            {
                                entry.ParentMatchup = matchups.Where(m => m.Id == entry.ParentMatchupId).First();
                            }
                        }

                        if (matchup.MatchupRound > tmt.Rounds.Count)
                        {
                            tmt.Rounds.Add(new List<MatchupModel>());
                        }
                        tmt.Rounds[matchup.MatchupRound - 1].Add(matchup);
                    }
                }
            }
            return output;
        }

        public void UpdateMatchupEntryModel(MatchupEntryModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@id", model.Id);
                p.Add("@TeamCompetingId", model.TeamCompeting.Id);
                p.Add("@Score", model.Score);
                connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure);
            }
        }

        public void UpdateMatchupModel(MatchupModel model)
        {
            // dbo.spMatchups_Update - @id, @WinnerId
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@id", model.Id);
                p.Add("@WinnerId", model.Winner.Id);
                connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure);
            }

            foreach (MatchupEntryModel entry in model.Entries)
            {
                UpdateMatchupEntryModel(entry);
            }
        }

        public void CompleteTournament(TournamentModel tmt)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@id", tmt.Id);
                
                connection.Execute("dbo.spTournament_Inactive", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
