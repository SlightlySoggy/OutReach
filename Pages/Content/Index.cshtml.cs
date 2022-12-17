using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;

namespace Outreach.Pages.Content
{
    public class ClientViewModel : PageModel
    {
        public List<Opportunity> oppList { get; set; }
        public List<Tag> ListTag = new List<Tag>();
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();
            oppList = ut.GetRecentPostOpportunities(10);

            Tag tag = new Tag();
            ListTag = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
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
            if (Request.Form["submitbutton1"] != "")
            {
                clickedbuttonName = Request.Form["submitbutton1"];
            }
            else if (Request.Form["submitbutton2"] != "")
            {
                clickedbuttonName = Request.Form["submitbutton2"];
            }


            string txtSearch = "";
            if (Request.Form["TxtSearch1"] != "")
            {
                txtSearch = Request.Form["TxtSearch1"];
            }
            else if (Request.Form["TxtSearch2"] != "")
            {
                txtSearch = Request.Form["TxtSearch2"];
            }

            GeneralUtilities ut = new GeneralUtilities();

            if (txtSearch != null || Request.Form["chktag"] != "")
            {
                oppList = ut.SearchOpportunities(txtSearch, Request.Form["chktag"]);
            }
        }
        //Response.Redirect("Index");
    }
}
