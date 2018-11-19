using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        public static void CreateTournamentRounds(TournamentModel tmt)
        {
            // Randomize order of Teams
            // Check if list is big enough (2^N), if not - add in byes
            // Create First round of matchups
            // Create every round after that

            List<TeamModel> randomizedTeams = RandomizeTeamOrder(tmt.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = FindNumberOfByes(randomizedTeams.Count, rounds);

            tmt.Rounds.Add(CreateFirstRound(byes, randomizedTeams));
            CreateRemainingRounds(rounds, tmt);
            
            UpdateTournamentResults(tmt);
        }

        public static void UpdateTournamentResults(TournamentModel tmt)
        {
            int startingRound = tmt.CheckCurrentRound();
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> round in tmt.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    if (matchup.Winner == null && (matchup.Entries.Any(x => x.Score != 0) || matchup.Entries.Count == 1))
                    {
                        toScore.Add(matchup);
                    }
                }
            }

            // TODO - Why are entries not updating properly?
            MarkMatchupWinners(toScore);
            AdvanceWinners(toScore, tmt);
            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchupModel(x));

            int endingRound = tmt.CheckCurrentRound();
            if (endingRound > startingRound && endingRound <= tmt.Rounds.Count)
            {
                // Alert users
                tmt.AlertUsersToNewRound();
            }
        }

        public static void AlertUsersToNewRound(this TournamentModel tmt)
        {
            int currentRoundNumber = tmt.CheckCurrentRound();
            List<MatchupModel> currentRound = tmt.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();
            
            List<string> to = new List<string>();

            foreach (MatchupModel m in currentRound)
            {
                foreach (MatchupEntryModel me in m.Entries)
                {
                    MatchupEntryModel competitor = m.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault();

                    string competitorName = "";

                    if (competitor != null)
                    {
                        competitorName = competitor.TeamCompeting.TeamName;
                    }

                    foreach (PersonModel p in me.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(p, me.TeamCompeting.TeamName, competitorName);
                    }
                }
            }
        }

        private static void AlertPersonToNewRound(PersonModel p, string teamName, string competitorName)
        {
            if (p.EmailAddress.Length == 0)
            {
                return;
            }

            string to = p.EmailAddress;
            string subject = "";
            StringBuilder body = new StringBuilder();
        
            if (competitorName.Length > 0)
            {
                subject = $"You have a new matchup with {competitorName}";
                body.AppendLine("<h1>You have a new matchup!</h1>");
                body.Append("<strong>Competitor: </strong>");
                body.Append(competitorName);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("Have a great time!");
                body.AppendLine("~Tournament Tracker");
            }
            else
            {
                subject = "You have a bye this round.";

                body.AppendLine("Enjoy your round off.");
                body.AppendLine("~Tournament Tracker");
            }

            EmailLogic.SendEmail(to, subject, body.ToString());
        }

        private static int CheckCurrentRound(this TournamentModel tmt)
        {
            int output = 1;

            foreach (List<MatchupModel> round in tmt.Rounds)
            {
                // This is not consistent with Tim's comments on assuming.  
                // This method only works properly if you assume the rounds
                // are in ascending order.  
                if (round.All(x => x.Winner != null))
                {
                    output++;
                }
                else
                {
                    return output;
                }
            }
            
            // If output is never returned because all tournament winners 
            // are determined, then Tournament is Complete
            CompleteTournament(tmt);

            return output - 1;
        }

        private static void CompleteTournament(TournamentModel tmt)
        {
            GlobalConfig.Connection.CompleteTournament(tmt);
            TeamModel winner = tmt.Rounds.Last().First().Winner;
            TeamModel runnerUp = tmt.Rounds.Last().First().Entries.Where(x => x.TeamCompeting != winner).First().TeamCompeting;

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;
            
            // Allocate Prizes
            if (tmt.Prizes.Count > 0)
            {
                decimal totalIncome = tmt.EntryFee * tmt.EnteredTeams.Count;

                PrizeModel firstPlacePrize = tmt.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
                PrizeModel secondPlacePrize = tmt.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizeValue(totalIncome);
                }

                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizeValue(totalIncome);
                }
            }

            // Email Everyone
            string subject = "";
            StringBuilder body = new StringBuilder();

            subject = $"In {tmt.TournamentName}, {winner.TeamName} has won!";
            body.AppendLine("<h1>WE HAVE A WINNER!</h1>");
            body.AppendLine("<p>Congratulations to our winner on a great tournament!");
            body.AppendLine("<br />");

            if (winnerPrize > 0)
            {
                body.AppendLine($"<p>{winner.TeamName} will receive ${winnerPrize} for winning the tournament.</p>");
            }

            if (runnerUpPrize > 0)
            {
                body.AppendLine($"<p>{runnerUp.TeamName} will receive ${runnerUpPrize} for coming in second.</p>");
            }

            body.AppendLine("<p>Thanks for a great tournament, everyone!</p>");
            body.AppendLine("~Tournament Tracker");

            List<string> bcc = new List<string>();
            
            foreach (TeamModel t in tmt.EnteredTeams)
            {
                foreach (PersonModel p in t.TeamMembers)
                {
                    if (p.EmailAddress.Length > 0)
                    {
                        bcc.Add(p.EmailAddress);
                    }
                }
            }

            EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());

            // Complete Tournament
            tmt.CompleteTournament();
        }

        /// <summary>
        /// Determines the prize value based on the total income of the tournament
        /// </summary>
        /// <param name="p">The prize the value is to be determined for.</param>
        /// <param name="TotalIncome">Total tournament income.</param>
        /// <returns>The value of the prize</returns>
        private static decimal CalculatePrizeValue(this PrizeModel p, decimal TotalIncome)
        {
            decimal output = 0;

            if (p.PrizeAmount > 0)
            {
                output = p.PrizeAmount;
            }
            else
            {
                output = Decimal.Multiply(TotalIncome, Convert.ToDecimal(p.PrizePercentage / 100));
            }

            return output; 
        }

        /// <summary>
        /// Uses the scores of the Entries to determine and set the winners for 
        /// each matchup in a round of matchups.
        /// </summary>
        /// <param name="matchups"></param>
        private static void MarkMatchupWinners(List<MatchupModel> matchups)
        {
            // 1 (default) or 0
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in matchups)
            {
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }

                // 0 = lower score wins
                if (greaterWins == "0")
                {
                    if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("You have a tie - play again!");
                    }
                }
                else
                {
                    // 1 or high score wins
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("You have a tie - play again!");
                    }
                }
            }
        }

        /// <summary>
        /// Take the winners of the matchups in a round and move them to Entries
        /// in matchups in the next round.
        /// </summary>
        /// <param name="matchups">Current round of matchups being scored</param>
        /// <param name="tmt">The tournament being played.</param>
        private static void AdvanceWinners(List<MatchupModel> matchups, TournamentModel tmt)
        {
            List<MatchupModel> nextRoundMatchups = new List<MatchupModel>();

            foreach (MatchupModel m in matchups)
            {
                if (m.MatchupRound < tmt.Rounds.Count)
                {
                    nextRoundMatchups = tmt.Rounds.Where(x=> x.First().MatchupRound == m.MatchupRound + 1).First();
                }

                foreach (MatchupModel mm in nextRoundMatchups)
                {
                    foreach (MatchupEntryModel me in mm.Entries)
                    {

                        if (me.ParentMatchup != null)
                        {
                            if (me.ParentMatchup.Id == m.Id)
                            {
                                me.TeamCompeting = m.Winner;
                                GlobalConfig.Connection.UpdateMatchupEntryModel(me);
                                break;
                            } 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create the matchups and entries for all rounds beyond the first
        /// </summary>
        /// <param name="rounds">The number of rounds in the tournament</param>
        /// <param name="tmt">The tournament being played.</param>
        private static void CreateRemainingRounds(int rounds, TournamentModel tmt)
        {
            int round = 2;
            List<MatchupModel> previousRound = tmt.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                }
                tmt.Rounds.Add(currRound);
                previousRound = currRound;
                currRound = new List<MatchupModel>();
                round++;
            }
        }

        /// <summary>
        /// Creates the first round matchups and entries in the tournament
        /// </summary>
        /// <param name="byes">The number of byes needed for the first round</param>
        /// <param name="teams">The entered teams in this tournament.</param>
        /// <returns></returns>
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> matchups = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });
                if (byes > 0 || currentMatchup.Entries.Count > 1)
                {
                    currentMatchup.MatchupRound = 1;
                    matchups.Add(currentMatchup);
                    currentMatchup = new MatchupModel();

                    if (byes > 0)
                    {
                        byes--;
                    }
                }
            }

            return matchups;
        }

        /// <summary>
        /// Determine the number of byes required for the first round.
        /// </summary>
        /// <param name="teamCount">The number of teams entered in the tournament</param>
        /// <param name="rounds">The number of rounds in the tournament</param>
        /// <returns>The number of byes in the first round.</returns>
        private static int FindNumberOfByes(int teamCount, int rounds)
        {
            int roundOneSlots = 1;
            
            // byes = 2^(rounds -1) - teamCount
            for (int i = 1;  i <= rounds; i++)
            {
                roundOneSlots *= 2;
            }

            return roundOneSlots - teamCount;
        }

        /// <summary>
        /// Determine the number of rounds in the tournament
        /// </summary>
        /// <param name="teamCount">The number of teams entered in the tournament</param>
        /// <returns>The number of rounds in the tournament</returns>
        private static int FindNumberOfRounds(int teamCount)
        {
            int rounds = 1;
            int x = 2;
            
            while (x < teamCount)
            {
                x *= 2;
                rounds++;
            }

            return rounds;
        }

        /// <summary>
        /// Randomizes the order of the entered teams so that the matchups and 
        /// byes are arranged in a fair way
        /// </summary>
        /// <param name="teams">The entered teams in the tournament</param>
        /// <returns>A list of the teams in a random order.</returns>
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            // Order list of teams randomly
            return teams.OrderBy(t => Guid.NewGuid()).ToList(); 
        }
    }
}
