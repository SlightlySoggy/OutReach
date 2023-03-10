
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NuGet.Packaging.Signing;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;
using System.Security.Cryptography;
using Project = Outreach.Pages.Utilities.Project;
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

        public string TaskBelongTo ="";

        public string orgid = "";
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


            TaskInfo.CreatedUserId = user_id.ToString();

            List <LoginUserInfo> LoginUserList = new List<LoginUserInfo>(); 
            LoginUserList = ut.GetLoginUserList("1", orgid, ""); // get all users belong to parent level (organization)


            //user_id = Convert.ToInt32(user.User_Id);


            //PTStatus ptStatus = new PTStatus();
            //ListTag = PTStatus.GetReferencedTagsbyOpptunityId(""); // get all active Status

            //orgInfo = GetOrganizationInfoByorgid();
            if (orgInfo == null)
            {

            }  

            if (string.IsNullOrWhiteSpace(Request.Query["TaskId"]))
            { // create a brand new Task 


                if (!string.IsNullOrWhiteSpace(Request.Query["OrganizationId"]))
                { // set OrganizationId with query string
                    TaskInfo.TaskLinkage.OrganizationId = Request.Query["OrganizationId"];
                    Organization org = new Organization(Request.Query["OrganizationId"]);
                    TaskBelongTo = "Organization (" + org.Name + ")";
                }
                else if (!string.IsNullOrWhiteSpace(Request.Query["TeamId"]))
                { // set TeamId with query string
                    TaskInfo.TaskLinkage.TeamId = Request.Query["TeamId"];
                    Team t = new Team(Request.Query["ProjectId"]);
                    TaskBelongTo = "Team (" + t.Name + ")";
                }
                else if (!string.IsNullOrWhiteSpace(Request.Query["ProjectId"]))
                { // set project with query string
                    TaskInfo.TaskLinkage.ProjectId = Request.Query["ProjectId"];
                    Project pr = new Project(Request.Query["ProjectId"]);
                    TaskBelongTo = "Project (" + pr.ProjectName + ")";
                }
                else
                { // remind client to create task from a parent group: Organization, Team, Project
                    //Response.Redirect("TasksLight");
                    //Response.Redirect("ProjectsLight"); 
                }
                 
                TaskInfo.CreatedUserId = user_id.ToString();
                return Page();
            }
            else
            { // load Task based on given id
                String TaskId = Request.Query["TaskId"];
                
                //hid_CurrentTaskid.value = "";

                Task op = new Task(TaskId);
                TaskInfo = op;
                TaskBelongTo = op.TaskLinkage.BelongTo;


                TaskManagerUserList = ut.ResetUserLinkageList(LoginUserList, op.TaskManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                LoginUserList = ut.GetLoginUserList("1", orgid, ""); // get all users belong to parent level (organization)
                TaskMemberList = ut.ResetUserLinkageList(LoginUserList, op.TaskMemberUserIds);

                 
            }

            return Page();

        }
        public void OnPost()
        {
            string result = "";
             
            TaskInfo.Name = Request.Form["inputName"];
            TaskInfo.Description = Request.Form["inputDescription"]; 
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
                TaskInfo.CreatedDate = DateTime.Now.ToString();
                TaskInfo.CreatedUserId = Request.Form["hid_userId"];

                TaskInfo.TaskLinkage.OrganizationId = Request.Form["hid_CurrentOrganizationId"]; //must has a projectid
                TaskInfo.TaskLinkage.TeamId = Request.Form["hid_CurrentTeamId"]; //must has a projectid
                TaskInfo.TaskLinkage.ProjectId = Request.Form["hid_CurrentProjectId"]; //must has a projectid

                if (TaskInfo.TaskLinkage.OrganizationId != "" || TaskInfo.TaskLinkage.ProjectId != "" || TaskInfo.TaskLinkage.TeamId == "")
                { // task must and only link to one of the higher group: Organization,Project, Team
                    result = TaskInfo.Save(); // Insert a new Task
                }



            }
            else
            {
                TaskInfo.Id = Request.Form["hid_CurrentTaskid"];

                //1 update the Task leader selection for existing Task 

                GeneralUtilities ut = new GeneralUtilities(); 
                Task existingTask = new Task(Request.Form["hid_CurrentTaskid"]);
                List<string> newUserLeadlist = Request.Form["inputTaskLeader"].ToString().Split(',').ToList();

                result = ut.ProcessLinkedUsers(existingTask.TaskManagerUserIds, newUserLeadlist,"4", TaskInfo.Id, "1"); 


                //2 update the Task member selection for existing Task  
                List<string> newMemberlist = Request.Form["inputTaskMember"].ToString().Split(',').ToList();

                //remove lead user from the member selection  
                List<string> newMemberlist2 = new List<string>();
                var differentMembers2 = newMemberlist.Where(p2 => newUserLeadlist.All(p => p2 != p)).ToList<string>();
                differentMembers2.ForEach(u => newMemberlist2.Add(u));

                result = ut.ProcessLinkedUsers(existingTask.TaskMemberUserIds, newMemberlist2, "4", TaskInfo.Id, "0");


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