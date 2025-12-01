using DAL;
using Models;

namespace FullStackTechTest.Models.Home;

public class DetailsViewModel
{
    public Person Person { get; set; }
    public Address Address { get; set; }
    public bool IsEditing { get; set; }
    public List<Specialty> AvailableSpecialties { get; set; } = new();
    public List<int> SelectedSpecialtyIds { get; set; } = new();

    public static async Task<DetailsViewModel> CreateAsync(int personId, bool isEditing, IPersonRepository personRepository, IAddressRepository addressRepository, ISpecialtyRepository specialtyRepository)
    {
        var person = await personRepository.GetByIdAsync(personId);
        var address = await addressRepository.GetForPersonIdAsync(personId);
        var availableSpecialties = await specialtyRepository.ListAllAsync() ?? new List<Specialty>();
        var personSpecialties = await specialtyRepository.ListForPersonAsync(personId) ?? new List<Specialty>();

        var model = new DetailsViewModel
        {
            Person = person,
            Address = address,
            IsEditing = isEditing,
            AvailableSpecialties = availableSpecialties,
            SelectedSpecialtyIds = [.. personSpecialties.Select(s => s.Id)]
        };
        return model;
    }
}
