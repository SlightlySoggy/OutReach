using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;
using System.Security.Cryptography;

namespace Outreach.Areas.Identity.Pages.RegisterOrg
{
    public class RegisterOrganizationModel : PageModel
    {
        public Organization orgInfo = new Organization();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;

         
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public RegisterOrganizationModel(UserManager<ApplicationUser> userManager,
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
            GeneralUtilities ut = new GeneralUtilities();
            user_id = ut.GetLoginUserIntIDbyGUID(user.Id); 

            //List<LoginUserInfo> LoginUserList = new List<LoginUserInfo>();
            //LoginUserList = ut.GetLoginUserList("1", "", ""); // get all users belong to parent level (organization)


            if (string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            { // create a brand new Organization  
                //orgInfo.CreatedDate = ??????????????????????????????
                orgInfo.CreatedUserId = user_id.ToString();
                orgInfo.StatusId = "1";
                return Page();
            }
            else
            { // load Organization based on given id

                String OrgId = Request.Query["OrgId"]; 
                orgInfo = new Organization(OrgId);

                //hid_CurrentOrgId.value = "";

                Organization op = new Organization(OrgId);
                orgInfo = op;

                //OrganizationManagerUserList = ut.ResetUserLinkageList(LoginUserList, op.OrganizationManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                //LoginUserList = ut.GetLoginUserList("");
                //OrganizationMemberList = ut.ResetUserLinkageList(LoginUserList, op.OrganizationMemberUserIds);


            }

            return Page();

        }


        public async Task<IActionResult> OnPostAsync()
        {

            return Page();


        }




        public void OnPost2()
        {
            string result = "";

            orgInfo.Name = Request.Form["inputName"];
            orgInfo.Description = Request.Form["inputDescription"];
            orgInfo.Email = Request.Form["inputEmail"]; 

            //if (!string.IsNullOrWhiteSpace(Request.Form["inputOrganizationStatus"]))
            //{
            //    orgInfo.OrganizationTaskStatusId = Request.Form["inputOrganizationStatus"];
            //}
            //else
            //    orgInfo.OrganizationTaskStatusId = "1";



            if (string.IsNullOrWhiteSpace(Request.Form["hid_CurrentOrgid"]))
            { //special for new Organization 
                orgInfo.CreatedDate = DateTime.Now.ToString();
                orgInfo.CreatedUserId = Request.Form["hid_userId"];  

                result = orgInfo.Save(); // Insert a new Organization 
            }
            else
            {
                orgInfo.Id = Request.Form["hid_CurrentOrgid"];

                //1 update the Organization leader selection for existing Organization 

                GeneralUtilities ut = new GeneralUtilities();
                Organization existingOrganization = new Organization(Request.Form["hid_CurrentOrganizationid"]);
                List<string> newUserLeadlist = Request.Form["inputOrganizationLeader"].ToString().Split(',').ToList();


                result = ut.ProcessLinkedUsers(existingOrganization.ManagerUserIds, newUserLeadlist, "1", orgInfo.Id, "1");


                //2 update the Organization member selection for existing Organization  
                List<string> newMemberlist = Request.Form["inputOrganizationMember"].ToString().Split(',').ToList();

                //remove lead user from the member selection  
                List<string> newMemberlist2 = new List<string>();
                var differentMembers2 = newMemberlist.Where(p2 => newUserLeadlist.All(p => p2 != p)).ToList<string>();
                differentMembers2.ForEach(u => newMemberlist2.Add(u));

                result = ut.ProcessLinkedUsers(existingOrganization.MemberUserIds, newMemberlist2, "1", orgInfo.Id, "0");



                // update existing Organization
                result = orgInfo.Save();

            }



            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hid_CurrentOrganizationid"]))
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                Response.Redirect("OrganizationsEditLight?OrganizationId=" + Request.Form["hid_CurrentOrganizationid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            else if (result.Contains("failed") == false && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentOrganizationid"]))
            {
                Response.Redirect("RegisterOrganization?Orgid=" + result); 
            }
            else
            {
                errorMessage = result;
            }

        }
    }

}

