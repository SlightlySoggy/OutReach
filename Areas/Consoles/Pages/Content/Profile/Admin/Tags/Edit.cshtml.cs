using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Tags;
using Outreach.Pages.Utilities;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Tags
{ 

    [Authorize]
    public class EditModel : PageModel
    { 
        public string Tagid { get; set; }  
        public string Tagname { get; set; }  
        public String errorMessage = "";
        public String successMessage = "";

         
        public void OnGet()
        {
            String Id = Request.Query["Tagid"];
            String Tagname = Request.Query["Tagname"];

            this.Tagid = Id;
            this.Tagname = Tagname;  
             
            return;

        }
        public void OnPost()
        {
            Tag tag = new Tag();
            tag.Id = Request.Form["Tagid"];
            tag.TagName = Request.Form["Tagname"];
            tag.StatusId = "1";
            var result = tag.Save(tag.Id); 
            
            if (result == "ok")
            {  
                this.Tagid = tag.Id;
                this.Tagname = tag.TagName; 
                successMessage = "Tag name was changed successfully."; 
            }
            else
            {
                errorMessage = "Error occur, please try later.";
            }

            Response.Redirect("Index");
        }
    }
}