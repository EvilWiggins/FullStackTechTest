namespace Models
{
    public class ImportDoctorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GMC { get; set; }
        public List<ImportAddressDto> Address { get; set; }
    }
}
