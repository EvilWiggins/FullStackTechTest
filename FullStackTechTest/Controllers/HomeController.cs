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
    private readonly ISpecialtyRepository _specialtyRepository;

    public HomeController(ILogger<HomeController> logger, IPersonRepository personRepository, IAddressRepository addressRepository, ISpecialtyRepository specialtyRepository)
    {
        _logger = logger;
        _personRepository = personRepository;
        _addressRepository = addressRepository;
        _specialtyRepository = specialtyRepository;
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
            var model = await DetailsViewModel.CreateAsync(id, false, _personRepository, _addressRepository, _specialtyRepository);
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
            var model = await DetailsViewModel.CreateAsync(id, true, _personRepository, _addressRepository, _specialtyRepository);
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

            var currentSpecialties = await _specialtyRepository.ListForPersonAsync(model.Person.Id) ?? [];
            var currentIds = currentSpecialties.Select(s => s.Id).ToHashSet();
            var desiredIds = model.SelectedSpecialtyIds.ToHashSet() ?? new HashSet<int>(); 

            foreach(var toAdd in desiredIds.Except(currentIds))
            {
                await _specialtyRepository.AssignToPersonAsync(model.Person.Id, toAdd);
            }

            foreach(var toRemove in currentIds.Except(desiredIds))
            {
                await _specialtyRepository.RemoveFromPersonAsync(model.Person.Id, toRemove);
            }

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