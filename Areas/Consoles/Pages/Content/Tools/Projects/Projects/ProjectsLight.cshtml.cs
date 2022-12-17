using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;

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

            if (Request.Query["Searchkeyword"] != "")
            {
                string txtSearch = Request.Query["Searchkeyword"];
                //ProjectList = ut.SearchOpportunities(txtSearch, "");
                ProjectList = pro.GetProjectListByStatusId("1"); //get all planned projects

                defaultsearchtext = txtSearch;
            }
            else
            {
                ProjectList = pro.GetProjectListByStatusId(""); //get all projects
            }

            //Tag tag = new Tag();
            //ListTag = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
        }

        //public IActionResult OnPostButton1(IFormCollection data)
        //{
        //    //...
        //}


        //public IActionResult OnPostButton2(IFormCollection data)
        //{
        //    //...
        //}

        public void OnPost()
        {
            // https://www.learnrazorpages.com/razor-pages/handler-methods
            string clickedbuttonName = "";
            Project pro = new Project();

            if (Request.Form["submitbutton1"] != "")
            {
                clickedbuttonName = Request.Form["submitbutton1"];
            }
            //else if (Request.Form["submitbutton2"] != "")
            //{ 
            //    clickedbuttonName = Request.Form["submitbutton2"];
            //}


            string txtSearch = "";
            if (Request.Form["TxtSearch1"] != "")
            {
                txtSearch = Request.Form["TxtSearch1"];
            }
            //else if (Request.Form["TxtSearch2"] != "")
            //{
            //    txtSearch = Request.Form["TxtSearch2"];
            //}

            GeneralUtilities ut = new GeneralUtilities();

            if (txtSearch != null || Request.Form["chktag"] != "")
            {
                //ProjectList = ut.SearchOpportunities(txtSearch, Request.Form["chktag"]);
                ProjectList = pro.GetProjectListByStatusId(""); //get all projects
            }
        }
        //Response.Redirect("Index");
    }
}
