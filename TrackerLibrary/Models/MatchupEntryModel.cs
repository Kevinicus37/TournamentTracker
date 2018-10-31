using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        /// <summary>
        /// Unique Identifier for this entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID from the Database used to look up the Team Competing
        /// </summary>
        public int TeamCompetingId { get; set; }

        /// <summary>
        /// Represents one team in the matchup
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// Represents the score for this particular team.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The ID from the Database used to look up the Parent Matchup
        /// </summary>
        public int ParentMatchupId { get; set; }

        /// <summary>
        /// Represents the Matchup that this team came from 
        /// as the winner.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
    }
}
