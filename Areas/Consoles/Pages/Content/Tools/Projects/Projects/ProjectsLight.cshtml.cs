using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;
using System.Data;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Projects.ProjectLight
{
    public class ProjectsLightModel : PageModel
    {

        public List<Project> ProjectList { get; set; }
        //public PTStatus PTStatus = new PTStatus(tagid);
        public string defaultsearchtext = "";
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();
            Project pro = new Project();

            if (!string.IsNullOrWhiteSpace(Request.Query["Searchkeyword"])) //if (Request.Query["Searchkeyword"] != "" && Request.Query["Searchkeyword"].Count != 0) //if (Request.Query["Searchkeyword"] != null && Request.Query["Searchkeyword"].ToString() != "")
            {
                string txtSearch = Request.Query["Searchkeyword"];
                defaultsearchtext = txtSearch;

                ProjectList = ut.GetProjectListByNameSearch(txtSearch); //get projects name contain search word
            }
            else if (!string.IsNullOrWhiteSpace(Request.Query["statusid"])) // 
            {
                ProjectList = ut.GetProjectListByStatusId(Request.Query["statusid"]); //get projects list with specified status
            }
            else
            {
                ProjectList = ut.GetProjectListByStatusId(""); //get all projects
            }

        }

        public void OnPost()
        {
            // https://www.learnrazorpages.com/razor-pages/handler-methods
            string result = "";
            Project pro = new Project();

            try
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["hid_DeleteProjectid"]))
                {
                    result = pro.Delete(Request.Form["hid_DeleteProjectid"]);
                }

                if (result == "ok")
                {
                    GeneralUtilities ut = new GeneralUtilities();
                    ProjectList = ut.GetProjectListByNameSearch(""); //get all planned projects
                    //Response.Redirect("ProjectsLight");
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
             
} 