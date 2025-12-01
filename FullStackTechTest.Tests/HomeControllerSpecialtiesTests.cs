using DAL;

using FullStackTechTest.Controllers;
using FullStackTechTest.Models.Home;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Models;

using Moq;

namespace FullStackTechTest.Tests
{
    public class HomeControllerSpecialtiesTests
    {
        [Fact]
        public async Task Edit_post_updates_specialties_assign_and_remove()
        {
            const int personId = 4;

            var person = new Person { Id = personId, FirstName = "John", LastName = "Doe", GMC = 1234567};
            var address = new Address { Id = 1, Line1 = "123 Main St", City = "Anytown", Postcode = "A1 2BC" };

            var personRepo = new Mock<IPersonRepository>();
            var addressRepo = new Mock<IAddressRepository>();
            var specialtyRepo = new Mock<ISpecialtyRepository>();

            specialtyRepo.Setup(r => r.ListForPersonAsync(personId))
                .ReturnsAsync(new List<Specialty>
                {
                    new Specialty { Id = 1, Name = "Cardiology" },
                    new Specialty { Id = 2, Name = "Neurology" }
                });

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), personRepo.Object, addressRepo.Object, specialtyRepo.Object);

            var model = new DetailsViewModel
            {
                Person = person,
                Address = address,
                SelectedSpecialtyIds = [2, 3]
            };

            var result = await controller.Edit(personId, model);

            personRepo.Verify(r => r.SaveAsync(person), Times.Once);
            addressRepo.Verify(r => r.SaveAsync(address), Times.Once);
            specialtyRepo.Verify(r => r.RemoveFromPersonAsync(personId, 1), Times.Once);
            specialtyRepo.Verify(r => r.AssignToPersonAsync(personId, 3), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal(personId, redirect.RouteValues?["id"]);
        }
    }
}
