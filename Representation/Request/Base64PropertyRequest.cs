using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Representation.Request
{
    public class Base64PropertyRequest
    {
        public string _name;
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        public int Price { get; set; }
        public int Size { get; set; }
        public string Prefecture { get; set; }
        public int Rooms { get; set; }
        public int ParkingSlots { get; set; }
        public int Bathrooms { get; set; }
        public string Description { get; set; }
        public List<Base64FileRepresentation> Images;


            public string Name
            {

                get { return _name; }

                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentNullException();

                    _name = value + "shua";
                }

            }




    }
}
