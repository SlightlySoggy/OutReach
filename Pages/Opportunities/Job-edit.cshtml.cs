using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Diagnostics;
//using Outreach.Pages.Clients;
using Outreach.Pages.Utilities;
using System;
using System.Data;
using System.Data.SqlClient; 
using Outreach.Data;


namespace Outreach.Pages.Opportunities
{
    [Authorize]//(Roles = "OrganizationContactor")]
    public class JobcreateModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public Opportunity oppInfo = new Opportunity();
        public List <Tag> ListTag = new List<Tag>(); 

        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;


        public int org_id = 1;
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public JobcreateModel(UserManager<ApplicationUser> userManager,
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
            GeneralUtilities generalUtilities = new GeneralUtilities();
            user_id = generalUtilities.GetLoginUserIntIDbyGUID(user.Id); 

            //user_id = Convert.ToInt32(user.User_Id);

            oppInfo.CreatedUserId = user_id.ToString();


            Tag tag = new Tag();
            ListTag = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags

            //orgInfo = GetOrganizationInfoByOrg_id();
            if (orgInfo == null)
            {

            }

            if (string.IsNullOrWhiteSpace(Request.Query["OppId"]))
            { // create a brand new opportunity 
                return Page();
            }
            else
            { // load opportunity based on given id
                String oppid = Request.Query["OppId"];
                Opportunity op = new Opportunity(oppid);
                oppInfo = op; 
            }

            return Page();

        }
        public void OnPost()
        {  
            oppInfo.OpportunityTitle = Request.Form["opptitle"];
            oppInfo.Description = Request.Form["oppdesc"];
            oppInfo.Responsibility = Request.Form["oppresp"];
            oppInfo.Requirement = Request.Form["oppreq"];
            oppInfo.SiteAddress = Request.Form["oppsiteadd"];
            oppInfo.CreatedOrgId = org_id.ToString();
            oppInfo.CreatedDate = DateTime.Now.ToString();
            oppInfo.CreatedUserId = Request.Form["hid_userId"];
            oppInfo.StartDate = Request.Form["oppstartdate"];
            oppInfo.EndDate = Request.Form["oppenddate"];
            oppInfo.Schedule = Request.Form["oppschedule"]; 
            oppInfo.StatusId = "1";
           // oppInfo.Tags = Request.Form["opptag"];

            string result = "";

            if (!string.IsNullOrWhiteSpace(Request.Form["chktag"]))
            { // multiple selction value can be [4,12,22]
                List <string> listTagid = Request.Form["chktag"].ToString().Split(",").ToList();
                foreach (string tagid in listTagid)
                { 
                    Tag tag = new Tag(tagid);
                    oppInfo.Tags.Add(tag);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["hid_oppId"]))
            { // because the hid_oppId is empty, it will do insert here
                result = oppInfo.Save("");
            }
            else
            { // because there is already an opportunity id, it means we will override/change the existing details
                result = oppInfo.Save(Request.Form["hid_oppId"]);
            }  

            if (result == "ok")
            {
                Response.Redirect("Job-list");
            }
            else
            {
                errorMessage = result;
            }

            //Response.Redirect("Index");
        }
    }

}
