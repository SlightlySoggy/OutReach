using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Clients;
using MyAffTest.Pages.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MyAffTest.Pages.Opportunities
{
    //[Authorize(Roles = "OrganizationContactor")]
    public class JobcreateModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public Opportunity oppInfo = new Opportunity();
        public int org_id = 1;
        public int user_id = 2;
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            orgInfo = new Organization(org_id.ToString());

            //orgInfo = GetOrganizationInfoByOrg_id();
            if (orgInfo == null)
            {

            }



        }
        public void OnPost()
        {
            oppInfo.OpportunityTitle = Request.Form["opptitle"];
            oppInfo.Description = Request.Form["oppdesc"];
            oppInfo.Responsibility = Request.Form["oppresp"];
            oppInfo.Requirement = Request.Form["oppreq"];
            oppInfo.CreatedOrgId = org_id.ToString();
            oppInfo.CreatedDate = DateTime.Now.ToString();
            oppInfo.CreatedUserId = user_id.ToString(); 
            oppInfo.StartDate = Request.Form["oppstartdate"];
            oppInfo.EndDate = Request.Form["oppenddate"];
            oppInfo.Schedule = Request.Form["oppschedule"]; 
            oppInfo.StatusId = "1";
            oppInfo.Tags = Request.Form["opptag"];

            string result = oppInfo.Save();

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
