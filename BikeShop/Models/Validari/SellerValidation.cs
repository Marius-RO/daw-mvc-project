using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BikeShop.Models.Validari
{
    public class SellerValidation : ValidationAttribute
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Order order = (Order)validationContext.ObjectInstance;

            // check if at least one bike or piece is selected
            if (order.BikesListCheckBoxes == null && order.PiecesListCheckBoxes == null)
            {
                return new ValidationResult("Nu a fost selectat nici un produs");
            }

            // if only one type of product is selected then that type is from one single seller
            // (because of BikeSellerValidation and PieceSellerValidation)
            if (order.BikesListCheckBoxes == null || order.PiecesListCheckBoxes == null)
            {
                return ValidationResult.Success;
            }

            // check if bikes and pieces are from the same seller
            // here we know that bikes if any are from one seller and pieces if any are from one seller
            // (because of BikeSellerValidation and PieceSellerValidation)
            int sellerId = 0;
            List<CheckBoxModel<Bike>> selectedBikes = new List<CheckBoxModel<Bike>>();
            List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();

            // if only one type of product is selected then that type is from one single seller
            // (because of BikeSellerValidation and PieceSellerValidation)
            if (selectedBikes.Count() == 0 || selectedPieces.Count() == 0)
            {
                return ValidationResult.Success;
            }


            if (order.BikesListCheckBoxes != null)
            {
                selectedBikes = order.BikesListCheckBoxes.Where(b => b.Checked).ToList();
            }
            for (int i = 0; i < selectedBikes.Count(); i++)
            {
                Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                if (bike != null)
                {
                    sellerId = bike.UserId;
                    break;
                }
            }


            if (order.PiecesListCheckBoxes != null)
            {
                selectedPieces = order.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
            }
            for (int i = 0; i < selectedPieces.Count(); i++)
            {
                Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                if (piece != null)
                {
                    if(sellerId != piece.UserId)
                    {
                        return new ValidationResult("Bicicletele si piesele nu sunt de la acelasi vanzator");
                    }
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Success;

        }
    }
}