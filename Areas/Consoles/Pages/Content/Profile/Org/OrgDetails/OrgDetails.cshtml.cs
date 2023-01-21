using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Linq;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Org.OrgDetails
{
    [Authorize]
    //[Authorize(Roles = "Administrator")]

    public class OrgDetailsModel : PageModel
    {
        public string OrgId = "";
        public string UserId = "";
        public Organization OrgInfo = new Organization();
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;

        public OrgDetailsModel(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              ILogger<JobdetailModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to find signin user info.");
                return Page();
            }
            GeneralUtilities ut = new GeneralUtilities();
            UserId = ut.GetLoginUserIntIDbyGUID(user.Id).ToString();



            //List<LoginUserInfo> LoginUserList = new List<LoginUserInfo>();
            //LoginUserList = ut.GetLoginUserList("1", "", ""); // get all users belong to parent level (organization)


            if (string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            {
                Response.Redirect("Dashboardpersonal"); 
            }
            else
            { // load Organization based on given id

                OrgId = Request.Query["OrgId"];
                if (ut.IsUserAuthorizedtoAccessOrganization(user.Id, OrgId) == false)
                { 
                    Response.Redirect("Dashboardpersonal");

                    return Page();
                }
                OrgInfo = new Organization(OrgId);

                //hid_CurrentOrgId.value = "";

                Organization op = new Organization(OrgId);
                OrgInfo = op;

                //OrganizationManagerUserList = ut.ResetUserLinkageList(LoginUserList, op.OrganizationManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                //LoginUserList = ut.GetLoginUserList("");
                //OrganizationMemberList = ut.ResetUserLinkageList(LoginUserList, op.OrganizationMemberUserIds);


            }

            return Page();

        }
        //public IActionResult OnPostUpload(FileUpload fileUpload)
        //{
        //    Creating upload folder
        //    if (!Directory.Exists(fullPath))
        //    {
        //        Directory.CreateDirectory(fullPath);
        //    }
        //    var formFile = fileUpload.FormFile;
        //    var filePath = Path.Combine(fullPath, formFile.FileName);

        //    using (var stream = System.IO.File.Create(filePath))
        //    {
        //        formFile.CopyToAsync(stream);
        //    }

        //    Process uploaded files
        //    Don't rely on or trust the FileName property without validation.
        //    ViewData["SuccessMessage"] = formFile.FileName.ToString() + " files uploaded!!";
        //    return Page();
        //}
        //public class FileUpload
        //{
        //    [Required]
        //    [Display(Name = "File")]
        //    public IFormFile FormFile { get; set; }
        //    public string SuccessMessage { get; set; }
        //}

        public async Task<IActionResult> OnPostAsync()
        {

            return Page();


        }
    }
}
