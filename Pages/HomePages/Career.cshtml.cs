using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyAffTest.Pages
{
    public class CareerModel : PageModel
    {
        private readonly ILogger<CareerModel> _logger;

        public CareerModel(ILogger<CareerModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}