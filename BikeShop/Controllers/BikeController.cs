using BikeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BikeShop.Controllers
{
    public class BikeController : Controller
    {

        public ActionResult Index()
        {
            List<Bike> bikes = new List<Bike>();
            return View(bikes);
        }

        public ActionResult Details(int id)
        {
            Bike bike = new Bike();
            return View(bike);
        }


        public ActionResult New()
        {
            Bike bike = new Bike();
            return View(bike);
        }

        public ActionResult Create(Bike bike)
        {
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Bike bike = new Bike();
            return View(bike);
        }

        public ActionResult Update()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {
            return RedirectToAction("Index");
        }


    }
}