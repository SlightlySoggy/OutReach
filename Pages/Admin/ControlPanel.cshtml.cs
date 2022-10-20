using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Users;

namespace MyAffTest.Pages.Admin
{
    [Authorize]
    public class ControlPanelModel : PageModel
    {
        private readonly ILogger<ControlPanelModel> _logger;
        public ControlPanelModel(ILogger<ControlPanelModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
