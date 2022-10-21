using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Utilities;

namespace MyAffTest.Pages.Opportunities
{
    public class JobdetailModel : PageModel
    {

        public Opportunity opportunity = new Opportunity();

        public void OnGet()
        {
            string oppid = Request.Query["OppId"]; 
            Opportunity op = new Opportunity(oppid);
            opportunity = op;
        }
    }
}