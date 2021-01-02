using BikeShop.Models.Validari;
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

        [Required]
        public int SellerId { get; set; }

        [Required]
        public string OrderDate { get; set; }

        [Required]
        public float OrderValue { get; set; }
        

        // one-to-one
        [Required]
        public virtual DeliveryInfo DeliveryInfo { get; set; }

        // one-to-many
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // many-to-many
        public virtual ICollection<Bike> Bikes { get; set; }
        public virtual ICollection<Piece> Pieces { get; set; }

        // used in views
        [NotMapped]
        [BikeSellerValidation]
        //[SellerValidation]
        public List<CheckBoxModel<Bike>> BikesListCheckBoxes { get; set; }
        [NotMapped]
        public List<CheckBoxModel<Piece>> PiecesListCheckBoxes { get; set; }

    }
}