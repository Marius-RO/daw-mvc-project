using BikeShop.Config;
using BikeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BikeShop.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<Order> orders = ctx.Orders.ToList();
            return View(orders);
        }

        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Order order = ctx.Orders.Find(id);
                if (order != null)
                {
                    return View(order);
                }

                return HttpNotFound("Nu s-a gasit comanda cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");

        }


        public ActionResult New()
        {
            Order order = new Order();
            order.DeliveryInfo = new DeliveryInfo();
            order.BikesListCheckBoxes = Utilities.GetAllBikeCheckBoxes(ctx);
            order.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);
            return View(order);
        }

        [HttpPost]
        public ActionResult Create(Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload checkboxes
                    order.BikesListCheckBoxes = Utilities.GetAllBikeCheckBoxes(ctx);
                    order.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);
                    return View("New", order);
                }

                // add delivery info to db
                ctx.DeliveryInfos.Add(order.DeliveryInfo);

                // add bikes if any
                List<CheckBoxModel<Bike>> selectedBikes = new List<CheckBoxModel<Bike>>();
                if (order.BikesListCheckBoxes != null)
                {
                    selectedBikes = order.BikesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                order.Bikes = new List<Bike>();
                for (int i = 0; i < selectedBikes.Count(); i++)
                {
                    Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                    if (bike != null)
                    {
                        order.Bikes.Add(bike);
                    }
                }

                // add pieces if any
                List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
                if (order.PiecesListCheckBoxes != null)
                {
                    selectedPieces = order.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                order.Pieces = new List<Piece>();
                for (int i = 0; i < selectedPieces.Count(); i++)
                {
                    Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                    if (piece != null)
                    {
                        order.Pieces.Add(piece);
                    }
                }

                ctx.Orders.Add(order);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View("New",order);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Order order = ctx.Orders.Find(id);
                if (order != null)
                {
                    // reload check boxes lists
                    order.BikesListCheckBoxes = Utilities.GetAllBikeCheckBoxes(ctx);
                    order.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);

                    // mark selected bikes
                    foreach (Bike checkedBike in order.Bikes)
                    {
                        order.BikesListCheckBoxes.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                    }

                    // mark selected pieces
                    foreach (Piece checkedPiece in order.Pieces)
                    {
                        order.PiecesListCheckBoxes.FirstOrDefault(c => c.Id == checkedPiece.PieceId).Checked = true;
                    }

                    return View(order);
                }

                return HttpNotFound("Nu s-a gasit comanda cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }
        
        [HttpPut]
        public ActionResult Update(Order updatedOrder)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload selected bikes and pieces
                    Order tmp = ctx.Orders.Find(updatedOrder.OrderId);
                    if (tmp != null)
                    {
                        updatedOrder.Bikes = tmp.Bikes;
                        updatedOrder.BikesListCheckBoxes = Utilities.GetAllBikeCheckBoxes(ctx);
                        foreach (Bike checkedBike in updatedOrder.Bikes)
                        {
                            updatedOrder.BikesListCheckBoxes.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                        }

                        updatedOrder.Pieces = tmp.Pieces;
                        updatedOrder.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);
                        foreach (Piece checkedPiece in updatedOrder.Pieces)
                        {
                            updatedOrder.PiecesListCheckBoxes.FirstOrDefault(c => c.Id == checkedPiece.PieceId).Checked = true;
                        }
                    }

                    return View("Edit", updatedOrder);
                }

                // get selected bikes
                List<CheckBoxModel<Bike>> selectedBikes = new List<CheckBoxModel<Bike>>();
                if (updatedOrder.BikesListCheckBoxes != null)
                {
                    selectedBikes = updatedOrder.BikesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                // get selected pieces
                List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
                if (updatedOrder.PiecesListCheckBoxes != null)
                {
                    selectedPieces = updatedOrder.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
                }

                // update order an delivery info
                Order order = ctx.Orders.Single(b => b.OrderId == updatedOrder.OrderId);
                if (order == null)
                {
                    return HttpNotFound("Nu s-a gasit comanda cu id-ul " + updatedOrder.OrderId.ToString() + "!");
                }

                DeliveryInfo deliveryInfo = ctx.DeliveryInfos.Single(b => b.DeliveryInfoId == order.DeliveryInfo.DeliveryInfoId);
                if (deliveryInfo == null)
                {
                    return HttpNotFound("Nu s-a informatia de livrare pentru comanda id-ul " +
                        order.DeliveryInfo.DeliveryInfoId.ToString() + "!");
                }

                if (TryUpdateModel(deliveryInfo) && TryUpdateModel(order))
                {
                    deliveryInfo.Name = updatedOrder.DeliveryInfo.Name;
                    deliveryInfo.CNP = updatedOrder.DeliveryInfo.CNP;
                    deliveryInfo.PhoneNumber = updatedOrder.DeliveryInfo.PhoneNumber;
                    deliveryInfo.Address = updatedOrder.DeliveryInfo.Address;
                    ctx.SaveChanges();

                    
                    order.SellerId = updatedOrder.SellerId;
                    order.OrderDate = updatedOrder.OrderDate;
                    order.OrderValue = updatedOrder.OrderValue;
                    order.DeliveryInfo = deliveryInfo;
                    order.Bikes.Clear();
                    order.Bikes = new List<Bike>();
                    order.Pieces.Clear();
                    order.Pieces = new List<Piece>();


                    // add new bikes if any
                    for (int i = 0; i < selectedBikes.Count(); i++)
                    {
                        Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                        if (bike != null)
                        {
                            order.Bikes.Add(bike);
                        }
                    }

                    // add new pieces if any
                    for (int i = 0; i < selectedPieces.Count(); i++)
                    {
                        Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                        if (piece != null)
                        {
                            order.Pieces.Add(piece);
                        }
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");

                }

                Console.WriteLine("Update-ul pentru comanda cu id-ul = " + updatedOrder.OrderId + " nu a reusit");
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View("Edit", updatedOrder);
            }

        }
        
       
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                Order order = ctx.Orders.Find(id);
                if (order != null)
                {
                    DeliveryInfo deliveryInfo = ctx.DeliveryInfos.Find(order.DeliveryInfo.DeliveryInfoId);

                    if(deliveryInfo != null)
                    {
                        ctx.Orders.Remove(order);
                        ctx.DeliveryInfos.Remove(deliveryInfo);
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    return HttpNotFound("Nu s-a gasit informatia de livrare cu id-ul " + order.DeliveryInfo.DeliveryInfoId.ToString() + "!");
                }

                return HttpNotFound("Nu s-a gasit bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}