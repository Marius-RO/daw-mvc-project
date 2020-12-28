using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.Models
{
    public class Bike
    {
        [Key]
        public int BikeId { get; set; }

        [Display(Name = "Nume")]
        [Required(ErrorMessage = "Numele nu a fost introdus")]
        [MaxLength(20, ErrorMessage = "Numele nu poate fi mai mare 20 de caractere")]
        public string Name { get; set; }

        [Display(Name = "Descriere")]
        [Required(ErrorMessage = "Descrierea nu a fost introdusa")]
        [MaxLength(350, ErrorMessage = "Descrierea nu poate fi mai mare 350 de caractere")]
        public string Description { get; set; }

        [Display(Name = "Data fabricatie")]
        [Required(ErrorMessage = "Data fabricatiei nu a fost selectata")]
        public DateTime FabricationDate { get; set; }

        [Display(Name = "Cantitate")]
        [Range(1,100, ErrorMessage = "Cantitate incorecta (1,100)")]
        public int Quantity { get; set; }

        [Display(Name = "Pret")]
        [Range(1, 1000, ErrorMessage = "Pret incorect (1,1000)")]
        public float Price { get; set; }

        public virtual ICollection<Piece> Pieces { get; set; }
        public virtual ICollection<Accessory> Accessories { get; set; }

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [NotMapped]
        public List<CheckBoxModel> PiecesListCheckBoxes { get; set; }
    }
}