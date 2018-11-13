using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentComplete;

        /// <summary>
        /// Unique Identifier for this tournament
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the name of this tournament
        /// </summary>
        public string TournamentName { get; set; }

        /// <summary>
        /// Represents the fee required to enter the tournament
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// Represents all of the teams currently entered into this
        /// tournament
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

        /// <summary>
        /// Represents all of the available prizes to be awarded for 
        /// this tournament
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

        /// <summary>
        /// Represents all of the rounds of matchups that will be played
        /// during this tournament
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();

        public void CompleteTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}
