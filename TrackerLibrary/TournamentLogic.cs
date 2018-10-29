using System;
using System.Collections.Generic;
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
