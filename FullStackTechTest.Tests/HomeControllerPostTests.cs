using DAL;
using FullStackTechTest.Controllers;
using FullStackTechTest.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Moq;

namespace FullStackTechTest.Tests;

public class HomeControllerPostTests
{
    [Fact]
    public async Task Edit_post_calls_repositories_and_redirects()
    {
        var personRepo = new Mock<IPersonRepository>();
        var addressRepo = new Mock<IAddressRepository>();
        var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), personRepo.Object, addressRepo.Object);

        var model = new DetailsViewModel
        {
            Person = new Person { Id = 3, FirstName = "A", LastName = "B", GMC = 1234567 },
            Address = new Address { Id = 7, PersonId = 3, Line1 = "Line", City = "City", Postcode = "AB1 2CD" }
        };

        var result = await controller.Edit(model.Person.Id, model);

        personRepo.Verify(r => r.SaveAsync(model.Person), Times.Once);
        addressRepo.Verify(r => r.SaveAsync(model.Address), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal(model.Person.Id, redirect.RouteValues?["id"]);
    }
}
