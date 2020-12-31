using BikeShop.Config;
using BikeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BikeShop.Controllers
{
    public class PieceController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<Piece> pieces = ctx.Pieces.ToList();
            return View(pieces);
        }

        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Piece piece = ctx.Pieces.Find(id);

                if (piece != null)
                {
                    return View(piece);
                }

                return HttpNotFound("Nu s-a gasit piesa cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");

        }


        public ActionResult New()
        {
            Piece piece = new Piece();
            piece.SellingOptionList = Utilities.GetSellingOptions();
            piece.BikeCheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
            return View(piece);
        }

        [HttpPost]
        public ActionResult Create(Piece piece)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item list
                    piece.SellingOptionList = Utilities.GetSellingOptions();
                    return View("New", piece);
                }

                // add bikes if any
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (piece.BikeCheckBoxesList != null)
                {
                    selectedCheckBoxes = piece.BikeCheckBoxesList.Where(b => b.Checked).ToList();
                }

                piece.Bikes = new List<Bike>();
                for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                {
                    Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                    if (bike != null)
                    {
                        piece.Bikes.Add(bike);
                    }
                }

                // add info in db
                ctx.Pieces.Add(piece);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(piece);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Piece piece = ctx.Pieces.Find(id);
                if (piece != null)
                {
                    // reload select item lists and check boxes lists
                    piece.SellingOptionList = Utilities.GetSellingOptions();
                    piece.BikeCheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);

                    // mark selected dropdown items       


                    // mark selected bikes
                    piece.BikeCheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
                    foreach (Bike checkedBike in piece.Bikes)
                    {
                        piece.BikeCheckBoxesList.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                    }

                    return View(piece);
                }

                return HttpNotFound("Nu s-a gasit piesa cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

        [HttpPost]
        public ActionResult Update(Piece updatedPiece)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item list
                    updatedPiece.SellingOptionList = Utilities.GetSellingOptions();
                    return View("Edit", updatedPiece);
                }

                // get selected bikes
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (updatedPiece.BikeCheckBoxesList != null)
                {
                    selectedCheckBoxes = updatedPiece.BikeCheckBoxesList.Where(b => b.Checked).ToList();
                }

                // update piece
                Piece piece = ctx.Pieces.Single(b => b.PieceId == updatedPiece.PieceId);
                if (piece == null)
                {
                    return HttpNotFound("Nu s-a gasit piesa cu id-ul " + updatedPiece.PieceId.ToString() + "!");
                }

                if (TryUpdateModel(piece))
                {
                    piece.Name = updatedPiece.Name;
                    piece.Description = updatedPiece.Description;
                    piece.FabricationDate = updatedPiece.FabricationDate;
                    piece.Quantity = updatedPiece.Quantity;
                    piece.Price = updatedPiece.Price;
                    piece.IsIndependent = updatedPiece.IsIndependent;

                    piece.Bikes.Clear();
                    piece.Bikes = new List<Bike>();

                    // add new bikes if any
                    for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                    {
                        Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                        if (bike != null)
                        {
                            piece.Bikes.Add(bike);
                        }
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                Console.WriteLine("Update-ul pentru pieceId = " + piece.PieceId + " nu a reusit");
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
                Piece piece = ctx.Pieces.Find(id);

                if (piece != null)
                {
                    ctx.Pieces.Remove(piece);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                return HttpNotFound("Nu s-a gasit piesa cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}