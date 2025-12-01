using DAL;

using FullStackTechTest.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Models;

using Moq;

using System;
using System.Collections.Generic;
using System.Text;

namespace FullStackTechTest.Tests
{
    public class SpecialtiesControllerTests
    {
        [Fact]
        public async Task Index_returns_view_with_list()
        {
            var list = new List<Specialty> { new() { Id = 1, Name = "Cardiology" } };
            var repo = new Mock<ISpecialtyRepository>();
            repo.Setup(r => r.ListAllAsync()).ReturnsAsync(list);

            var controller = new SpecialtiesController(repo.Object, Mock.Of<ILogger<SpecialtiesController>>());

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Specialty>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Cardiology", model[0].Name);
        }

        [Fact]
        public async Task Create_valid_redirects_and_inserts()
        {
            var repo = new Mock<ISpecialtyRepository>();
            repo.Setup(r => r.InsertAsync(It.IsAny<Specialty>())).ReturnsAsync(1);

            var controller = new SpecialtiesController(repo.Object, Mock.Of<ILogger<SpecialtiesController>>());

            var specialty = new Specialty { Name = "Neurology" };
            var result = await controller.Create(specialty);

            repo.Verify(r => r.InsertAsync(It.Is<Specialty>(s => s.Name == "Neurology")), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SpecialtiesController.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_valid_redirects_and_updates()
        {
            var repo = new Mock<ISpecialtyRepository>();

            var controller = new SpecialtiesController(repo.Object, Mock.Of<ILogger<SpecialtiesController>>());

            var result = await controller.Edit(new Specialty { Id = 2, Name = "UpdatedName" });

            repo.Verify(r => r.UpdateAsync(It.Is<Specialty>(s => s.Id == 2 && s.Name == "UpdatedName")), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SpecialtiesController.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_calls_repo_and_redirects()
        {
            var repo = new Mock<ISpecialtyRepository>();
            var controller = new SpecialtiesController(repo.Object, Mock.Of<ILogger<SpecialtiesController>>());

            var result = await controller.Delete(3);

            repo.Verify(r => r.DeleteAsync(3), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SpecialtiesController.Index), redirectResult.ActionName);
        }
    }
}
