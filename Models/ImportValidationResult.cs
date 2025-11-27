namespace Models
{
    public class ImportValidationResult
    {
        public List<string> Errors { get; set; } = [];
        public bool IsValid => Errors.Count == 0;
        public List<ImportDoctorDto> ValidDoctors { get; set; } = [];
    }
}
