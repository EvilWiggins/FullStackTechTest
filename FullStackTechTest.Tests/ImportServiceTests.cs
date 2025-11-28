using DAL;

using FullStackTechTest.Services;

using Models;

using Moq;

using System.Text;

namespace FullStackTechTest.Tests
{
    public class ImportServiceTests
    {
        private static Stream ToStream(string json) => new MemoryStream(Encoding.UTF8.GetBytes(json));

        [Fact]
        public async Task Inserts_new_person_and_address()
        {
            var personRepo = new Mock<IPersonRepository>();
            var addressRepo = new Mock<IAddressRepository>();

            personRepo.Setup(r => r.GetByGMCAsync(1234567)).ReturnsAsync((Person?)null);
            personRepo.Setup(r => r.InsertAsync(It.IsAny<Person>())).ReturnsAsync(2077);

            var service = new ImportService(personRepo.Object, addressRepo.Object, new ImportValidator());

            var json = """
                [
                    {
                        "firstName": "Hedy",
                        "lastName": "Lamarr",
                        "GMC": 1234567,
                        "address": [
                            {
                                "line1": "1 The Blvd",
                                "city": "London",
                                "postcode": "AB1 2CD"
                            }
                        ]
                    }
                ]
                """;

            var result = await service.ImportAsync(ToStream(json));

            Assert.True(result.Success);
            Assert.Equal(1, result.Inserted);
            addressRepo.Verify(r => r.InsertAsync(It.Is<Address>(a => a.PersonId == 2077)), Times.Once);
        }

        [Fact]
        public async Task Skips_existing_gmc()
        {
            var personRepo = new Mock<IPersonRepository>();
            var addressRepo = new Mock<IAddressRepository>();

            personRepo.Setup(r => r.GetByGMCAsync(1234567)).ReturnsAsync(new Person { Id = 5, GMC = 1234567 });

            var service = new ImportService(personRepo.Object, addressRepo.Object, new ImportValidator());

            var json = """
                [
                    {
                        "firstName": "Hedy",
                        "lastName": "Lamarr",
                        "GMC": 1234567,
                        "address": [
                            {
                                "line1": "1 The Blvd",
                                "city": "London",
                                "postcode": "AB1 2CD"
                            }
                        ]
                    }
                ]
                """;

            var result = await service.ImportAsync(ToStream(json));

            Assert.Equal(1, result.SkippedExisting);
            personRepo.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Never);
        }

        [Fact]
        public async Task Reports_duplicate_in_payload()
        {
            var personRepo = new Mock<IPersonRepository>();
            var addressRepo = new Mock<IAddressRepository>();

            var service = new ImportService(personRepo.Object, addressRepo.Object, new ImportValidator());

            var json = """
                [
                    {
                        "firstName": "Hedy",
                        "lastName": "Lamarr",
                        "GMC": 1234567,
                        "address": [
                            {
                                "line1": "1 The Blvd",
                                "city": "London",
                                "postcode": "AB1 2CD"
                            }
                        ]
                    },
                    {
                        "firstName": "John",
                        "lastName": "Doe",
                        "GMC": 1234567,
                        "address": [
                            {
                                "line1": "2 The St",
                                "city": "Manchester",
                                "postcode": "XY9 8ZW"
                            }
                        ]
                    }
                ]
                """;

            var result = await service.ImportAsync(ToStream(json));

            Assert.False(result.Success);
            Assert.Contains(result.Errors, e => e.Contains("Duplicate"));
            personRepo.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Once);
        }
    }
}
