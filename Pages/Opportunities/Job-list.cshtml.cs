using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Utilities;

namespace MyAffTest.Pages.Opportunities
{
    public class JoblistModel : PageModel
    {
         
        public List<Opportunity> oppList { get; set; }
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();

            oppList = ut.GetRecentPostOpportunities(10);
             
        }
    }
}