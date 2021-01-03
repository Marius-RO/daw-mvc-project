using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using BikeShop.Config;
using BikeShop.Models.Validations;

namespace BikeShop.Models
{
    public class Bike
    {
        [Key]
        public int BikeId { get; set; }

        [Display(Name = "Nume bicicleta")]
        [Required(ErrorMessage = "Numele bicicletei nu a fost introdus")]
        [RegularExpression(@"^[ a-zA-Z0-9_-]{5,20}$", ErrorMessage = "Numele bicicletei trebuie sa contina intre 5 si 20 de caractere [ a-zA-Z0-9_-]")]
        public string Name { get; set; }

        [Display(Name = "Descriere")]
        [Required(ErrorMessage = "Descrierea nu a fost introdusa")]
        [MaxLength(350, ErrorMessage = "Descrierea nu poate fi mai mare 350 de caractere")]
        public string Description { get; set; }

        [Display(Name = "Data fabricatie")]
        [Required(ErrorMessage = "Data fabricatiei nu a fost selectata")]
        [DateValidation]
        public DateTime FabricationDate { get; set; }

        [Display(Name = "Cantitate")]
        [Range(1,100, ErrorMessage = "Cantitate incorecta (1,100)")]
        public int Quantity { get; set; }

        [Display(Name = "Pret")]
        [Range(1, 1000, ErrorMessage = "Pret incorect (1,1000)")]
        public float Price { get; set; }

        // one to many
        [Display(Name = "Categorie biciclist")]
        [Required(ErrorMessage = "Categoria de biciclist nu a fost selectata")]
        [ForeignKey("BikerType")]
        public int BikerTypeId { get; set; }
        public virtual BikerType BikerType { get; set; }

        // one to many
        [Display(Name = "Categorie bicicleta")]
        [Required(ErrorMessage = "Categoria de bicicleta nu a fost selectata")]
        [ForeignKey("BikeCategory")]
        public int BikeCategoryId { get; set; }
        public virtual BikeCategory BikeCategory { get; set; }

        // one to many
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // many-to-many
        public virtual ICollection<Piece> Pieces { get; set; }
        // many-to-many
        public virtual ICollection<Order> Orders { get; set; }

        // used in views
        [NotMapped]
        public List<CheckBoxModel<Piece>> PiecesListCheckBoxes { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> BikerTypeList { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> BikeCategoryList { get; set; }
    }
}