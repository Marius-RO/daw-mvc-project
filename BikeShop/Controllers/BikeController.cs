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
        private ApplicationDbContext ctx = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<Bike> bikes = ctx.Bikes.ToList();
            return View(bikes);
        }

        public ActionResult Details(int id)
        {
            Bike bike = ctx.Bikes.Find(id);
            return View(bike);
        }


        public ActionResult New()
        {
            Bike bike = new Bike();
            return View(bike);
        }

        [HttpPost]
        public ActionResult Create(Bike bike)
        {
            ctx.Bikes.Add(bike);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Bike bike = ctx.Bikes.Find(id);
            return View(bike);
        }

        [HttpPost]
        public ActionResult Update(Bike updatedBike)
        {

            Bike bike = ctx.Bikes.Single(b => b.BikeId == updatedBike.BikeId);
            bike.Name = updatedBike.Name;
            bike.Description = updatedBike.Description;
            bike.FabricationDate = updatedBike.FabricationDate;
            bike.Price = updatedBike.Price;

            ctx.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {

            Console.WriteLine("Aici");

            Bike bike = ctx.Bikes.Find(id);
            ctx.Bikes.Remove(bike);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}