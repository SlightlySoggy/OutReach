
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities; 

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Departments.DepartmentEdit
{
    [Authorize]//(Roles = "OrganizationContactor")]
    public class DepartmentsEditLightModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public Department DepartmentInfo = new Department();

        public List<StandardStatus> StatusList = new List<StandardStatus>();
        public List<LoginUserInfo> DepartmentManagerUserList = new List<LoginUserInfo>();
        public List<LoginUserInfo> DepartmentMemberList = new List<LoginUserInfo>();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public int org_id = 2;
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public DepartmentsEditLightModel(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              ILogger<JobdetailModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        //public void OnGet()
        //{
        //}


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
            StatusList = ut.GetStandardStatusList();

            List<LoginUserInfo> LoginUserList = new List<LoginUserInfo>();
            LoginUserList = ut.GetLoginUserList("");

            //user_id = Convert.ToInt32(user.User_Id);


            //PTStatus ptStatus = new PTStatus();
            //ListTag = PTStatus.GetReferencedTagsbyOpptunityId(""); // get all active Status

            //orgInfo = GetOrganizationInfoByOrg_id();
            if (orgInfo == null)
            {

            }

            if (string.IsNullOrWhiteSpace(Request.Query["DepartmentId"]))
            { // create a brand new Department 
                DepartmentInfo.OrganizationId = org_id.ToString();
                DepartmentInfo.CreatedUserId = user_id.ToString();
                return Page();
            }
            else
            { // load Department based on given id
                String DepartmentId = Request.Query["DepartmentId"];
                
                //hid_CurrentDepartmentid.value = "";

                Department op = new Department(DepartmentId);
                DepartmentInfo = op;

                DepartmentManagerUserList = ut.ResetDepartmentUserList(LoginUserList, op.DepartmentManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                LoginUserList = ut.GetLoginUserList("");
                DepartmentMemberList = ut.ResetDepartmentUserList(LoginUserList, op.DepartmentMemberUserIds);

                 
    }

            return Page();

        }
        public void OnPost()
        {
            string result = "";
             
            DepartmentInfo.Name = Request.Form["inputName"];
            DepartmentInfo.Description = Request.Form["inputDescription"];  

            if (!string.IsNullOrWhiteSpace(Request.Form["inputDepartmentStatus"]))
            {
                DepartmentInfo.StatusId = Request.Form["inputDepartmentStatus"];
            }
            else
                DepartmentInfo.StatusId = "1";



            if (string.IsNullOrWhiteSpace(Request.Form["hid_CurrentDepartmentid"]))
            { //special for new Department
                DepartmentInfo.OrganizationId = Request.Form["hid_orgid"];
                DepartmentInfo.CreatedDate = DateTime.Now.ToString();
                DepartmentInfo.CreatedUserId = Request.Form["hid_userId"];
                //DepartmentInfo.DepartmentManagerUserId = Request.Form["hid_userId"]; 


                result = DepartmentInfo.Save(); // Insert a new Department


            }
            else
            {
                DepartmentInfo.Id = Request.Form["hid_CurrentDepartmentid"];

                //1 update the Department leader selection for existing Department 

                GeneralUtilities ut = new GeneralUtilities(); 
                Department existingDepartment = new Department(Request.Form["hid_CurrentDepartmentid"]);
                List<string> newUserLeadlist = Request.Form["inputDepartmentLeader"].ToString().Split(',').ToList(); 

                if (!ut.IsDepartmentMemberChanged(existingDepartment.DepartmentManagerUserIds,newUserLeadlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllDepartmentUser(existingDepartment.Id, "true");                     

                    foreach (string uid in newUserLeadlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save Department level lead user
                            DepartmentUser ptu = new DepartmentUser();
                            ptu.DepartmentId = existingDepartment.Id; 
                            ptu.UserId = uid;
                            ptu.IsLead = "1"; //leader
                            ptu.Save();
                        }                     
                    }
                }


                //2 update the Department member selection for existing Department  
                List<string> newMemberlist = Request.Form["inputDepartmentMember"].ToString().Split(',').ToList();

                if (!ut.IsDepartmentMemberChanged(existingDepartment.DepartmentMemberUserIds, newMemberlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllDepartmentUser(existingDepartment.Id, "false");

                    foreach (string uid in newMemberlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save Department level lead user
                            DepartmentUser ptu = new DepartmentUser();
                            ptu.DepartmentId = existingDepartment.Id; 
                            ptu.UserId = uid;
                            ptu.IsLead = ""; //regulare member
                            ptu.Save();
                        }
                    }
                }


                // update existing Department
                result = DepartmentInfo.Save();

            }

             

            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hid_CurrentDepartmentid"]))
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                Response.Redirect("TeamEditLight?DepartmentId=" + Request.Form["hid_CurrentDepartmentid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            else if(result == "ok" && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentDepartmentid"]))
            {
                Response.Redirect("TeamEditLight");
            }
            else
            {
                errorMessage = result;
            }
             
        }
    }

}

