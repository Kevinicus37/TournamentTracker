using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCUI.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCUI.Models
{
    public class TournamentMVCModel
    {
        /// <summary>
        /// Represents the name of this tournament
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Tournament Name")]
        public string TournamentName { get; set; }

        /// <summary>
        /// Represents the fee required to enter the tournament
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Entry Fee")]
        public decimal EntryFee { get; set; }

        /// <summary>
        /// Represents all of the teams currently entered into this
        /// tournament
        /// </summary>
        [Display(Name = "Entered Teams")]
        public List<SelectListItem> EnteredTeams { get; set; } = new List<SelectListItem>();

        [MinimumCount(2, ErrorMessage = "Must have at least two Teams in a tournament.")]
        public List<string> SelectedEnteredTeams { get; set; } = new List<string>();

        /// <summary>
        /// Represents all of the available prizes to be awarded for 
        /// this tournament
        /// </summary>
        [Display(Name = "Prizes")]
        public List<SelectListItem> Prizes { get; set; } = new List<SelectListItem>();

        [CountLimit(2, ErrorMessage = "You cannot pick more than 2 prizes.")]
        public List<string> SelectedPrizes { get; set; } = new List<string>();


    }
}