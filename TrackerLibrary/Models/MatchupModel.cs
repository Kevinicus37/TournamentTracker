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
        /// Represents the team that won this matchup
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// Represents the round number in which this matchup occurs
        /// </summary>
        public int MatchupRound { get; set; }

    }
}
