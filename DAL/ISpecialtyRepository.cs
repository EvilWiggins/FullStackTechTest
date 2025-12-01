using Models;

namespace DAL
{
    public interface ISpecialtyRepository
    {
        Task AssignToPersonAsync(int personId, int specialtyId);

        Task DeleteAsync(int id);

        Task<Specialty?> GetByIdAsync(int id);

        Task<int> InsertAsync(Specialty specialty);

        Task<List<Specialty>> ListAllAsync();
        Task<List<Specialty>> ListForPersonAsync(int personId);

        Task RemoveFromPersonAsync(int personId, int specialtyId);

        Task UpdateAsync(Specialty specialty);
    }
}
