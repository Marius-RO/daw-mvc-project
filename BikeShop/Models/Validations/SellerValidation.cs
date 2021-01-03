using BikeShop.Config;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BikeShop.Models.Validations
{
    public class SellerValidation : ValidationAttribute
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Order order = (Order)validationContext.ObjectInstance;

            // get selected bikes and pieces
            List<CheckBoxModel<Bike>> selectedBikes = new List<CheckBoxModel<Bike>>();
            List<CheckBoxModel<Piece>> selectedPieces = new List<CheckBoxModel<Piece>>();
            if (order.BikesListCheckBoxes != null)
            {
                selectedBikes = order.BikesListCheckBoxes.Where(b => b.Checked).ToList();
            }
            if (order.PiecesListCheckBoxes != null)
            {
                selectedPieces = order.PiecesListCheckBoxes.Where(b => b.Checked).ToList();
            }


            // check if at least one bike or piece is selected
            if ((selectedBikes == null || selectedBikes.Count() == 0) &&
                (selectedPieces == null || selectedPieces.Count() == 0))
            {
                return new ValidationResult("Au fost selectate 0 produse. Selecteaza cel putin o bicicleta sau o piesa");
            }

            // if only one type of product is selected then that type is from one single seller
            // (because of BikeSellerValidation and PieceSellerValidation
            if (!(selectedBikes != null && selectedBikes.Count() > 0 && selectedPieces != null && selectedPieces.Count() > 0))
            {
                return ValidationResult.Success;
            }

            // check if bikes and pieces are from the same seller
            // here we know that bikes if any are from one single seller and pieces if any are from one single seller
            // (because of BikeSellerValidation and PieceSellerValidation)
            string bikeSeller = null;
            for (int i = 0; i < selectedBikes.Count(); i++)
            {
                Bike bike = ctx.Bikes.Find(selectedBikes[i].Id);
                if (bike != null)
                {
                    bikeSeller = bike.UserId;
                    break;
                }
            }

            string pieceSeller = null;
            for (int i = 0; i < selectedPieces.Count(); i++)
            {
                Piece piece = ctx.Pieces.Find(selectedPieces[i].Id);
                if (piece != null)
                {
                    pieceSeller = piece.UserId;
                    break;
                }
            }

            if (bikeSeller != pieceSeller)
            {
                return new ValidationResult("Bicicletele si piesele nu sunt de la acelasi vanzator");
            }

            return ValidationResult.Success;

        }
    }
}