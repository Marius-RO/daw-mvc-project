using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BikeShop.Models.Validations
{
    public class BikeSellerValidation : ValidationAttribute
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Order order = (Order)validationContext.ObjectInstance;

            List<CheckBoxModel<Bike>> selectedBikes = new List<CheckBoxModel<Bike>>();
            if (order.BikesListCheckBoxes != null)
            {
                selectedBikes = order.BikesListCheckBoxes.Where(b => b.Checked).ToList();
            }

            // check if selected bikes (if any) are from the same seller
            HashSet<string> sellers = new HashSet<string>();
            for (int i = 0; i < selectedBikes.Count(); i++)
            {
                Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                if (bike != null)
                {
                    sellers.Add(bike.UserId);
                    if (sellers.Count() > 1)
                    {
                        return new ValidationResult("Bicicletele nu sunt de la acelasi vanzator");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}