
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
using Project = Outreach.Pages.Utilities.Project;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Projects.ProjectEdit
{
    [Authorize]//(Roles = "OrganizationContactor")]
    public class ProjectsEditLightModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public Project projectInfo = new Project();

        public List<ProjTaskStatus> StatusList = new List<ProjTaskStatus>();
        public List<LoginUserInfo> ProjectManagerUserList = new List<LoginUserInfo>();
        public List<LoginUserInfo> ProjectMemberList = new List<LoginUserInfo>();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public int org_id = 2;
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public ProjectsEditLightModel(UserManager<ApplicationUser> userManager,
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
            StatusList = ut.GetProjTaskStatusList();

            List<LoginUserInfo> LoginUserList = new List<LoginUserInfo>();
            LoginUserList = ut.GetLoginUserList("");

            //user_id = Convert.ToInt32(user.User_Id);


            //PTStatus ptStatus = new PTStatus();
            //ListTag = PTStatus.GetReferencedTagsbyOpptunityId(""); // get all active Status

            //orgInfo = GetOrganizationInfoByOrg_id();
            if (orgInfo == null)
            {

            }

            if (string.IsNullOrWhiteSpace(Request.Query["ProjectId"]))
            { // create a brand new Project 
                projectInfo.CreatedOrgId = org_id.ToString();
                projectInfo.CreatedUserId = user_id.ToString();
                return Page();
            }
            else
            { // load Project based on given id
                String ProjectId = Request.Query["ProjectId"];
                
                //hid_CurrentProjectid.value = "";

                Project op = new Project(ProjectId);
                projectInfo = op;

                ProjectManagerUserList = ut.ResetProjectTaskUserList(LoginUserList, op.ProjectManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                LoginUserList = ut.GetLoginUserList("");
                ProjectMemberList = ut.ResetProjectTaskUserList(LoginUserList, op.ProjectMemberUserIds);

                 
    }

            return Page();

        }
        public void OnPost()
        {
            string result = "";
             
            projectInfo.ProjectName = Request.Form["inputName"];
            projectInfo.Description = Request.Form["inputDescription"];
            projectInfo.EstimatedBudget = Request.Form["inputEstimatedBudget"];
            projectInfo.ActualSpent = Request.Form["inputSpentBudget"];
            //projectInfo.DurationByDay = Request.Form["inputEstimatedDuration"];
            projectInfo.StartDate = Request.Form["inputStartDate"];
            projectInfo.DueDate = Request.Form["inputDueDate"];
            projectInfo.CompletionDate = Request.Form["inputCompletionDate"];

            if (!string.IsNullOrWhiteSpace(Request.Form["inputProjectStatus"]))
            {
                projectInfo.ProjectTaskStatusId = Request.Form["inputProjectStatus"];
            }
            else
                projectInfo.ProjectTaskStatusId = "1";



            if (string.IsNullOrWhiteSpace(Request.Form["hid_CurrentProjectid"]))
            { //special for new project
                projectInfo.CreatedOrgId = Request.Form["hid_orgid"];
                projectInfo.CreatedDate = DateTime.Now.ToString();
                projectInfo.CreatedUserId = Request.Form["hid_userId"];
                //projectInfo.ProjectManagerUserId = Request.Form["hid_userId"]; 


                result = projectInfo.Save(); // Insert a new project


            }
            else
            {
                projectInfo.Id = Request.Form["hid_CurrentProjectid"];

                //1 update the project leader selection for existing project 

                GeneralUtilities ut = new GeneralUtilities(); 
                Project existingProject = new Project(Request.Form["hid_CurrentProjectid"]);
                List<string> newUserLeadlist = Request.Form["inputProjectLeader"].ToString().Split(',').ToList(); 

                if (!ut.IsProjTaskMemberChanged(existingProject.ProjectManagerUserIds,newUserLeadlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllProjectTaskUser(existingProject.Id, "", "true");                     

                    foreach (string uid in newUserLeadlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save project level lead user
                            ProjectTaskUser ptu = new ProjectTaskUser();
                            ptu.ProjectId = existingProject.Id;
                            ptu.TaskId = "";
                            ptu.UserId = uid;
                            ptu.IsLead = "1"; //leader
                            ptu.Save();
                        }                     
                    }
                }


                //2 update the project member selection for existing project  
                List<string> newMemberlist = Request.Form["inputProjectMember"].ToString().Split(',').ToList();

                if (!ut.IsProjTaskMemberChanged(existingProject.ProjectMemberUserIds, newMemberlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllProjectTaskUser(existingProject.Id, "", "false");

                    foreach (string uid in newMemberlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save project level lead user
                            ProjectTaskUser ptu = new ProjectTaskUser();
                            ptu.ProjectId = existingProject.Id;
                            ptu.TaskId = "";
                            ptu.UserId = uid;
                            ptu.IsLead = ""; //regulare member
                            ptu.Save();
                        }
                    }
                }


                // update existing project
                result = projectInfo.Save();

            }

             

            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hid_CurrentProjectid"]))
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                Response.Redirect("ProjectsEditLight?ProjectId=" + Request.Form["hid_CurrentProjectid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            else if(result == "ok" && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentProjectid"]))
            {
                Response.Redirect("ProjectsLight");
            }
            else
            {
                errorMessage = result;
            }
             
        }
    }

}

