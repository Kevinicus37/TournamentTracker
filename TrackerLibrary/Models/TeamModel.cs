using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        /// <summary>
        /// Unique Identifier for this team
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the name of this team.
        /// </summary>
        public string TeamName { get; set; }
        
        /// <summary>
        /// Represents all of the people that belong to this team
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

        
    }
}
