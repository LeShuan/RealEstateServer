using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Representation.Request
{
    public class SearchRequest
    {
        public int PriceMin { get; set; }
        public int PriceMax { get; set; }
        public int Distance { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
    }
}
