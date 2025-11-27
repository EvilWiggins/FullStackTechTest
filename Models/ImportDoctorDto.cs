namespace Models
{
    public class ImportDoctorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GMC { get; set; }
        public ImportAddressDto Address { get; set; }
    }
}
