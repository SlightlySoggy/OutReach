using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;
using System.Data;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Teams.TeamLight
{
    public class TeamsLightModel : PageModel
    {

        public List<Team> TeamList { get; set; }
        //public PTStatus PTStatus = new PTStatus(tagid);
        public string defaultsearchtext = "";
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();
            Team pro = new Team();
            string txtSearch = ""; 

            if (!string.IsNullOrWhiteSpace(Request.Query["Searchkeyword"])) //if (Request.Query["Searchkeyword"] != "" && Request.Query["Searchkeyword"].Count != 0) //if (Request.Query["Searchkeyword"] != null && Request.Query["Searchkeyword"].ToString() != "")
            {
                txtSearch = Request.Query["Searchkeyword"];
                defaultsearchtext = txtSearch;
            }

            TeamList = ut.GetTeamListByNameSearch(txtSearch); //get Teams name contain search word or get all 
        }

        public void OnPost()
        {
            // https://www.learnrazorpages.com/razor-pages/handler-methods
            string result = "";
            Team pro = new Team();

            try
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["hid_DeleteTeamid"]))
                {
                    result = pro.Delete(Request.Form["hid_DeleteTeamid"]);
                }

                if (result == "ok")
                {
                    GeneralUtilities ut = new GeneralUtilities();
                    TeamList = ut.GetTeamListByNameSearch(""); //get all planned Teams
                    //Response.Redirect("TeamsLight");
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
             
} 