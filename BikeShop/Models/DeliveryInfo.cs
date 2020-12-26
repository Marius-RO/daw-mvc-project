using System.ComponentModel.DataAnnotations;

namespace BikeShop.Models
{
    public class DeliveryInfo
    {
        [Key]
        public int DeliveryInfoId { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }

        public virtual Order Order { get; set; }
    }
}