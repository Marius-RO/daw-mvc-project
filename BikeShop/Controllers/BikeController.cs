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
            bike.PiecesListCheckBoxes = GetAllPiecesChecboxes();
            return View(bike);
        }

        [HttpPost]
        public ActionResult Create(Bike bike)
        {
            if (!ModelState.IsValid)
            {
                return View("New", bike);
            }

            var selectedPieces = bike.PiecesListCheckBoxes.Where(b => b.Checked).ToList();

            try
            {
                bike.Pieces = new List<Piece>();
                for (int i = 0; i < selectedPieces.Count(); i++)
                {
                    Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                    bike.Pieces.Add(piece);
                }

                ctx.Bikes.Add(bike);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(bike);
            }
        }

        public ActionResult Edit(int id)
        {
            Bike bike = ctx.Bikes.Find(id);
            return View(bike);
        }

        [HttpPost]
        public ActionResult Update(Bike updatedBike)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", updatedBike);
            }

            Bike bike = ctx.Bikes.Single(b => b.BikeId == updatedBike.BikeId);
            bike.Name = updatedBike.Name;
            bike.Description = updatedBike.Description;
            bike.FabricationDate = updatedBike.FabricationDate;
            bike.Price = updatedBike.Price;
            bike.Quantity = updatedBike.Quantity;

            ctx.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {

            Bike bike = ctx.Bikes.Find(id);
            ctx.Bikes.Remove(bike);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        private List<CheckBoxModel> GetAllPiecesChecboxes()
        {
            var checkboxList = new List<CheckBoxModel>();
            foreach (var piece in ctx.Pieces.ToList())
            {
                checkboxList.Add(new CheckBoxModel
                {
                    Id = piece.PieceId,
                    Name = piece.Name,
                    DisplayName = "  " + piece.Name,
                    Checked = false
                });
            }
            return checkboxList;
        }

    }
}