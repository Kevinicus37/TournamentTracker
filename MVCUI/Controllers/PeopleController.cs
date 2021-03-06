﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class PeopleController : Controller
    {
        // GET: People
        public ActionResult Index()
        {
            List<PersonModel> availablePeople = GlobalConfig.Connection.GetPerson_All();
            return View(availablePeople);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonModel p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    GlobalConfig.Connection.CreatePerson(p);

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

        //// GET: People/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: People/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: People/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: People/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
