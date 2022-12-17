using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Role
{
    [Authorize]
    // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-6.0
    // https://www.yogihosting.com/aspnet-core-identity-roles/
    public class IndexModel : PageModel
    {

        RoleManager<IdentityRole> roleManager; 
        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public List<IdentityRole> roles { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            roles = await roleManager.Roles.ToListAsync();

            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            IdentityRole role = await roleManager.FindByIdAsync(Request.Form["DeleteRoleid"]);
            await roleManager.DeleteAsync(role);
            return RedirectToPage("Index");
        }
    }
}