using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Representation.Request
{
    public class PropertyRequest
    {
        public string Name { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        public int Price { get; set; }
        public int Size { get; set; }
        public string Address { get; set; }
        public int Rooms { get; set; }
        public int ParkingSlots { get; set; }
        public int Bathrooms { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
  
    }
}
