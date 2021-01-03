using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BikeShop.Models.Validations
{
    public class PieceSellerValidation : ValidationAttribute
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Order order = (Order)validationContext.ObjectInstance;

            List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
            if (order.PiecesListCheckBoxes != null)
            {
                selectedPieces = order.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
            }

            // check if selected bikes (if any) are from the same seller
            HashSet<string> sellers = new HashSet<string>();
            for (int i = 0; i < selectedPieces.Count(); i++)
            {
                Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                if (piece != null)
                {
                    sellers.Add(piece.UserId);
                    if (sellers.Count() > 1)
                    {
                        return new ValidationResult("Piesele nu sunt de la acelasi vanzator");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}