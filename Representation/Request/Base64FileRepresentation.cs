using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Representation.Request
{
    public class Base64FileRepresentation
    {
        public string Name { get; set; }
        private byte[] _Base64Data;

        public string Base64Data {

            get { return Convert.ToBase64String(_Base64Data); }
            set { _Base64Data = Convert.FromBase64String(value); }
       
        }
         
        public byte[] getBase64ByteArray() {
            return _Base64Data;
        }
    }
}
