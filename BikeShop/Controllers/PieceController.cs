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

        public ActionResult Details(int id)
        {
            Piece piece = ctx.Pieces.Find(id);
            return View(piece);
        }


        public ActionResult New()
        {
            Piece piece = new Piece();
            return View(piece);
        }

        [HttpPost]
        public ActionResult Create(Piece piece)
        {
            if (!ModelState.IsValid)
            {
                return View("New", piece);
            }

            ctx.Pieces.Add(piece);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Piece piece = ctx.Pieces.Find(id);
            return View(piece);
        }

        [HttpPost]
        public ActionResult Update(Piece updatedPiece)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", updatedPiece);
            }

            Piece piece = ctx.Pieces.Single(p => p.PieceId == updatedPiece.PieceId);
            piece.Name = updatedPiece.Name;
            piece.Description = updatedPiece.Description;
            piece.FabricationDate = updatedPiece.FabricationDate;
            piece.Price = updatedPiece.Price;
            piece.Quantity = updatedPiece.Quantity;
            piece.isIndependent = updatedPiece.isIndependent;

            ctx.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Piece piece = ctx.Pieces.Find(id);
            ctx.Pieces.Remove(piece);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}