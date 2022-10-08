using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.LoginUsers;

namespace MyAffTest.Areas.Identity.Pages.Role
{
    //https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-6.0&tabs=visual-studio
    ///https://csharp-video-tutorials.blogspot.com/2019/07/edit-role-in-aspnet-core.html
    //https://csharp-video-tutorials.blogspot.com/2019/07/add-or-remove-users-from-role-in-aspnet.html

    [Authorize(Roles = "Administrator")]
    public class UserInRoleModel : PageModel
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<IdentityUser> userManager;

        //[Required(ErrorMessage = "Role Name is required")]
        public string Id { get; set; }
        public string RoleName { get; set; }
        //public string NewUserEmail { get; set; }
        
        public List<LoginUserInfo> Users { get; set; }
        public String errorMessage = "";
        public String successMessage = "";


        public UserInRoleModel(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public List<IdentityRole> roles { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            String roleid = Request.Query["roleid"]; 

            this.Id = roleid; 
            this.Users = new List<LoginUserInfo>();

            //roles = await roleManager.Roles.
            //roles = await roleManager.Roles.ToListAsync();

            // Find the role by Role ID
            var role = await roleManager.FindByIdAsync(roleid);
            this.RoleName = role.Name; 
            //if (role == null)
            //{
            //    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
            //    return View("NotFound");
            //}
            // Retrieve all the Users 
            foreach (var user in userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    LoginUserInfo lu = new LoginUserInfo();
                    lu.Id = user.Id;
                    lu.Email = user.Email;
                    Users.Add(lu);
                }
            }

        //https://localhost:7057/Identity/Role/UserInRole?roleid=cbaf5614-e558-4cb6-87bf-3b90158beac8
            //return View(model);
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            string NewUserEmail = Request.Form["NewUserEmail"].ToString().Trim(); 
            string checkedtodeleteuserid = Request.Form["AreChecked"].ToString().Trim();
            string userid = checkedtodeleteuserid;
            string actionResult = "";


            IdentityRole role = await roleManager.FindByIdAsync(Request.Form["Id"]);
            if (role != null)
            {
                // Find new user want to be assigned with this role by user name
                var newuser = await userManager.FindByEmailAsync(NewUserEmail);
                if (NewUserEmail.Length > 0)
                {
                    if (newuser != null)
                    {
                        if (!await userManager.IsInRoleAsync(newuser, role.Name))
                        {
                            var result = await userManager.AddToRoleAsync(newuser, role.Name); 
                            if (result.Succeeded)
                            {
                                actionResult = "Succeeded";
                                successMessage = "new user is assigned with this role.";
                            } 
                        }
                        else
                        { 
                            actionResult = "User already has this role";
                        }

                    }
                }

                // Find the user want to be removed with this role by user ID
                if (checkedtodeleteuserid.Length > 0)
                {
                    List<string> userIdList = checkedtodeleteuserid.Split(',').ToList(); 

                    foreach (string userId in userIdList)
                    {
                        var user = await userManager.FindByIdAsync(userid);
                        if (user != null)
                        {
                            var result = await userManager.RemoveFromRoleAsync(user, role.Name);
                            {
                                actionResult = "Succeeded";
                                successMessage = "user is removed from this role.";
                            }
                        }

                    }



                }
            }

            //Prepare to reload this page 
            this.Id = role.Id;
            this.RoleName = role.Name;
            this.Users = new List<LoginUserInfo>();
            successMessage = actionResult;
            foreach (var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    LoginUserInfo lu = new LoginUserInfo();
                    lu.Id = user.Id;
                    lu.Email = user.Email;
                    Users.Add(lu);
                }
            }

            if (actionResult != "")
            {
                errorMessage = "";
            }
            else
            {
                errorMessage = "Error occur, please try later.";
            }

            return Page();
        }
    }
}