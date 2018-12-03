using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary.Models;

namespace MVCUI.Models
{
    public class TeamMVCModel
    {
        [Display(Name = "Team Name")]
        [StringLength(100, MinimumLength = 2)]
        [Required]
        public string TeamName { get; set; }

        [Required]
        [Display(Name = "Team Members")]
        public List<SelectListItem> TeamMembers { get; set; } = new List<SelectListItem>();

        [CannotBeEmpty(ErrorMessage = "Must select at least one Team Member")]
        public List<string> SelectedTeamMembers { get; set; } = new List<string>();
    }
}