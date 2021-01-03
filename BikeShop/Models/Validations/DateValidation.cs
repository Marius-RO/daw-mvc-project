using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BikeShop.Models.Validations
{
    public class DateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            DateTime date = DateTime.Now;

            if (validationContext.ObjectInstance.GetType() == typeof(Bike))
            {
                date = ((Bike)validationContext.ObjectInstance).FabricationDate;
            }
            else if (validationContext.ObjectInstance.GetType() == typeof(Piece))
            {
                date = ((Piece)validationContext.ObjectInstance).FabricationDate;
            }

            if(date > DateTime.Now)
            {
                return new ValidationResult("Data de fabricatie nu poate fi in viitor");
            }

            return ValidationResult.Success;
        }
    }
}