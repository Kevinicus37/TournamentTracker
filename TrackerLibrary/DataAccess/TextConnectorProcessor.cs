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

    // * Load the textFile
    // * Convert to List<PrizeModel>
    
    // Convert Prizes to List<string>
    // Add List<string> to text file


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
            // id,TournamentName,EntryFee,1|3(TeamIds),1|4|2(PrizeIds),id^id^id|id^id^id|id^id^id(Rounds)
            
            List<TournamentModel> output = new List<TournamentModel>();
            List<PrizeModel> prizes = prizesFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<TeamModel> teams = teamsFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                string[] roundList = cols[5].Split('|');
                
                TournamentModel t = new TournamentModel();
                t.Id = int.Parse(cols[0]);
                t.TournamentName = cols[1];
                t.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach (string id in teamIds)
                {
                    t.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                string[] prizeIds = cols[4].Split('|');


                foreach (string id in prizeIds)
                {
                    t.Prizes.Add(prizes.Where(p => p.Id == int.Parse(id)).First());
                }

                // TODO - Capture Rounds information
                
                output.Add(t);
            }
            return output;
        }

        public static void SaveToTournamentsFile(this List<TournamentModel> tournaments, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tmt in tournaments)
            {
                lines.Add($@"{tmt.Id},
                             {tmt.TournamentName},
                             {tmt.EntryFee},
                             {ConvertToSeparatedString<TeamModel>(tmt.EnteredTeams, "|")},
                             {ConvertToSeparatedString<PrizeModel>(tmt.Prizes, "|")},
                             {ConvertRoundsListToString(tmt.Rounds)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            // id^id^id|id^id^id

            if (rounds.Count == 0)
            {
                return "";
            }

            string output = "";

            foreach (List<MatchupModel> round in rounds)
            {
                output += $"{ConvertToSeparatedString<MatchupModel>(round, "^")}|";
            }

            output = output.Substring(0, output.Length - 1);
            
            return output;
        }


        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            if (teams.Count == 0)
            {
                return "";
            }

            string output = "";

            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            if (prizes.Count == 0)
            {
                return "";
            }

            string output = "";

            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        

        private static string ConvertToSeparatedString<T>(List<T> models, string separator) 
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
