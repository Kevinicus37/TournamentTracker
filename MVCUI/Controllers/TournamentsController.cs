using MVCUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class TournamentsController : Controller
    {
        // GET: Tournaments
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Create()
        {
            TournamentMVCModel input = new TournamentMVCModel();

            List<TeamModel> allTeams = GlobalConfig.Connection.GetTeams_All();
            List<PrizeModel> allPrizes = GlobalConfig.Connection.GetPrizes_All();

            input.EnteredTeams = allTeams.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TeamName }).ToList();
            input.Prizes = allPrizes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.PlaceName }).ToList();

            return View(input);
        }

        // POST: Tournaments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TournamentMVCModel model)
        {
            try
            {
                if (ModelState.IsValid && model.SelectedEnteredTeams.Count > 1)
                {
                    TournamentModel tmt = new TournamentModel();

                    tmt.TournamentName = model.TournamentName;
                    tmt.EntryFee = model.EntryFee;
                    tmt.EnteredTeams = model.SelectedEnteredTeams.Select(x => new TeamModel { Id = int.Parse(x) }).ToList();
                    tmt.Prizes = model.SelectedPrizes.Select(x => new PrizeModel { Id = int.Parse(x) }).ToList();
                    TournamentLogic.CreateTournamentRounds(tmt);

                    GlobalConfig.Connection.CreateTournament(tmt);

                    tmt.AlertUsersToNewRound();

                    return RedirectToAction("Index");
                }
                else
                {
                    List<TeamModel> allTeams = GlobalConfig.Connection.GetTeams_All();
                    List<PrizeModel> allPrizes = GlobalConfig.Connection.GetPrizes_All();

                    model.EnteredTeams = allTeams
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TeamName, Selected = model.SelectedEnteredTeams.Contains(x.Id.ToString()) })
                        .ToList();
                    model.Prizes = allPrizes
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.PlaceName, Selected = model.SelectedPrizes.Contains(x.Id.ToString()) })
                        .ToList();
                    return View(model);
                }
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }
    }
}