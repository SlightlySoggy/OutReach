
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

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Teams.TeamEdit
{
    [Authorize]//(Roles = "OrganizationContactor")]
    public class TeamsEditLightModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public Team TeamInfo = new Team();

        public List<StandardStatus> StatusList = new List<StandardStatus>();
        public List<LoginUserInfo> TeamManagerUserList = new List<LoginUserInfo>();
        public List<LoginUserInfo> TeamMemberList = new List<LoginUserInfo>();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public string orgid = "0";
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public TeamsEditLightModel(UserManager<ApplicationUser> userManager,
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
            LoginUserList = ut.GetLoginUserList("1", orgid,""); // get all users belong to parent level (organization)

            //user_id = Convert.ToInt32(user.User_Id);


            //PTStatus ptStatus = new PTStatus();
            //ListTag = PTStatus.GetReferencedTagsbyOpptunityId(""); // get all active Status

            //orgInfo = GetOrganizationInfoByorgid();
            if (!string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            {
                orgid = Request.Query["OrgId"];
            }
            else
            {
               // redirect to a page to specify the orgination first
            }

            if (string.IsNullOrWhiteSpace(Request.Query["TeamId"]))
            { // create a brand new Team 
                TeamInfo.OrganizationId = orgid.ToString();
                TeamInfo.CreatedUserId = user_id.ToString();
                return Page();
            }
            else
            { // load Team based on given id
                String TeamId = Request.Query["TeamId"];
                
                //hid_CurrentTeamid.value = "";

                Team op = new Team(TeamId);
                TeamInfo = op;

                TeamManagerUserList = ut.ResetTeamUserList(LoginUserList, op.TeamManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist  
                LoginUserList = ut.GetLoginUserList("1", orgid, ""); // get all users belong to parent level (organization)
                TeamMemberList = ut.ResetTeamUserList(LoginUserList, op.TeamMemberUserIds);

                 
    }

            return Page();

        }
        public void OnPost()
        {
            string result = "";
             
            TeamInfo.Name = Request.Form["inputName"];
            TeamInfo.Description = Request.Form["inputDescription"];  

            if (!string.IsNullOrWhiteSpace(Request.Form["inputTeamStatus"]))
            {
                TeamInfo.StatusId = Request.Form["inputTeamStatus"];
            }
            else
                TeamInfo.StatusId = "1";



            if (string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTeamid"]))
            { //special for new Team
                TeamInfo.OrganizationId = Request.Form["hid_orgid"];
                TeamInfo.CreatedDate = DateTime.Now.ToString();
                TeamInfo.CreatedUserId = Request.Form["hid_userId"];
                //TeamInfo.TeamManagerUserId = Request.Form["hid_userId"]; 


                result = TeamInfo.Save(); // Insert a new Team


            }
            else
            {
                TeamInfo.Id = Request.Form["hid_CurrentTeamid"];

                //1 update the Team leader selection for existing Team 

                GeneralUtilities ut = new GeneralUtilities(); 
                Team existingTeam = new Team(Request.Form["hid_CurrentTeamid"]);
                List<string> newUserLeadlist = Request.Form["inputTeamLeader"].ToString().Split(',').ToList(); 

                if (!ut.IsTeamMemberChanged(existingTeam.TeamManagerUserIds,newUserLeadlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllTeamUser(existingTeam.Id, "true");                     

                    foreach (string uid in newUserLeadlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save Team level lead user
                            TeamUser ptu = new TeamUser();
                            ptu.TeamId = existingTeam.Id; 
                            ptu.UserId = uid;
                            ptu.IsLead = "1"; //leader
                            ptu.Save();
                        }                     
                    }
                }


                //2 update the Team member selection for existing Team  
                List<string> newMemberlist = Request.Form["inputTeamMember"].ToString().Split(',').ToList();

                if (!ut.IsTeamMemberChanged(existingTeam.TeamMemberUserIds, newMemberlist))
                {
                    // if member changed, then delete all old selection and add new selected users again

                    result = ut.DeleteAllTeamUser(existingTeam.Id, "false");

                    foreach (string uid in newMemberlist)
                    {
                        if (ut.IsNumeric(uid))
                        { // save Team level lead user
                            TeamUser ptu = new TeamUser();
                            ptu.TeamId = existingTeam.Id; 
                            ptu.UserId = uid;
                            ptu.IsLead = ""; //regulare member
                            ptu.Save();
                        }
                    }
                }


                // update existing Team
                result = TeamInfo.Save();

            }

             

            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTeamid"]))
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                Response.Redirect("TeamEditLight?TeamId=" + Request.Form["hid_CurrentTeamid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            else if(result == "ok" && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTeamid"]))
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

