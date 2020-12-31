using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.Models
{
    public class BikerType
    {
        [Key]
        public int BikerTypeId { get; set; }

        [Display(Name = "Nume categorie biciclist")]
        [Required(ErrorMessage = "Numele categoriei de biciclist nu a fost introdus")]
        [RegularExpression(@"^[ a-zA-Z0-9_-]{5,20}$", ErrorMessage = "Numele categoriei de biciclist trebuie sa contina intre 5 si 20 de caractere [ a-zA-Z0-9_-]")]
        public string Name { get; set; }

        [Display(Name = "Descriere")]
        [Required(ErrorMessage = "Descrierea nu a fost introdusa")]
        [MaxLength(350, ErrorMessage = "Descrierea nu poate fi mai mare 350 de caractere")]
        public string Description { get; set; }

        // many-to-one
        public virtual ICollection<Bike> Bikes { get; set; }

        // used in views
        [NotMapped]
        public List<CheckBoxModel<Bike>> CheckBoxesList { get; set; }
    }
}