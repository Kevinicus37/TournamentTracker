using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        /// <summary>
        /// Unique Identifier for this Matchup
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents all of the teams in this particular matchup
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// The ID from database used to look up the winner.
        /// </summary>
        public int WinnerId { get; set; }

        /// <summary>
        /// Represents the team that won this matchup
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// Represents the round number in which this matchup occurs
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName
        {
            get
            {
                string output = "";

                foreach (MatchupEntryModel m in Entries)
                {
                    string thisTeamName = "Unknown";
                    if (m.TeamCompeting != null)
                    {
                        thisTeamName = m.TeamCompeting.TeamName;
                    }
                    output += $"{thisTeamName} vs. ";
                }

                if (Entries.Count < 2)
                {
                    output += "Bye";
                }
                else
                {
                    output = output.Substring(0, output.Length - 5);
                }

                return output;
            }

        }
            

    }
}
