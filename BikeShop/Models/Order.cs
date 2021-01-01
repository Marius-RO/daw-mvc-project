using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BikeShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public virtual ICollection<Bike> Bikes { get; set; }
        public virtual ICollection<Piece> Pieces { get; set; }

        [Required]
        public virtual DeliveryInfo DeliveryInfo { get; set; }

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}