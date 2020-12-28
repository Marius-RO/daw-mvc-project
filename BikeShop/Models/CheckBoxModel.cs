using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikeShop.Models
{
    public class CheckBoxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Checked { get; set; }
    }
}