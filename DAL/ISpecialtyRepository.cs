using Models;

namespace DAL
{
    public interface ISpecialtyRepository
    {
        Task<List<Specialty>> ListAllAsync();
        Task<Specialty?> GetByIdAsync(string id);
        Task<int> InsertAsync(Specialty specialty);
        Task UpdateAsync(Specialty specialty);
        Task DeleteAsync(string id);
        Task<List<Specialty>> ListForPersonAsync(int personId);
        Task AssignToPersonAsync(int personId, string specialtyId);
        Task RemoveFromPersonAsync(int personId, string specialtyId);
    }
}
