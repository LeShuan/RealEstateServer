using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class Property
    {
        [Key]
        public int id { get; set; }
        [MaxLength(256)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1024)]
        [Required]
        public string MainImageURL { get; set; }
        [Required]
        public double Longitud { get; set; }
        [Required]
        public double Latitud { get; set; }
        [Required]
        public int Price { get; set; }
        public int Size { get; set; }
        [MaxLength(254)]
        [Required]
        public string Address { get; set; }
        public PropertyDetails Details { get; set; }

    }
}
