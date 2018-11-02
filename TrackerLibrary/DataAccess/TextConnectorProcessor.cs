using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filesPath"] }\\{ fileName }";
        }

        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();

                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }

            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> prizes, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in prizes)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();

                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellPhoneNumber = cols[4];
                output.Add(p);
            }

            return output;
        }

        public static void SaveToPeopleFile(this List<PersonModel> people, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in people)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellPhoneNumber}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            // id, teamName, list of ids separated by pipe
            // 3, AwesomeSauce, 3|4|8|1

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                string[] ids = cols[2].Split('|');

                

                TeamModel t = new TeamModel();

                foreach (string id in ids)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];
                output.Add(t);
            }

            return output;
        }

        public static void SaveToTeamsFile(this List<TeamModel> teams, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in teams)
            {
                lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            if (people.Count == 0)
            {
                return "";
            }

            string output = "";

            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines,
            string teamsFileName,
            string peopleFileName,
            string prizesFileName)
        {
            // id,TournamentName,EntryFee,1|3(TeamIds),1|4|2(PrizeIds),id^id^id^id|id^id|id(Rounds)
            
            List<TournamentModel> output = new List<TournamentModel>();
            List<PrizeModel> prizes = prizesFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<TeamModel> teams = teamsFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                
                TournamentModel t = new TournamentModel();
                t.Id = int.Parse(cols[0]);
                t.TournamentName = cols[1];
                t.EntryFee = decimal.Parse(cols[2]);

                if (cols[3].Length > 0)
                {
                    string[] teamIds = cols[3].Split('|');

                    foreach (string id in teamIds)
                    {
                        t.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                    } 
                }

                if (cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');

                    foreach (string id in prizeIds)
                    {
                        t.Prizes.Add(prizes.Where(p => p.Id == int.Parse(id)).First());
                    } 
                }

                // Capture Rounds information
                string[] rounds = cols[5].Split('|');

                foreach (string round in rounds)
                {
                    List<MatchupModel> matches = new List<MatchupModel>();

                    string[] matchIds = round.Split('^');
                    
                    foreach (string matchId in matchIds)
                    {
                        matches.Add(matchups.Where(m => m.Id == int.Parse(matchId)).First());      
                    }

                    t.Rounds.Add(matches);
                }
                
                output.Add(t);
            }
            return output;
        }

        public static void SaveRoundsToFile(this TournamentModel tmt, string matchupFileName, string matchupEntryFileName)
        {
            foreach (List<MatchupModel> round in tmt.Rounds)
            {
                foreach (MatchupModel match in round)
                {
                    match.SaveToMatchupFile(matchupFileName, matchupEntryFileName);
                    
                }
            }
            
        }

        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
            List<string> lines = new List<string>();

            MatchupEntryModel oldEntry = new MatchupEntryModel();
            foreach (MatchupEntryModel e in entries)
            {
                if (e.Id == entry.Id)
                {
                    oldEntry = e;
                    break;
                }
            }

            entries.Remove(oldEntry);
            entries.Add(entry);

            foreach (MatchupEntryModel e in entries)
            {
                string teamCompetingId = "";
                if (e.TeamCompeting != null)
                {
                    teamCompetingId = e.TeamCompeting.Id.ToString();
                }

                string parentMatchupId = "";
                if (e.ParentMatchup != null)
                {
                    parentMatchupId = e.ParentMatchup.Id.ToString();
                }

                lines.Add($"{e.Id},{teamCompetingId},{e.Score},{parentMatchupId}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);

        }

        public static void UpdateMatchupToFile(this MatchupModel match)
        { 
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            List<string> lines = new List<string>();

            MatchupModel oldMatchup = new MatchupModel();
            foreach (MatchupModel m in matchups)
            {
                if (m.Id == match.Id)
                {
                    oldMatchup = m;
                    break;
                }
            }

            matchups.Remove(oldMatchup);
            
            foreach (MatchupEntryModel entry in match.Entries)
            {
                entry.UpdateEntryToFile();
            }

            matchups.Add(match);

            // id=0,entries=1 pipe separated, winner = 2, matchupRound = 3
            foreach (MatchupModel m in matchups)
            {
                string winner = "";

                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }

                string entries = ConvertIdsToSeparatedString<MatchupEntryModel>(m.Entries, "|");

                lines.Add($"{m.Id},{entries},{winner},{m.MatchupRound}");
            }

            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        private static void SaveToMatchupFile(this MatchupModel match, string matchupFileName, string matchupEntryFileName)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            List<string> lines = new List<string>();

            int currentId = 1;

            if (matchups.Count > 0)
            {
                currentId = matchups.Max(m => m.Id) + 1;
            }

            match.Id = currentId;

            foreach (MatchupEntryModel entry in match.Entries)
            {
                entry.SaveEntryToFile();
            }

            matchups.Add(match);

            // id=0,entries=1 pipe separated, winner = 2, matchupRound = 3
            foreach (MatchupModel matchup in matchups)
            {
                string winner = "";

                if (matchup.Winner != null)
                {
                    winner = matchup.Winner.Id.ToString();
                }

                string entries = ConvertIdsToSeparatedString<MatchupEntryModel>(matchup.Entries, "|");

                lines.Add($"{matchup.Id},{entries},{winner},{matchup.MatchupRound}");
            }

            File.WriteAllLines(matchupFileName.FullFilePath(), lines);
        }

        private static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
            List<string> lines = new List<string>();

            int currentId = 1;

            if (entries.Count > 0)
            {
                currentId = entries.Max(e => e.Id) + 1;
            }

            entry.Id = currentId;
            entries.Add(entry);
            
            foreach (MatchupEntryModel e in entries)
            {
                string teamCompetingId = "";
                if (e.TeamCompeting != null)
                {
                    teamCompetingId = e.TeamCompeting.Id.ToString();
                }

                string parentMatchupId = "";
                if (e.ParentMatchup != null)
                {
                    parentMatchupId = e.ParentMatchup.Id.ToString();
                }

                lines.Add($"{e.Id},{teamCompetingId},{e.Score},{parentMatchupId}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            // id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupEntryModel entry = new MatchupEntryModel();
                entry.Id = int.Parse(cols[0]);

                if (cols[1].Length == 0)
                {
                    entry.TeamCompeting = null;
                }
                else
                {
                    entry.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }

                entry.Score = double.Parse(cols[2]);

                int parentId = 0;

                if (int.TryParse(cols[3], out parentId))
                {
                    entry.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    entry.ParentMatchup = null;
                }

                output.Add(entry);
            }
            
            return output;
        }

        public static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
                
            }

            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile();

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');

                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        private static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile();

            foreach (string team in teams)
            {
                string[] cols = team.Split(',');

                if (cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
                }
            }

            return null;
            
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            // id=0,entries=1 pipe separated, winner = 2, matchupRound = 3
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupModel m = new MatchupModel();

                m.Id = int.Parse(cols[0]);
                m.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                
                if (cols[2].Length == 0)
                {
                    m.Winner = null;
                }
                else
                {
                    m.Winner = LookupTeamById(int.Parse(cols[2]));
                }

                m.MatchupRound = int.Parse(cols[3]);
                
                output.Add(m);
            }

            return output;
        }

        public static void SaveToTournamentsFile(this List<TournamentModel> tournaments, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tmt in tournaments)
            {
                lines.Add($@"{tmt.Id},{tmt.TournamentName},{tmt.EntryFee},{ConvertIdsToSeparatedString<TeamModel>(tmt.EnteredTeams, "|")},{ConvertIdsToSeparatedString<PrizeModel>(tmt.Prizes, "|")},{ConvertRoundsListToString(tmt.Rounds)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            // round 1|round 2|round 3
            // id^id^id^id|id^id|id

            if (rounds.Count == 0)
            {
                return "";
            }

            string output = "";

            foreach (List<MatchupModel> round in rounds)
            {
                output += $"{ConvertIdsToSeparatedString<MatchupModel>(round, "^")}|";
            }

            output = output.Substring(0, output.Length - 1);
            
            return output;
        }

        //private static string ConvertTeamListToString(List<TeamModel> teams)
        //{
        //    if (teams.Count == 0)
        //    {
        //        return "";
        //    }

        //    string output = "";

        //    foreach (TeamModel t in teams)
        //    {
        //        output += $"{t.Id}|";
        //    }

        //    output = output.Substring(0, output.Length - 1);

        //    return output;
        //}

        //private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        //{
        //    if (prizes.Count == 0)
        //    {
        //        return "";
        //    }

        //    string output = "";

        //    foreach (PrizeModel p in prizes)
        //    {
        //        output += $"{p.Id}|";
        //    }

        //    output = output.Substring(0, output.Length - 1);

        //    return output;
        //}

        private static string ConvertIdsToSeparatedString<T>(List<T> models, string separator) 
        {
            if (models.Count == 0)
            {
                return "";
            }

            string output = "";

            foreach (T m in models)
            {
                var id = typeof(T).GetProperty("Id").GetValue(m);
                output += $"{id}{separator}";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}
