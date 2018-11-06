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

            MarkMatchupWinners(toScore);
            AdvanceWinners(toScore, tmt);
            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchupModel(x));
        }

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

        private static void AdvanceWinners(List<MatchupModel> matchups, TournamentModel tmt)
        {
            List<MatchupModel> nextRoundMatchups = new List<MatchupModel>();

            foreach (MatchupModel m in matchups)
            {
                if (m.MatchupRound < tmt.Rounds.Count)
                {
                    nextRoundMatchups = tmt.Rounds[m.MatchupRound];
                }

                foreach (MatchupModel mm in nextRoundMatchups)
                {
                    foreach (MatchupEntryModel me in mm.Entries)
                    {
                        if (me.ParentMatchup == m)
                        {
                            me.TeamCompeting = m.Winner;
                            GlobalConfig.Connection.UpdateMatchupEntryModel(me);
                            break;
                        }
                    }
                }
            }
        }

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

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            // Order list of teams randomly
            return teams.OrderBy(t => Guid.NewGuid()).ToList(); 
        }
    }
}
