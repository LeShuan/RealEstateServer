using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Representation.Response
{
    public class PropertyItemResponse
    {
        public int id { get; set; }

        public string Name { get; set; }

        public string MainImageURL { get; set; }

        public double Longitud { get; set; }

        public double Latitud { get; set; }

        public int Price { get; set; }

        public int Size { get; set; }

        public string Address { get; set; }
    }
}
