using BikeShop.Config;
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

        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Bike bike = ctx.Bikes.Find(id);

                if (bike != null)
                {
                    return View(bike);
                }

                return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");

        }


        public ActionResult New()
        {
            Bike bike = new Bike();
            bike.BikerTypeList = Utilities.GetAllBikerTypes(ctx);
            bike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx);
            bike.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);
            return View(bike);
        }

        [HttpPost]
        public ActionResult Create(Bike bike)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item lists
                    bike.BikerTypeList = Utilities.GetAllBikerTypes(ctx);
                    bike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx);
                    return View("New", bike);
                }

                // add pieces if any
                List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
                if (bike.PiecesListCheckBoxes != null)
                {
                    selectedPieces = bike.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                bike.Pieces = new List<Piece>();
                for (int i = 0; i < selectedPieces.Count(); i++)
                {
                    Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                    if(piece != null)
                    {
                        bike.Pieces.Add(piece);
                    }
                }

                // add info in db
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

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Bike bike = ctx.Bikes.Find(id);
                if (bike != null)
                {
                    // reload select item lists and check boxes lists
                    bike.BikerTypeList = Utilities.GetAllBikerTypes(ctx);
                    bike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx);
                    bike.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);

                    // mark selected pieces
                    foreach (Piece checkedPiece in bike.Pieces)
                    { 
                        bike.PiecesListCheckBoxes.FirstOrDefault(c => c.Id == checkedPiece.PieceId).Checked = true;
                    }

                    return View(bike);
                }

                return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

        [HttpPost]
        public ActionResult Update(Bike updatedBike)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item lists
                    updatedBike.BikerTypeList = Utilities.GetAllBikerTypes(ctx);
                    updatedBike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx);
                    return View("Edit", updatedBike);
                }

                // get selected pieces
                List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
                if (updatedBike.PiecesListCheckBoxes != null)
                {
                    selectedPieces = updatedBike.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                // update bike
                Bike bike = ctx.Bikes.Single(b => b.BikeId == updatedBike.BikeId);
                if (bike == null)
                {
                    return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + updatedBike.BikeId.ToString() + "!");
                }

                if (TryUpdateModel(bike))
                {
                    bike.Name = updatedBike.Name;
                    bike.Description = updatedBike.Description;
                    bike.FabricationDate = updatedBike.FabricationDate;
                    bike.Quantity = updatedBike.Quantity;
                    bike.Price = updatedBike.Price;
                    bike.BikerTypeId = updatedBike.BikerTypeId;
                    bike.BikeCategoryId = updatedBike.BikeCategoryId;

                    bike.Pieces.Clear();
                    bike.Pieces = new List<Piece>();

                    // add new pieces if any
                    for (int i = 0; i < selectedPieces.Count(); i++)
                    {
                        Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                        if (piece != null)
                        {
                            bike.Pieces.Add(piece);
                        }
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                Console.WriteLine("Update-ul pentru bikeId = " + bike.BikeId + " nu a reusit");
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                Bike bike = ctx.Bikes.Find(id);

                if (bike != null)
                {
                    ctx.Bikes.Remove(bike);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}