using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;
using Microsoft.Extensions.Logging;
using Outreach.Data;

namespace Outreach.Pages.Opportunities
{
    [Authorize]
    public class JobdetailModel : PageModel
    {

        public Opportunity opportunity = new Opportunity();
        public Boolean DefaultShow = false; 

        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public JobdetailModel(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              ILogger<JobdetailModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            string oppid = Request.Query["OppId"];
            Opportunity op = new Opportunity(oppid);
            opportunity = op;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to find signin user info.");
                return Page();
            }

            var admins = (await _userManager
                .GetUsersInRoleAsync("Administrator"))
                .ToList();

            if (admins.Contains(user))
            {
                DefaultShow = true;

            }
            else
            {
                GeneralUtilities generalUtilities = new GeneralUtilities();
                int user_id = generalUtilities.GetLoginUserIntIDbyGUID(user.Id);

                if (user_id.ToString() == op.CreatedUserId)
                {
                    DefaultShow = true;
                }

            }

            return Page();
        }
    }
}