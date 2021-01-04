using BikeShop.Models.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace BikeShop.Models
{
    public class Piece
    {
        [Key]
        public int PieceId { get; set; }

        [Display(Name = "Nume piesa")]
        [Required(ErrorMessage = "Numele piese nu a fost introdus")]
        [RegularExpression(@"^[ a-zA-Z0-9_-]{5,40}$", ErrorMessage = "Numele piesei trebuie sa contina intre 5 si 40 de caractere [ a-zA-Z0-9_-]")]
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
        [Range(1, 100, ErrorMessage = "Cantitate incorecta (1,100)")]
        public int Quantity { get; set; }

        [Display(Name = "Pret")]
        [Range(1, 1000, ErrorMessage = "Pret incorect (1,1000)")]
        public float Price { get; set; }

        [Display(Name = "Pentru vanzare")]
        [Required(ErrorMessage = "Selecteaza daca piesa este pentru vanzare sau nu")]
        public bool IsIndependent { get; set; }

        [Display(Name = "Accesoriu")]
        [Required(ErrorMessage = "Alege daca este accesoriu")]
        public bool IsAccessory { get; set; }

        [Display(Name = "Alege imagine")]
        public string ImagePath { get; set; }

        // many-to-many
        public virtual ICollection<Bike> Bikes { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        // one to many
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // used in views
        [NotMapped]
        public IEnumerable<SelectListItem> SellingOptionList { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> AccessoryOptionList { get; set; }
        [NotMapped]
        public List<CheckBoxModel<Bike>> BikeCheckBoxesList { get; set; }

        [NotMapped]
        [Display(Name = "Schima imaginea")]
        public string NewImagePath { get; set; }
    }
}