using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyAffTest.Pages.Admin
{
    // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-6.0
    // https://www.yogihosting.com/aspnet-core-identity-roles/
    public class RoleManagerModel : PageModel
    {

        RoleManager<IdentityRole> roleManager;

        public RoleManagerModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public List<IdentityRole> roles { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            roles = await roleManager.Roles.ToListAsync();

            return Page();

        }
    }
}