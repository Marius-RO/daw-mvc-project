using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BikeShop.Models.Validations
{
    public class BikeSellerValidation : ValidationAttribute
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Order order = (Order)validationContext.ObjectInstance;

            if (order.BikesListCheckBoxes == null)
            {
               return ValidationResult.Success;
            }

            // check if selected bikes (if any) are from the same seller
            List<CheckBoxModel<Bike>> selectedBikes = new List<CheckBoxModel<Bike>>();
            selectedBikes = order.BikesListCheckBoxes.Where(b => b.Checked).ToList();
            
            HashSet<int> sellers = new HashSet<int>();
            for (int i = 0; i < selectedBikes.Count(); i++)
            {
                Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                if (bike != null)
                {
                    sellers.Add(bike.UserId);
                    if(sellers.Count() > 1)
                    {
                        new ValidationResult("Bicicletele nu sunt de la acelasi vanzator");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}