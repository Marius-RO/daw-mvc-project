using BikeShop.Config;
using BikeShop.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BikeShop.Controllers
{

    [Authorize(Roles = Utilities.ROLE_ADMIN + "," + Utilities.ROLE_SELLER)]
    public class BikerTypeController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            List<BikerType> types;

            if (User.IsInRole(Utilities.ROLE_ADMIN))
            {
                types = ctx.BikerTypes.ToList();
                return View(types);
            }

            // seller role
            types = ctx.BikerTypes.Where(b => b.UserId == userId).ToList();
            return View(types);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                BikerType type = ctx.BikerTypes.Find(id);
                if (type != null && (type.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    return View(type);
                }

                return HttpNotFound("Nu s-a gasit categoria de biciclist cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

        [HttpGet]
        public ActionResult New()
        {
            BikerType type = new BikerType();
            type.UserId = User.Identity.GetUserId();
            type.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx, userId: type.UserId);
            return View(type);
        }

        [HttpPost]
        public ActionResult Create(BikerType type)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload checkboxes
                    type.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx, userId: type.UserId);
                    return View(Utilities.VIEW_NEW, type);
                }

                // add bikes if any
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (type.CheckBoxesList != null)
                {
                    selectedCheckBoxes = type.CheckBoxesList.Where(b => b.Checked).ToList();
                }

                type.Bikes = new List<Bike>();
                for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                {
                    Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                    if (bike != null)
                    {
                        type.Bikes.Add(bike);
                    }
                }

                // add info in db
                ctx.BikerTypes.Add(type);
                ctx.SaveChanges();
                return RedirectToAction(Utilities.ACTION_INDEX);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(Utilities.VIEW_NEW, type);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                BikerType type = ctx.BikerTypes.Find(id);
                if (type != null && (type.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    // reload checkboxes
                    type.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx, userId: type.UserId);
            
                    // mark selected bikes
                    foreach (Bike checkedBike in type.Bikes)
                    {
                        type.CheckBoxesList.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                    }

                    return View(type);
                }

                return HttpNotFound("Nu s-a gasit categoria de biciclist cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

        [HttpPut]
        public ActionResult Update(BikerType updatedType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload selected bikes
                    BikerType tmp = ctx.BikerTypes.Find(updatedType.BikerTypeId);
                    if (tmp != null && (tmp.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                    {
                        updatedType.Bikes = tmp.Bikes;
                        updatedType.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx, userId: tmp.UserId);
                        
                        foreach (Bike checkedBike in updatedType.Bikes)
                        {
                            updatedType.CheckBoxesList.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                        }
                    }

                    return View(Utilities.VIEW_EDIT, updatedType);
                }

                // get selected bikes
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (updatedType.CheckBoxesList != null)
                {
                    selectedCheckBoxes = updatedType.CheckBoxesList.Where(b => b.Checked).ToList();
                }

                // update BikerType
                BikerType type = ctx.BikerTypes.Single(b => b.BikerTypeId == updatedType.BikerTypeId);
                if (type == null || !(type.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    return HttpNotFound("Nu s-a gasit categoria de biciclist cu id-ul " + updatedType.BikerTypeId.ToString() + "!");
                }

                if (TryUpdateModel(type))
                {
                    type.Name = updatedType.Name;
                    type.Description = updatedType.Description;

                    // add bikes
                    type.Bikes.Clear();
                    for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                    {
                        Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                        if (bike != null)
                        {
                            type.Bikes.Add(bike);
                        }
                    }
                    
                    ctx.SaveChanges();
                    return RedirectToAction(Utilities.ACTION_INDEX);
                }

                Console.WriteLine("Update-ul pentru bikerTypeId = " + type.BikerTypeId + " nu a reusit");
                return RedirectToAction(Utilities.ACTION_INDEX);

            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(Utilities.VIEW_EDIT, updatedType);
            }
        }


        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                BikerType type = ctx.BikerTypes.Find(id);

                if (type != null && (type.UserId == User.Identity.GetUserId() || User.IsInRole(Utilities.ROLE_ADMIN)))
                {
                    ctx.BikerTypes.Remove(type);
                    ctx.SaveChanges();
                    return RedirectToAction(Utilities.ACTION_INDEX);
                }

                return HttpNotFound("Nu s-a gasit categoria de biciclist cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}