using System.Diagnostics;
using DAL;
using Microsoft.AspNetCore.Mvc;
using FullStackTechTest.Models.Home;
using FullStackTechTest.Models.Shared;

namespace FullStackTechTest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPersonRepository _personRepository;
    private readonly IAddressRepository _addressRepository;

    public HomeController(ILogger<HomeController> logger, IPersonRepository personRepository, IAddressRepository addressRepository)
    {
        _logger = logger;
        _personRepository = personRepository;
        _addressRepository = addressRepository;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var model = await IndexViewModel.CreateAsync(_personRepository);
            return View(model);

        } catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the Index page.");
            return RedirectToAction("Error");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var model = await DetailsViewModel.CreateAsync(id, false, _personRepository, _addressRepository);
            return View(model);

        } catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the Details page for Person ID {PersonId}.", id);
            return RedirectToAction("Error");
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var model = await DetailsViewModel.CreateAsync(id, true, _personRepository, _addressRepository);
            return View("Details", model);

        } catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the Edit page for Person ID {PersonId}.", id);
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] DetailsViewModel model)
    {
        try
        {
            await _personRepository.SaveAsync(model.Person);
            await _addressRepository.SaveAsync(model.Address);
            return RedirectToAction("Details", new { id = model.Person.Id });

        } catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the Edit page for Person ID {PersonId}.", id);
            return RedirectToAction("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}