using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;

namespace Outreach.Areas.Identity.Pages.RegisterOrganization
{
    public class OrganizationListModel : PageModel
    {
        public Organization orgInfo = new Organization();

        public List<Organization> OrganizationList { get; set; }
        //public PTStatus PTStatus = new PTStatus(tagid);
        public string defaultsearchtext = "";
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();
            Organization pro = new Organization();
            string txtSearch = "";

            if (!string.IsNullOrWhiteSpace(Request.Query["Searchkeyword"])) //if (Request.Query["Searchkeyword"] != "" && Request.Query["Searchkeyword"].Count != 0) //if (Request.Query["Searchkeyword"] != null && Request.Query["Searchkeyword"].ToString() != "")
            {
                txtSearch = Request.Query["Searchkeyword"];
                defaultsearchtext = txtSearch;
            }

            OrganizationList = ut.GetOrganizationListByNameSearch(txtSearch); //get Organizations name contain search word or get all 
        }

        public void OnPost()
        {
            // https://www.learnrazorpages.com/razor-pages/handler-methods
            string result = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["hid_DeleteOrganizationid"]))
                {
                    Organization pro = new Organization(Request.Form["hid_DeleteOrganizationid"]);
                    pro.StatusId = "3";  // 3: Deleted in StandardStatus table
                    result = pro.Save();
                }

                if (result == "ok")
                {
                    GeneralUtilities ut = new GeneralUtilities();
                    OrganizationList = ut.GetOrganizationListByNameSearch(""); //get all planned Organizations
                    //Response.Redirect("OrganizationsLight");
                }

            }
            catch (Exception ex)
            {
            }
        }
    }

}