using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAffTest.Pages.Clients;
using System.Data;

namespace MyAffTest.Areas.Identity.Pages.Role
{
    public class CreateModel : PageModel
    {
        RoleManager<IdentityRole> roleManager;
        public CreateModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        //public void OnGet()
        //{
        //}  
        //public async Task<IActionResult> Create(IdentityRole role)
        //{
        //    await roleManager.CreateAsync(role);
        //    return RedirectToAction("RoleManager");
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            string newRoleName = Request.Form["rolename"].ToString().Trim();
            List<IdentityRole> roles = await roleManager.Roles.ToListAsync();

            int count = 0;
            foreach (IdentityRole r in roles)
            {
                if (r.Name.ToUpper() == newRoleName.ToUpper())
                    count++;
            }

            if (count == 0)
            {
                // Create the role 
                IdentityRole role = new IdentityRole(newRoleName);
                await roleManager.CreateAsync(role); 
            }
            return RedirectToPage("Index");
        }
    }
}