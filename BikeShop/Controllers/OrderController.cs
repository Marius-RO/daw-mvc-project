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
    public class OrderController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            List<Order> orders;

            if (User.IsInRole(Utilities.ROLE_ADMIN))
            {
                orders = ctx.Orders.ToList();
                return View(orders);
            }

            if (User.IsInRole(Utilities.ROLE_SELLER))
            {
                orders = ctx.Orders.Where(b => b.SellerId == userId).ToList();
                return View(orders);
            }

            // client role
            orders = ctx.Orders.Where(b => b.UserId == userId).ToList();
            return View(orders);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Order order = ctx.Orders.Find(id);
                if (order != null && (order.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    return View(order);
                }

                return HttpNotFound("Nu s-a gasit comanda cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");

        }

        [HttpGet]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_CLIENT)]
        public ActionResult New()
        {
            Order order = new Order();
            order.UserId = User.Identity.GetUserId();
            order.DeliveryInfo = new DeliveryInfo();
            order.BikesListCheckBoxes = Utilities.GetAllBikeCheckBoxes(ctx);
            order.PiecesListCheckBoxes = Utilities.GetAllPiecesCheckBoxes(ctx);
 
            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_CLIENT)]
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

                float orderValue = 0.0f;
                string sellerId = null;

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
                        orderValue += bike.Price;

                        if (sellerId != null)
                        {
                            sellerId = bike.UserId;
                        }
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
                        orderValue += piece.Price;

                        if (sellerId != null)
                        {
                            sellerId = piece.UserId;
                        }
                    }
                }

                // add order info
                order.SellerId = sellerId;
                order.OrderValue = orderValue;
                order.OrderDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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

        [HttpGet]
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Order order = ctx.Orders.Find(id);
                if (order != null && (order.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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
        [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
        public ActionResult Update(Order updatedOrder)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload selected bikes and pieces
                    Order tmp = ctx.Orders.Find(updatedOrder.OrderId);
                    if (tmp != null && (tmp.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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
                if (order == null || !(order.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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
                    deliveryInfo.PhoneNumber = updatedOrder.DeliveryInfo.PhoneNumber;
                    deliveryInfo.Address = updatedOrder.DeliveryInfo.Address;
                    ctx.SaveChanges();

                    order.OrderDate = updatedOrder.OrderDate;
                    order.DeliveryInfo = deliveryInfo;
                    order.Bikes.Clear();
                    order.Bikes = new List<Bike>();
                    order.Pieces.Clear();
                    order.Pieces = new List<Piece>();

                    float orderValue = 0.0f;
                    string sellerId = null;

                    // add new bikes if any
                    for (int i = 0; i < selectedBikes.Count(); i++)
                    {
                        Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                        if (bike != null )
                        {
                            order.Bikes.Add(bike);
                            orderValue += bike.Price;

                            if(sellerId != null)
                            {
                                sellerId = bike.UserId;
                            }
                        }
                    }

                    // add new pieces if any
                    for (int i = 0; i < selectedPieces.Count(); i++)
                    {
                        Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                        if (piece != null)
                        {
                            order.Pieces.Add(piece);
                            orderValue += piece.Price;

                            if (sellerId != null)
                            {
                                sellerId = piece.UserId;
                            }
                        }
                    }

                    order.SellerId = sellerId;
                    order.OrderValue = orderValue;

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
        [Authorize(Roles = Utilities.ROLE_ADMIN)]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                Order order = ctx.Orders.Find(id);
                if (order != null && (order.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
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

                return HttpNotFound("Nu s-a gasit comanda cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}