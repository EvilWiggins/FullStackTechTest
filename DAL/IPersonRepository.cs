using Models;

namespace DAL;

public interface IPersonRepository
{
    Task<Person?> GetByGMCAsync(int gmcNumber);

    Task<Person> GetByIdAsync(int personId);

    Task<int> InsertAsync(Person person);

    Task<List<Person>> ListAllAsync();
    Task SaveAsync(Person person);
}