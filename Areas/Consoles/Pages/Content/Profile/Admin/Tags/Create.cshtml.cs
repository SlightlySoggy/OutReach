using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Outreach.Pages.Utilities;
//using Outreach.Pages.Clients;
using System.Data;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Tags
{
    public class CreateModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {  
        }
        public void OnPost()
        {
            Tag tag = new Tag();
            tag.Id = "0";
            tag.TagName = Request.Form["tagname"];
            tag.StatusId = "1";
            var result = "";

            if (Request.Form["tagname"].ToString().Trim()!="")
            { 
            result = tag.Save(tag.Id);

            }

            if (result == "ok")
            { 
                successMessage = "Tag name was changed successfully.";


                //Response.Redirect("Index");
            }
            else
            {
                errorMessage = "Error occur, please try later.";
            }
        }
    }
}