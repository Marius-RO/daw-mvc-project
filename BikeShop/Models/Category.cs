using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.Models
{
    public abstract class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Nume categorie")]
        [Required(ErrorMessage = "Numele categoriei nu a fost introdus")]
        [RegularExpression(@"^[ a-zA-Z0-9_-]{5,20}$",ErrorMessage = "Numele categoriei trebuie sa contina intre 5 si 20 de caractere [ a-zA-Z0-9_-]")]
        public string Name { get; set; }

        [Display(Name = "Descrierea categoriei")]
        [Required(ErrorMessage = "Descrierea nu a fost introdusa")]
        [MaxLength(350, ErrorMessage = "Descrierea nu poate fi mai mare 350 de caractere")]
        public string Description { get; set; }

    }
}