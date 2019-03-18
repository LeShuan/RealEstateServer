using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class PropertyDetails
    {
        public int Id { get; set; }
        public int Rooms { get; set; }
        public int ParkingSlots { get; set; }
        public int Bathrooms { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public int PropertyId { get; set; }
        public Property property { get; set; }

    }
}
