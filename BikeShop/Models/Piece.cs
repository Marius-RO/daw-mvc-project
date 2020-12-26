using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeShop.Models
{
    public class Piece
    {
        [Key]
        public int PieceId { get; set; }
        public float Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime FabricationDate { get; set; }
        public int Quantity { get; set; }
        public Boolean isIndependent { get; set; }

        public virtual ICollection<Bike> Bikes { get; set; }

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}