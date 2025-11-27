using FullStackTechTest.Services;

using Models;

namespace FullStackTechTest.Tests
{
    public class ImportValidatorTests
    {
        private readonly ImportValidator _importValidator = new();

        [Fact]
        public void Valid_payload_passes()
        {
            var payload = new[]
            {
                new ImportDoctorDto { FirstName = "Alice", LastName = "Smith", GMC = "1234567" },
                new ImportDoctorDto { FirstName = "Bob", LastName = "Jones", GMC = "7654321" }
            };

            var result = _importValidator.Validate(payload);

            Assert.True(result.IsValid);
            Assert.Equal(2, result.ValidDoctors.Count);
        }

        [Fact]
        public void Rejects_non_seven_digit_gmc()
        {
            var payload = new[]
            {
                new ImportDoctorDto { FirstName = "Alice", LastName = "Smith", GMC = "12345" },
                new ImportDoctorDto { FirstName = "Bob", LastName = "Jones", GMC = "76543210" }
            };
            
            var result = _importValidator.Validate(payload);
         
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains("GMC must be 7 digits for Dr Alice Smith", result.Errors);
            Assert.Contains("GMC must be 7 digits for Dr Bob Jones", result.Errors);
        }

        [Fact]
        public void Rejects_duplicate_gmc()
        {
            var payload = new[]
            {
                new ImportDoctorDto { FirstName = "Alice", LastName = "Smith", GMC = "1234567" },
                new ImportDoctorDto { FirstName = "Bob", LastName = "Jones", GMC = "1234567" }
            };

            var result = _importValidator.Validate(payload);
            
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("Duplicate GMC 1234567 for Dr Bob Jones", result.Errors);
        }
    }
}
