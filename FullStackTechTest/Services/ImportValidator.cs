using Models;

namespace FullStackTechTest.Services
{
    public class ImportValidator
    {
        public ImportValidationResult Validate(IEnumerable<ImportDoctorDto> doctors)
        {
            var result = new ImportValidationResult();
            var seenGmcs = new HashSet<int>();

            foreach(var doctor in doctors)
            {
                var gmcRaw = doctor.GMC?.Trim() ?? string.Empty;
                if(!int.TryParse(gmcRaw, out var gmc))
                {
                    result.Errors.Add($"GMC must be numeric for Dr {doctor.FirstName} {doctor.LastName}".Trim()); 
                    continue;
                }

                if(gmc < 1000000 || gmc > 9999999)
                {
                    result.Errors.Add($"GMC must be 7 digits for Dr {doctor.FirstName} {doctor.LastName}".Trim());
                    continue;
                }

                if(!seenGmcs.Add(gmc))
                {
                    result.Errors.Add($"Duplicate GMC {gmc} for Dr {doctor.FirstName} {doctor.LastName}".Trim());
                    continue;
                }

                doctor.Address ??= new ImportAddressDto();
                doctor.GMC = gmc.ToString("D7");
                result.ValidDoctors.Add(doctor);
            }

            return result;
        }
    }
}
