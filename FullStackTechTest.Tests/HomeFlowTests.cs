using DAL;
using FullStackTechTest.Controllers;
using FullStackTechTest.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Moq;

namespace FullStackTechTest.Tests;

public class HomeFlowTests
{
    [Fact]
    public async Task Index_returns_view_with_people_list()
    {
        var people = new List<Person>
        {
            new() { Id = 1, FirstName = "Test", LastName = "User", GMC = 1234567 }
        };

        var personRepo = new Mock<IPersonRepository>();
        personRepo.Setup(r => r.ListAllAsync()).ReturnsAsync(people);

        var addressRepo = new Mock<IAddressRepository>();
        var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), personRepo.Object, addressRepo.Object);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<IndexViewModel>(viewResult.Model);
        Assert.Single(model.PeopleList);
        Assert.Equal(people[0].Id, model.PeopleList[0].Id);
    }

    [Fact]
    public async Task DetailsViewModel_populates_person_address_and_edit_flag()
    {
        const int personId = 5;
        var person = new Person { Id = personId, FirstName = "Ada", LastName = "Lovelace", GMC = 7654321 };
        var address = new Address { Id = 10, PersonId = personId, Line1 = "1 Loop St", City = "Byteville", Postcode = "AB1 2CD" };

        var personRepo = new Mock<IPersonRepository>();
        personRepo.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);

        var addressRepo = new Mock<IAddressRepository>();
        addressRepo.Setup(r => r.GetForPersonIdAsync(personId)).ReturnsAsync(address);

        var model = await DetailsViewModel.CreateAsync(personId, true, personRepo.Object, addressRepo.Object);

        Assert.Same(person, model.Person);
        Assert.Same(address, model.Address);
        Assert.True(model.IsEditing);
    }
}
