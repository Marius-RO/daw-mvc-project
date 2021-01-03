using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BikeShop.Models.Validations
{
    public class CustomEmailValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RegisterViewModel model = (RegisterViewModel)validationContext.ObjectInstance;
            Regex regex = new Regex(@"^[a-zA-Z0-9_.-]{2,20}[@](gmail.com|yahoo.com)$");
            Match match = regex.Match(model.Email);
            if (match.Success)
                return ValidationResult.Success;

            return new ValidationResult("Adresa de email invalida. Trebuie sa fie de forma [{2,20}@yahoo.com] sau [{2,20}@gmail.com]");
        }
    }
}