using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;
using System.Data;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Departments.DepartmentLight
{
    public class DepartmentsLightModel : PageModel
    {

        public List<Department> DepartmentList { get; set; }
        //public PTStatus PTStatus = new PTStatus(tagid);
        public string defaultsearchtext = "";
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();
            Department pro = new Department();
            string txtSearch = ""; 

            if (!string.IsNullOrWhiteSpace(Request.Query["Searchkeyword"])) //if (Request.Query["Searchkeyword"] != "" && Request.Query["Searchkeyword"].Count != 0) //if (Request.Query["Searchkeyword"] != null && Request.Query["Searchkeyword"].ToString() != "")
            {
                txtSearch = Request.Query["Searchkeyword"];
                defaultsearchtext = txtSearch;
            }

            DepartmentList = ut.GetDepartmentListByNameSearch(txtSearch); //get Departments name contain search word or get all 
        }

        public void OnPost()
        {
            // https://www.learnrazorpages.com/razor-pages/handler-methods
            string result = "";
            Department pro = new Department();

            try
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["hid_DeleteDepartmentid"]))
                {
                    result = pro.Delete(Request.Form["hid_DeleteDepartmentid"]);
                }

                if (result == "ok")
                {
                    GeneralUtilities ut = new GeneralUtilities();
                    DepartmentList = ut.GetDepartmentListByNameSearch(""); //get all planned Departments
                    //Response.Redirect("DepartmentsLight");
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
             
} 