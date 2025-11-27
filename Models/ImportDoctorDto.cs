using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ImportDoctorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GMC { get; set; }
        public ImportAddressDto Address { get; set; }
    }
}
