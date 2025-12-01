using DAL;

using Microsoft.AspNetCore.Mvc;

using Models;

namespace FullStackTechTest.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly ILogger<SpecialtiesController> _logger;

        public SpecialtiesController(ISpecialtyRepository specialtyRepository, ILogger<SpecialtiesController> logger)
        {
            _specialtyRepository = specialtyRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var specialties = await _specialtyRepository.ListAllAsync();
                return View(specialties);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Specialties page.");
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Create() => View(new Specialty());

        [HttpPost]
        public async Task<IActionResult> Create(Specialty specialty)
        {
            if(!ModelState.IsValid)
            {
                return View(specialty);
            }
            try
            {
                await _specialtyRepository.InsertAsync(specialty);
                return RedirectToAction("Index");
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new Specialty.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the specialty. Please try again.");
                return View(specialty);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var specialty = await _specialtyRepository.GetByIdAsync(id);
                if(specialty == null)
                {
                    return NotFound();
                }
                return View(specialty);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Edit page for Specialty ID {SpecialtyId}.", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Specialty specialty)
        {
            if(!ModelState.IsValid)
            {
                return View(specialty);
            }
            try
            {
                await _specialtyRepository.UpdateAsync(specialty);
                return RedirectToAction("Index");
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving Specialty ID {SpecialtyId}.", specialty.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the specialty. Please try again.");
                return View(specialty);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _specialtyRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Specialty ID {SpecialtyId}.", id);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
