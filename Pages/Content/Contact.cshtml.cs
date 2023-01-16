using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Packaging.Signing;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;

namespace Outreach.Pages.Content
{
    public class ContactModel : PageModel
    {

        public ContactMessage ContactInfo = new ContactMessage();  //instanate a new ContactMessage class with defaut value
        // public (filename) (variable) = new

        public string orgid = ""; 
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        // Find sign in user's id START
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;

        public ContactModel(UserManager<ApplicationUser> userManager,
                      SignInManager<ApplicationUser> signInManager,
                      ILogger<JobdetailModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        // Find sign in user's id END
        public async Task<IActionResult> OnGetAsync() // Common error "not all code paths return a value" means there is no returned value
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Page();
            }
            GeneralUtilities ut = new GeneralUtilities();
            user_id = ut.GetLoginUserIntIDbyGUID(user.Id);


            return Page();

            //ContactInfo = new ContactMessage();
            //ContactInfo = new ContactMessage("1");

        }
        public void OnPost()
        {
            string result = "";

            ContactInfo.Name = Request.Form["inputName"];
            ContactInfo.Email = Request.Form["inputEmail"];
            ContactInfo.Subject = Request.Form["inputSubject"];
            ContactInfo.Message = Request.Form["inputMessage"];
            ContactInfo.CreateDate = DateTime.Now.ToString();
            ContactInfo.UserId = Request.Form["hidUserId"];

            // update existing Task
            result = ContactInfo.Save(); 

            if (result == "ok")
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                successMessage = "Contact was successfully sent.";

                //Response.Redirect("Contact");
                //Response.Redirect("Contact?TaskId=" + Request.Form["hid_CurrentTaskid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            //else if (result == "ok" && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentTaskid"]))
            //{
            //    Response.Redirect("TasksLight");
            //}
            else
            {
                errorMessage = "Something went wrong.";
            }

        }
    }

}