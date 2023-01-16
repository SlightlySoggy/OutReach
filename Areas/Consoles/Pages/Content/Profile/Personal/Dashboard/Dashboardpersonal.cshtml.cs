using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Data;
using Outreach.Pages.Opportunities;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Personal.Dashboard
{
    [Authorize]

    public class DashboardpersonalModel : PageModel
    {
        public string UserName { get; set; }
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public int org_id = 2;
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public DashboardpersonalModel(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              ILogger<JobdetailModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to find signin user info.");
                return Page();
            }
            else
            {
                UserName = user.UserName;
            }

            return Page();
        }
    }
}
