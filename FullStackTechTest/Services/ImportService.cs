using DAL;

using Models;

using System.Text.Json;

namespace FullStackTechTest.Services
{
    public class ImportService : IImportService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ImportValidator _importValidator;

        public ImportService(IPersonRepository personRepository, IAddressRepository addressRepository, ImportValidator importValidator)
        {
            _personRepository = personRepository;
            _addressRepository = addressRepository;
            _importValidator = importValidator;
        }

        public async Task<ImportResult> ImportAsync(Stream jsonStream)
        {
            var result = new ImportResult();

            var payload = await JsonSerializer.DeserializeAsync<List<ImportDoctorDto>>(jsonStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];

            result.TotalRecords = payload.Count;

            var validation = _importValidator.Validate(payload);
            result.Errors.AddRange(validation.Errors);

            foreach(var doctor in validation.ValidDoctors)
            {
                var gmc = doctor.GMC;
                var existing = await _personRepository.GetByGMCAsync(gmc);

                if(existing != null)
                {
                    result.SkippedExisting++;
                    continue;
                }

                var personId = await _personRepository.InsertAsync(new Person
                {
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    GMC = gmc
                });

                foreach(var address in doctor.Address)
                {
                    await _addressRepository.InsertAsync(new Address
                    {
                        PersonId = personId,
                        Line1 = address.Line1,
                        City = address.City,
                        Postcode = address.Postcode
                    });
                }

                result.Inserted++;
            }

            return result;
        }
    }
}
