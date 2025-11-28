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
                var gmc = doctor.GMC;

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

                doctor.Address ??= [];
                result.ValidDoctors.Add(doctor);
            }

            return result;
        }
    }
}
