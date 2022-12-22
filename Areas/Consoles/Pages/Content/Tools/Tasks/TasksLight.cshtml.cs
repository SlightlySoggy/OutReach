using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.ObjectModelRemoting;
using NuGet.Packaging.Signing;
using Outreach.Pages.Utilities;
using System.Data;
using Task = Outreach.Pages.Utilities.Task;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Tasks.TaskLight
{
    public class TasksLightModel : PageModel
    {

        public List<Task> TaskList { get; set; }
        public List<Project> ProjectList { get; set; }
        
        //public PTStatus PTStatus = new PTStatus(tagid);
        public string defaultsearchtext = "";
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();
            ProjectList = ut.GetProjectListByStatusId("-1"); // get planed and in progress projects list


            //if (!string.IsNullOrWhiteSpace(Request.Query["ProjectId"]))
            //{ // set project with query string
            //    TaskInfo.ProjectId = Request.Query["ProjectId"];
            //}
            //else
            //{ // select a project first
            //    Response.Redirect("ProjectsLight");
            //    //return Page();
            //}



            Task pro = new Task();

            if (!string.IsNullOrWhiteSpace(Request.Query["Searchkeyword"])) //if (Request.Query["Searchkeyword"] != "" && Request.Query["Searchkeyword"].Count != 0) //if (Request.Query["Searchkeyword"] != null && Request.Query["Searchkeyword"].ToString() != "")
            {
                string txtSearch = Request.Query["Searchkeyword"];
                defaultsearchtext = txtSearch;

                TaskList = ut.GetTaskListByNameSearch(txtSearch); //get Tasks name contain search word
            }
            else if (!string.IsNullOrWhiteSpace(Request.Query["statusid"])) // 
            {
                TaskList = ut.GetTaskListByStatusId(Request.Query["statusid"]); //get Tasks list with specified status
            }
            else
            {
                TaskList = ut.GetTaskListByStatusId(""); //get all Tasks
            }

        }

        public void OnPost()
        {
            // https://www.learnrazorpages.com/razor-pages/handler-methods
            string result = "";
            Task pro = new Task();

            try
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["hid_DeleteTaskid"]))
                {
                    result = pro.Delete(Request.Form["hid_DeleteTaskid"]);
                }

                if (result == "ok")
                {
                    GeneralUtilities ut = new GeneralUtilities();
                    TaskList = ut.GetTaskListByNameSearch(""); //get all planned Tasks
                    //Response.Redirect("TasksLight");
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
             
} 