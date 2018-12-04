using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class PrizesController : Controller
    {
        public ActionResult Index()
        {
            List<PrizeModel> allPrizes = GlobalConfig.Connection.GetPrizes_All();
            return View(allPrizes);
        }

        // GET: Prizes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Prizes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PrizeModel p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    GlobalConfig.Connection.CreatePrize(p);

                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
    }
}