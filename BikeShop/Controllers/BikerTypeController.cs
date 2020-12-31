using BikeShop.Config;
using BikeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BikeShop.Controllers
{
    public class BikerTypeController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            List<BikerType> types = ctx.BikerTypes.ToList();
            return View(types);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                BikerType type = ctx.BikerTypes.Find(id);

                if (type != null)
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
            type.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
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
                    type.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
                    return View("New", type);
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
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View(type);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                BikerType type = ctx.BikerTypes.Find(id);
                if (type != null)
                {
                    // mark selected bikes
                    type.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
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

        [HttpPost]
        public ActionResult Update(BikerType updatedType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload selected bikes
                    BikerType tmp = ctx.BikerTypes.Find(updatedType.BikerTypeId);
                    if (tmp != null)
                    {
                        updatedType.Bikes = tmp.Bikes;
                        updatedType.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
                        foreach (Bike checkedBike in updatedType.Bikes)
                        {
                            updatedType.CheckBoxesList.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                        }
                    }

                    return View("Edit", updatedType);
                }

                // get selected bikes
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (updatedType.CheckBoxesList != null)
                {
                    selectedCheckBoxes = updatedType.CheckBoxesList.Where(b => b.Checked).ToList();
                }

                // update BikerType
                BikerType type = ctx.BikerTypes.Single(b => b.BikerTypeId == updatedType.BikerTypeId);
                if (type == null)
                {
                    return HttpNotFound("Nu s-a gasit categoria de biciclist cu id-ul " + updatedType.BikerTypeId.ToString() + "!");
                }

                if (TryUpdateModel(type))
                {
                    type.Name = updatedType.Name;
                    type.Description = updatedType.Description;
                    type.Bikes.Clear();
                    type.Bikes = new List<Bike>();

                    // add new bikes if any
                    for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                    {
                        Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                        if (bike != null)
                        {
                            type.Bikes.Add(bike);
                        }
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                Console.WriteLine("Update-ul pentru bikerTypeId = " + type.BikerTypeId + " nu a reusit");
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
                BikerType type = ctx.BikerTypes.Find(id);

                if (type != null)
                {
                    ctx.BikerTypes.Remove(type);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                return HttpNotFound("Nu s-a gasit categoria de biciclist cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}