using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class Image
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(1024)]
        public string Url { get; set; }
        public int PropertyDetailsId { get; set; }
        public PropertyDetails propertyDetails { get; set; }

    }
}
