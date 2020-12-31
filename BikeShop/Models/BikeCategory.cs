using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.Models
{
    public class BikeCategory : Category
    {
        // many-to-one
        public virtual ICollection<Bike> Bikes { get; set; }

        // used in views
        [NotMapped]
        public List<CheckBoxModel<Bike>> CheckBoxesList { get; set; }
    }
}