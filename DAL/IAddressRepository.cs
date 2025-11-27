using Models;

namespace DAL;

public interface IAddressRepository
{
    Task<Address> GetForPersonIdAsync(int personId);
    Task<int> InsertAsync(Address address);

    Task SaveAsync(Address address);
}