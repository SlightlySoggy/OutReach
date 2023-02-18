using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Team
{
    public class TeamDetailModel : PageModel
    {

        public string OrgId = "";
        public string TeamId = "";
        public string UserId = "";
        public string ValidEmaillist = "";
        public string InvalidEmaillist = "";

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(Request.Query["TeamId"]))
            {
                TeamId = Request.Query["TeamId"];

            }
        }
    }
}
