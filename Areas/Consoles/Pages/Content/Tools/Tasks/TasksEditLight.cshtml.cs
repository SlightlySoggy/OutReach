
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
using Task = Outreach.Pages.Utilities.Task;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Tasks.TaskEdit
{
    [Authorize]//(Roles = "OrganizationContactor")]
    public class TasksEditLightModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public Task TaskInfo = new Task();

        public List<ProjTaskStatus> StatusList = new List<ProjTaskStatus>();
        public List<LoginUserInfo> TaskManagerUserList = new List<LoginUserInfo>();
        public List<LoginUserInfo> TaskMemberList = new List<LoginUserInfo>();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public int org_id = 2;
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public TasksEditLightModel(UserManager<ApplicationUser> userManager,
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

            if (!string.IsNullOrWhiteSpace(Request.Query["ProjectId"]))
            { // set project with query string
                TaskInfo.ProjectId = Request.Query["ProjectId"];   
            } 
            else
            { // select a project first
                Response.Redirect("ProjectsLight");
                //return Page();
            }


            if (string.IsNullOrWhiteSpace(Request.Query["TaskId"]))
            { // create a brand new Task 
                TaskInfo.CreatedOrgId = org_id.ToString();
                TaskInfo.CreatedUserId = user_id.ToString();
                return Page();
            }
            else
            { // load Task based on given id
                String TaskId = Request.Query["TaskId"];
                
                //hid_CurrentTaskid.value = "";

                Task op = new Task(TaskId);
                TaskInfo = op;

                TaskManagerUserList = ut.ResetProjectTaskUserList(LoginUserList, op.TaskManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                LoginUserList = ut.GetLoginUserList("");
                TaskMemberList = ut.ResetProjectTaskUserList(LoginUserList, op.TaskMemberUserIds);

                 
            }

            return Page();

        }
        public void OnPost()
        {
            string result = "";
             
            TaskInfo.Name = Request.Form["inputName"];
            TaskInfo.Description = Request.Form["inputDescription"];
            TaskInfo.EstimatedBudget = Request.Form["inputEstimatedBudget"];
            TaskInfo.ActualSpent = Request.Form["inputSpentBudget"];
            //TaskInfo.DurationByDay = Request.Form["inputEstimatedDuration"];
            TaskInfo.StartDate = Request.Form["inputStartDate"];
            TaskInfo.DueDate = Request.Form["inputDueDate"];
            TaskInfo.CompletionDate = Request.Form["inputCompletionDate"];

            if (!string.IsNullOrWhiteSpace(Request.Form["inputTaskStatus"]))
            {
                TaskInfo.ProjectTaskStatusId = Request.Form["inputTaskStatus"];
            }
            else
                TaskInfo.ProjectTaskStatusId = "1";



            if (string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTaskid"]))
            { //special for new Task
                TaskInfo.CreatedOrgId = Request.Form["hid_orgid"];
                TaskInfo.CreatedDate = DateTime.Now.ToString();
                TaskInfo.CreatedUserId = Request.Form["hid_userId"];
                //TaskInfo.TaskManagerUserId = Request.Form["hid_userId"]; 


                result = TaskInfo.Save(); // Insert a new Task


            }
            else
            {
                TaskInfo.Id = Request.Form["hid_CurrentTaskid"];

                //1 update the Task leader selection for existing Task 

                GeneralUtilities ut = new GeneralUtilities(); 
                Task existingTask = new Task(Request.Form["hid_CurrentTaskid"]);
                List<string> newUserLeadlist = Request.Form["inputTaskLeader"].ToString().Split(',').ToList(); 

                if (!ut.IsProjTaskMemberChanged(existingTask.TaskManagerUserIds,newUserLeadlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllProjectTaskUser(existingTask.Id, "", "true");                     

                    foreach (string uid in newUserLeadlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save Task level lead user
                            ProjectTaskUser ptu = new ProjectTaskUser();
                            ptu.TaskId = existingTask.Id;
                            ptu.TaskId = "";
                            ptu.UserId = uid;
                            ptu.IsLead = "1"; //leader
                            ptu.Save();
                        }                     
                    }
                }


                //2 update the Task member selection for existing Task  
                List<string> newMemberlist = Request.Form["inputTaskMember"].ToString().Split(',').ToList();

                if (!ut.IsProjTaskMemberChanged(existingTask.TaskMemberUserIds, newMemberlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllProjectTaskUser(existingTask.Id, "", "false");

                    foreach (string uid in newMemberlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save Task level lead user
                            ProjectTaskUser ptu = new ProjectTaskUser();
                            ptu.TaskId = existingTask.Id;
                            ptu.TaskId = "";
                            ptu.UserId = uid;
                            ptu.IsLead = ""; //regulare member
                            ptu.Save();
                        }
                    }
                }


                // update existing Task
                result = TaskInfo.Save();

            }

             

            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTaskid"]))
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                Response.Redirect("TasksEditLight?TaskId=" + Request.Form["hid_CurrentTaskid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            else if(result == "ok" && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTaskid"]))
            {
                Response.Redirect("TasksLight");
            }
            else
            {
                errorMessage = result;
            }
             
        }
    }

}

