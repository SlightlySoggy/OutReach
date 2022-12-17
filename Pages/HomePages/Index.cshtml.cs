using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;

namespace Outreach.Pages.HomePages
{
    public class IndexModel : PageModel
    { 

        public void OnGet()
        {

        }
        public void OnPost()
        { 

            string txtSearch = "";
            if (Request.Form["TxtSearch1"] != "")
            {
                txtSearch = Request.Form["TxtSearch1"];
            }

            Response.Redirect("/Job-list?Searchkeyword=" + txtSearch.Trim());
        } 
    }
}
