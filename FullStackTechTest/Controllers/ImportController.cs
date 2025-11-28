using FullStackTechTest.Services;

using Microsoft.AspNetCore.Mvc;

using Models;

namespace FullStackTechTest.Controllers
{
    public class ImportController : Controller
    {
        private readonly IImportService _importService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(IImportService importService, ILogger<ImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() => View(new ImportResult());

        [HttpPost]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Index(IFormFile jsonFile)
        {
            if(jsonFile == null || jsonFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a JSON file.");
                return View(new ImportResult { Errors = { "No file supplied" } });
            }

            try
            {
                await using var stream = jsonFile.OpenReadStream();
                var result = await _importService.ImportAsync(stream);
                return View(result);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import data");
                return View(new ImportResult { Errors = { "Import failed. See logs for details." } });
            }
        }
    }
}
