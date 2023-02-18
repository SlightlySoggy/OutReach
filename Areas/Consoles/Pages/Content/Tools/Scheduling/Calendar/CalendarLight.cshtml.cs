using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Outreach.Areas.Consoles.Pages.Content.Tools.Scheduling.Calendar
{
    public class CalendarLightModel : PageModel
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
