using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Packaging.Signing;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;
using System.Data.SqlClient;


namespace Outreach.Pages.Content
{
    public class ReportBugModel : PageModel
    {

        public ReportBug BugInfo = new ReportBug();  //instanate a new ReportBug class with defaut value
                                                     // public (filename) (variable) = new

        [BindProperty]
        public IFormFile Upload { get; set; }

        public string orgid = "";
        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        // Find sign in user's id START
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;

        public ReportBugModel(UserManager<ApplicationUser> userManager,
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

        }

        public IActionResult OnPostUploadFileAsync(IFormFile postedFile)  //upload file and save the bug info together
        {

            if (!string.IsNullOrWhiteSpace(Request.Form["hidBugId"]))
            {
                BugInfo.Id = Request.Form["hidBugId"];
            }

                string result = "";
            BugInfo.Name = Request.Form["inputName"];
            BugInfo.Email = Request.Form["inputEmail"];
            BugInfo.Subject = Request.Form["inputSubject"];
            BugInfo.Message = Request.Form["inputMessage"];
            BugInfo.CreateDate = DateTime.Now.ToString();
            BugInfo.UserId = Request.Form["hidUserId"];

            GeneralUtilities ut = new GeneralUtilities();
            // update existing Task
            string returnedBugId = "";
            returnedBugId = BugInfo.Save();
            string tmpFileID = "";

            if (ut.IsNumeric(returnedBugId))
            {
                result = "ok";
                BugInfo = new ReportBug(returnedBugId); 

                if (ut.IsNumeric(Request.Form["hidUserId"].ToString()))
                    user_id = Convert.ToInt32(Request.Form["hidUserId"].ToString()); 

                if (postedFile != null)
                {
                    if (BugInfo.FileId != null && BugInfo.FileId != "")
                    {
                        UploadFile uf = new UploadFile(BugInfo.FileId);
                        uf.Delete(); // delete old attachment.

                    }
                    tmpFileID = ut.SaveUploadFile(postedFile, "5", BugInfo.Id, "2", Request.Form["hidUserId"]);
                    if (ut.IsNumeric(tmpFileID))
                    {
                        BugInfo.FileId = tmpFileID;
                        result = "ok";
                    }
                }


            }
             
            //return RedirectToPage("#");




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

            return Page();

        }
         

    }
}
