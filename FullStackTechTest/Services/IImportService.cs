using Models;

namespace FullStackTechTest.Services
{
    public interface IImportService
    {
        Task<ImportResult> ImportAsync(Stream jsonStream);
    }
}
