using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "First Name")]
        [StringLength(100, MinimumLength = 2)]
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Represents the last name of this player or team member
        /// </summary>
        [Display(Name = "Last Name")]
        [StringLength(100, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Represents the Email address of this player or team member
        /// </summary>
        [Display(Name = "Email Address")]
        [StringLength(100, MinimumLength = 6)]
        [Required]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Represents the Cell Phone Number of this player or team member
        /// </summary>
        [Display(Name = "Cell Phone Number")]
        [StringLength(20, MinimumLength = 10)]
        [Required]
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
