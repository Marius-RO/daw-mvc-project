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
    public class BikeController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            List<Bike> bikes;

            // seller see`s only his bikes
            if (User.IsInRole(Utilities.ROLE_SELLER))
            {
                bikes = ctx.Bikes.Where(b => b.UserId == userId).ToList();
                return View(bikes);
            }

            // is admin, client or anonym
            bikes = ctx.Bikes.ToList();
            return View(bikes);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Bike bike = ctx.Bikes.Find(id);
                if (bike != null)
                {
                    // seller can see only his bikes
                    if (User.IsInRole(Utilities.ROLE_SELLER) && bike.UserId != User.Identity.GetUserId())
                    {
                        return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
                    }

                    return View(bike);
                }

                return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");

        }

        [HttpGet]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult New()
        {
            Bike bike = new Bike();
            bike.UserId = User.Identity.GetUserId();
            bike.BikerTypeList = Utilities.GetAllBikerTypes(ctx, userId: bike.UserId);
            bike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx, userId: bike.UserId);
            bike.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx, userId: bike.UserId);
            return View(bike);
        }

        [HttpPost]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Create(Bike bike)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item lists
                    bike.BikerTypeList = Utilities.GetAllBikerTypes(ctx, userId: bike.UserId);
                    bike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx, userId: bike.UserId);
                    return View(Utilities.VIEW_NEW, bike);
                }

                bike.ImagePath = Utilities.BIKES_IMAGES_PATH + bike.ImagePath;

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
                return RedirectToAction(Utilities.ACTION_INDEX);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(Utilities.VIEW_NEW, bike);
            }
        }

        [HttpGet]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Bike bike = ctx.Bikes.Find(id);
                if (bike != null && (bike.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {

                    // reload select item lists and check boxes lists
                    bike.BikerTypeList = Utilities.GetAllBikerTypes(ctx, userId: bike.UserId);
                    bike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx, userId: bike.UserId);
                    bike.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx, userId: bike.UserId);

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

        [HttpPut]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Update(Bike updatedBike)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload select item lists
                    updatedBike.BikerTypeList = Utilities.GetAllBikerTypes(ctx, userId: updatedBike.UserId);
                    updatedBike.BikeCategoryList = Utilities.GetAllBikeCategories(ctx, userId: updatedBike.UserId);
                    return View(Utilities.VIEW_EDIT, updatedBike);
                }

                // get selected pieces
                List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
                if (updatedBike.PiecesListCheckBoxes != null)
                {
                    selectedPieces = updatedBike.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                // update bike
                Bike bike = ctx.Bikes.Single(b => b.BikeId == updatedBike.BikeId);
                if (bike == null || !(bike.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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
                    if (updatedBike.NewImagePath != null)
                    {
                        bike.ImagePath = Utilities.BIKES_IMAGES_PATH + updatedBike.NewImagePath;
                    }
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
                    return RedirectToAction(Utilities.ACTION_INDEX);
                }

                Console.WriteLine("Update-ul pentru bikeId = " + bike.BikeId + " nu a reusit");
                return RedirectToAction(Utilities.ACTION_INDEX);

            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(Utilities.VIEW_EDIT, updatedBike);
            }

        }


        [HttpPost]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                Bike bike = ctx.Bikes.Find(id);
                if (bike != null && (bike.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    ctx.Bikes.Remove(bike);
                    ctx.SaveChanges();
                    return RedirectToAction(Utilities.ACTION_INDEX);
                }

                return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}