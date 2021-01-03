using BikeShop.Models.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BikeShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }


        public string SellerId { get; set; }

        [MaxLength(30, ErrorMessage = "Formatul datei este prea lung")]
        public string OrderDate { get; set; }

        [Range(1, 9000, ErrorMessage = "Valoare incorecta (1,9000)")]
        public float OrderValue { get; set; }
        
        // one-to-one
        [Required]
        public virtual DeliveryInfo DeliveryInfo { get; set; }

        // one-to-many
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // many-to-many
        public virtual ICollection<Bike> Bikes { get; set; }
        public virtual ICollection<Piece> Pieces { get; set; }

        // used in views
        [NotMapped]
        [BikeSellerValidation]
        [SellerValidation]
        public List<CheckBoxModel<Bike>> BikesListCheckBoxes { get; set; }

        [NotMapped]
        [PieceSellerValidation]
        [SellerValidation]
        public List<CheckBoxModel<Piece>> PiecesListCheckBoxes { get; set; }

    }
}