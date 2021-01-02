using BikeShop.Config;
using BikeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BikeShop.Controllers
{
    public class BikeCategoryController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            List<BikeCategory> categories = ctx.BikeCategories.ToList();
            return View(categories);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                BikeCategory category = ctx.BikeCategories.Find(id);

                if (category != null)
                {
                    return View(category);
                }

                return HttpNotFound("Nu s-a gasit categoria de bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

        [HttpGet]
        public ActionResult New()
        {
            BikeCategory category = new BikeCategory();
            category.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
            return View(category);
        }

        [HttpPost]
        public ActionResult Create(BikeCategory category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload checkboxes
                    category.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
                    return View("New", category);
                }

                // add bikes if any
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (category.CheckBoxesList != null)
                {
                    selectedCheckBoxes = category.CheckBoxesList.Where(b => b.Checked).ToList();
                }

                category.Bikes = new List<Bike>();
                for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                {
                    Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                    if (bike != null)
                    {
                        category.Bikes.Add(bike);
                    }
                }

                // add info in db
                ctx.BikeCategories.Add(category);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View("New", category);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                BikeCategory category = ctx.BikeCategories.Find(id);
                if (category != null)
                {
                    // mark selected bikes
                    category.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
                    foreach (Bike checkedBike in category.Bikes)
                    {
                        category.CheckBoxesList.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                    }

                    return View(category);
                }

                return HttpNotFound("Nu s-a gasit categoria de bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

        [HttpPut]
        public ActionResult Update(BikeCategory updatedCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // reload selected bikes
                    BikeCategory tmp = ctx.BikeCategories.Find(updatedCategory.CategoryId);
                    if (tmp != null)
                    {
                        updatedCategory.Bikes = tmp.Bikes;
                        updatedCategory.CheckBoxesList = Utilities.GetAllBikeCheckBoxes(ctx);
                        foreach (Bike checkedBike in updatedCategory.Bikes)
                        {
                            updatedCategory.CheckBoxesList.FirstOrDefault(c => c.Id == checkedBike.BikeId).Checked = true;
                        }
                    }

                    return View("Edit", updatedCategory);
                }

                // get selected bikes
                List<CheckBoxModel<Bike>> selectedCheckBoxes = new List<CheckBoxModel<Bike>>();
                if (updatedCategory.CheckBoxesList != null)
                {
                    selectedCheckBoxes = updatedCategory.CheckBoxesList.Where(b => b.Checked).ToList();
                }

                // update BikerType
                BikeCategory category = ctx.BikeCategories.Single(b => b.CategoryId == updatedCategory.CategoryId);
                if (category == null)
                {
                    return HttpNotFound("Nu s-a gasit categoria de bicicleta cu id-ul " + updatedCategory.CategoryId.ToString() + "!");
                }

                if (TryUpdateModel(category))
                {
                    category.Name = updatedCategory.Name;
                    category.Description = updatedCategory.Description;
                    category.Bikes.Clear();
                    category.Bikes = new List<Bike>();

                    // add new bikes if any
                    for (int i = 0; i < selectedCheckBoxes.Count(); i++)
                    {
                        Bike bike = ctx.Bikes.Find(selectedCheckBoxes[i].Id);
                        if (bike != null)
                        {
                            category.Bikes.Add(bike);
                        }
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                Console.WriteLine("Update-ul pentru bikeCategoryId = " + updatedCategory.CategoryId + " nu a reusit");
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine(e.Message);
                return View("Edit", updatedCategory);
            }
        }


        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                BikeCategory category = ctx.BikeCategories.Find(id);

                if (category != null)
                {
                    ctx.BikeCategories.Remove(category);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                return HttpNotFound("Nu s-a gasit categoria de bicicleta cu id-ul " + id.ToString() + "!");
            }

            return HttpNotFound("Lipseste parametrul id!");
        }

    }
}