using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Outreach.Pages.Utilities;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Tags
{
    [Authorize]
    // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/Tags?view=aspnetcore-6.0
    // https://www.yogihosting.com/aspnet-core-identity-Tags/
    public class TaglistModel : PageModel
    { 
        public List<Tag> Tags { get; set; } 

        public void OnGet()
        { 

            Tag tag = new Tag();
            Tags = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
        }
        public void OnPost()
        {
            Tag tag = new Tag(Request.Form["DeleteTagid"]);
            tag.Id = "-" + Request.Form["DeleteTagid"]; 
            tag.StatusId = "2"; // delete tag just set statusid=2
            var result = tag.Save(tag.Id);

            if (result == "ok")
            {
               // successMessage = "Tag name was changed successfully.";


                Response.Redirect("Index"); 
                Tags = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
            }
            else
            {
                Response.Redirect("Index");
                //errorMessage = "Error occur, please try later.";
            }
        }
    }
}