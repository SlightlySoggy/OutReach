using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Role;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Role
{
    //https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-6.0&tabs=visual-studio
    ///https://csharp-video-tutorials.blogspot.com/2019/07/edit-role-in-aspnet-core.html
    //https://csharp-video-tutorials.blogspot.com/2019/07/add-or-remove-users-from-role-in-aspnet.html

    [Authorize]
    public class EditModel : PageModel
    {
        RoleManager<IdentityRole> roleManager; 

        //[Required(ErrorMessage = "Role Name is required")]
        public string Id { get; set; }  
        public string RoleName { get; set; }  
        public String errorMessage = "";
        public String successMessage = "";


        public EditModel(RoleManager<IdentityRole> roleManager)
        { 
            this.roleManager = roleManager;
        }
        public List<IdentityRole> roles { get; set; }
        public void OnGet()
        {
            String roleid = Request.Query["roleid"];
            String rolename = Request.Query["rolename"];

            this.Id = roleid;
            this.RoleName = rolename; 
            //roles = await roleManager.Roles.
            //roles = await roleManager.Roles.ToListAsync();

            // Find the role by Role ID 

            //if (role == null)
            //{
            //    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
            //    return View("NotFound");
            //} 
             
            return; 

        }
        public async Task<IActionResult> OnPostAsync()
        {
            IdentityRole role = await roleManager.FindByIdAsync(Request.Form["Id"]);
            role.Name = Request.Form["RoleName"];
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {  
                this.Id = role.Id;
                this.RoleName = role.Name; 
                successMessage = "Role name was changed successfully."; 
            }
            else
            {
                errorMessage = "Error occur, please try later.";
            }

            return Page();
        } 
    }
}