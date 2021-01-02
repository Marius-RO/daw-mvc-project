using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.Models
{
    public class DeliveryInfo
    {
        [Key]
        public int DeliveryInfoId { get; set; }

        [Display(Name = "CNP")]
        public string CNP { get; set; }

        [Display(Name = "Nume")]
        [Required(ErrorMessage = "Numele nu a fost introdus")]
        [RegularExpression(@"^[ a-zA-Z0-9_-]{2,50}$", ErrorMessage = "Numele trebuie sa contina intre 2 si 50 de caractere [ a-zA-Z0-9_-]")]
        public string Name { get; set; }

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "Numarul de telefon nu a fost introdus")]
        [RegularExpression(@"^07(\d{8})$", ErrorMessage = "Acesta nu este un numar de telefon valid!")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Adresa de livrare")]
        [Required(ErrorMessage = "Adresa de livrare nu a fost introdusa")]
        [MaxLength(200, ErrorMessage = "Adresa de livrare nu poate fi mai mare 200 de caractere")]
        public string Address { get; set; }

        public virtual Order Order { get; set; }
    }
}