using BikeShop.Config;
using BikeShop.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BikeShop.Controllers
{
    [Authorize]
    public class PieceController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {

            string userId = User.Identity.GetUserId();
            List<Piece> pieces;

            // seller see`s only his pieces
            if (User.IsInRole(Utilities.ROLE_SELLER))
            {
                pieces = ctx.Pieces.Where(b => b.UserId == userId).ToList();
                return View(pieces);
            }

            // admin see`s everything
            if (User.IsInRole(Utilities.ROLE_ADMIN))
            {
                pieces = ctx.Pieces.ToList();
                return View(pieces);
            }

            // client or anonymus see only pieces for sale
            pieces = ctx.Pieces.Where(b => b.IsIndependent == true).ToList();
            return View(pieces);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Piece piece = ctx.Pieces.Find(id);
                if (piece != null)
                {
                    // seller can see only his piece
                    if (User.IsInRole(Utilities.ROLE_SELLER) && piece.UserId != User.Identity.GetUserId())
                    {
                        return HttpNotFound("Nu s-a gasit piesa cu id-ul " + id.ToString() + "!");
                    }

                    return View(piece);
                }

                return HttpNotFound("Nu s-a gasit piesa cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");

        }

        [HttpGet]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult New()
        {
            Piece piece = new Piece();
            piece.UserId = User.Identity.GetUserId();
            piece.SellingOptionList = Utilities.GetBasicOptions();
            piece.AccessoryOptionList = Utilities.GetBasicOptions();
            piece.BikeCheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx, userId: piece.UserId);
            return View(piece);
        }

        [HttpPost]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Create(Piece piece)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item list
                    piece.SellingOptionList = Utilities.GetBasicOptions();
                    piece.AccessoryOptionList = Utilities.GetBasicOptions();
                    return View("New", piece);
                }

                piece.ImagePath = Utilities.PIECES_IMAGES_PATH + piece.ImagePath;

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
                return View("New", piece);
            }
        }

        [HttpGet]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Piece piece = ctx.Pieces.Find(id);
                if (piece != null && (piece.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    // reload select item lists and check boxes lists
                    piece.SellingOptionList = Utilities.GetBasicOptions();
                    piece.AccessoryOptionList = Utilities.GetBasicOptions();
                    piece.BikeCheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx, userId: piece.UserId);
                    
                    // mark selected bikes
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

        [HttpPut]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Update(Piece updatedPiece)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item list
                    updatedPiece.SellingOptionList = Utilities.GetBasicOptions();
                    updatedPiece.AccessoryOptionList = Utilities.GetBasicOptions();
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
                if (piece == null || !(piece.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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
                    if (updatedPiece.NewImagePath != null)
                    {
                        piece.ImagePath = Utilities.PIECES_IMAGES_PATH + updatedPiece.NewImagePath;
                    }
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
                return View("Edit", updatedPiece);
            }

        }


        [HttpPost]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                Piece piece = ctx.Pieces.Find(id);

                if (piece != null && (piece.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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