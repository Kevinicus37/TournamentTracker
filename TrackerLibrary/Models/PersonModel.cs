using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {

        /// <summary>
        /// The unique identifier for this person object
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Represents the first name of this player or team member
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Represents the last name of this player or team member
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Represents the Email address of this player or team member
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Represents the Cell Phone Number of this player or team member
        /// </summary>
        public string CellPhoneNumber { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

    }
}
