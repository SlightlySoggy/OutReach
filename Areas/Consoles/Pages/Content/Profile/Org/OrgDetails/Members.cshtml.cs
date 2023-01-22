using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Outreach.Data;
using Outreach.Pages.Utilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Org.OrgDetails
{
    public class MembersModel : PageModel
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<MembersModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public List<LoginUserInfo> OrgMemberList = new List<LoginUserInfo>();


        public Organization orgInfo = new Organization();
        public string orgId = "";
        public string ValidEmaillist = "";
        public string InvalidEmaillist = "";
        public List<Opportunity> oppList { get; set; }
        public List<Tag> ListTag = new List<Tag>();
        public List<string> SelectedTagIds = new List<string>();
        public string defaultsearchtext = "";

        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        //public MembersModel(UserManager<ApplicationUser> userManager) 
        //{
        //    _userManager = userManager; 
        //}

        public MembersModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<MembersModel> logger,
        RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            //_emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();

            if (!string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            { // must has an OrgID passed in    
                orgId = Request.Query["OrgId"];
                orgInfo = new Organization(orgId);

                OrgMemberList = ut.GetLoginUserList("1", orgId, "");
            }

        }

        public async Task<IActionResult> OnPostAsync(IFormFile postedFile)
        {
            string result = "";

            orgInfo.Name = Request.Form["inputName"];
            orgInfo.Description = Request.Form["inputDescription"];
            orgInfo.Email = Request.Form["inputEmail"];

            GeneralUtilities ut = new GeneralUtilities();

            if (string.IsNullOrWhiteSpace(Request.Form["hidOrgid"]))
            { //special for new Organization 
                orgInfo.CreatedDate = DateTime.Now.ToString();
                orgInfo.CreatedUserId = Request.Form["hidUserId"];

                result = orgInfo.Save(); // Insert a new Organization 
            }
            else
            {
                orgInfo.Id = Request.Form["hidOrgid"];

                result = orgInfo.Save();
            }
            orgInfo = new Organization(orgInfo.Id);

            string tmpFileID = "";

            if (postedFile != null)
            {
                if (orgInfo.Logo != null && orgInfo.Logo.Id != null && orgInfo.Logo.Id != "")
                {
                    UploadFile uf = new UploadFile(orgInfo.Logo.Id);
                    uf.Delete(); // delete old attachment.

                }
                tmpFileID = ut.SaveUploadFile(postedFile, "1", orgInfo.Id, "1", Request.Form["hidUserId"]);
                if (ut.IsNumeric(tmpFileID))
                {
                    orgInfo.Logo = new UploadFile(tmpFileID);
                    orgInfo.Logo.Id = tmpFileID;
                    result = "ok";
                }
            }

            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hidOrgid"]))
            {
                //Random random = new Random();
                //int randomNumber = random.Next(1000, 9999);

                //Response.Redirect("OrgSettings?OrgId=" + Request.Form["hidOrgid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");

                return Page();
            }
            //else if (result.Contains("failed") == false && string.IsNullOrWhiteSpace(Request.Form["hidOrgid"]))
            //{
            //    Response.Redirect("OrgSettings?Orgid=" + result);
            //}
            else
            {
                errorMessage = result;
            }
            return Page();

        }
        public void OnPostSearchMember()
        {
            GeneralUtilities ut = new GeneralUtilities();

            if (Request.Query["Searchkeyword"].ToString() != "")
            {
                string txtSearch = Request.Query["Searchkeyword"];
                oppList = ut.SearchOpportunities(txtSearch, "");

                defaultsearchtext = txtSearch;
            }
            else
            {
                oppList = new List<Opportunity> { };
            }
        }

        public async Task<IActionResult> OnPostAddMemberAsync(string rurl = "")
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            string returnUrl = "Members?OrgId=" + Request.Form["hidOrgid"] + "&Random=" + randomNumber.ToString(); //Url.Content("~/");

            if (!string.IsNullOrWhiteSpace(Request.Form["hidOrgid"]))
            {
                orgId = Request.Form["hidOrgid"];
            }
            else
            {
                Response.Redirect(returnUrl);
                return Page();
            }

            string result = "";
            List<string> validUserId = new List<string>();

            GeneralUtilities ut = new GeneralUtilities();

            if (Request.Form["MemberEmailList"].ToString() != "")
            {
                string emaillist = Request.Form["MemberEmailList"];
                string[] email = emaillist.Split(",");
                foreach (string e in email)
                {
                    int newUserId = 0;

                    if (ut.IsValidEmail(e))
                    {
                        ValidEmaillist += e + ", ";
                        newUserId = ut.GetLoginUserIntIDbyEmail(e);
                        if (newUserId == 0)
                        { // if username not exist then insert a new user with empty first and last name, default password Happy_2Spring

                            var user = new ApplicationUser { UserName = e, Email = e, firstName = "", lastName = "" };
                            await _userStore.SetUserNameAsync(user, e, CancellationToken.None);
                            var result2 = await _userManager.CreateAsync(user, "Happy_2Spring");

                            if (result2.Succeeded)
                            {
                                newUserId = ut.GetLoginUserIntIDbyGUID(user.Id);
                            }
                        }
                        else
                        { //user already registered with the email; 
                        }
                        validUserId.Add(newUserId.ToString());
                    }
                    else
                    {
                        InvalidEmaillist += e + ", ";
                    }
                }
            }

            if (ValidEmaillist != "")
            {
                ValidEmaillist = "New Member added: " + ValidEmaillist;

                //insert into Organization UserLinkage

                //2 update the Organization member selection for existing Project  
                //List<string> newMemberlist = Request.Form["inputProjectMember"].ToString().Split(',').ToList();

                ////remove lead user from the member selection  
                //List<string> newMemberlist2 = new List<string>();
                //var differentMembers2 = newMemberlist.Where(p2 => newUserLeadlist.All(p => p2 != p)).ToList<string>();
                //differentMembers2.ForEach(u => newMemberlist2.Add(u));

                List<UserLinkage> memberTobeDeletedIds = new List<UserLinkage>(); //nothing to be deleted, only add new here

                result = ut.ProcessLinkedUsers(memberTobeDeletedIds, validUserId, "1", orgId, "0");

            }
            if (InvalidEmaillist != "")
            {
                InvalidEmaillist = "Invalid Email: " + InvalidEmaillist;
            }
            Response.Redirect(returnUrl);
            return Page();
        }
        public void OnPostAddMember2()
        {
            string clickedbuttonName = "";

            GeneralUtilities ut = new GeneralUtilities();

            if (Request.Form["MemberEmailList"].ToString() != "")
            {
                string emaillist = Request.Form["MemberEmailList"];
                string[] email = emaillist.Split(",");
                foreach (string e in email)
                {
                    if (ut.IsValidEmail(e))
                    {
                        ValidEmaillist += e + ", ";
                    }
                    else
                    {
                        InvalidEmaillist += e + ", ";
                    }



                }

                if (ValidEmaillist != "")
                {
                    ValidEmaillist = "New Member added: " + InvalidEmaillist;
                }
                if (InvalidEmaillist != "")
                {
                    InvalidEmaillist = "Invalid Email: " + InvalidEmaillist;
                }
                //oppList = ut.SearchOpportunities(txtSearch, "");

                //defaultsearchtext = txtSearch;
            }
            else
            {
                oppList = new List<Opportunity> { };
            }


            if (Request.Form["submitbutton1"] != "")
            {
                clickedbuttonName = Request.Form["submitbutton1"];
            }
            //else if (Request.Form["submitbutton2"] != "")
            //{ 
            //    clickedbuttonName = Request.Form["submitbutton2"];
            //}


            string txtSearch = "";
            if (Request.Form["TxtSearch"] != "")
            {
                defaultsearchtext = Request.Form["TxtSearch"];
            }


            if (txtSearch != null || Request.Form["chkTag"] != "")
            {
                SelectedTagIds = Request.Form["chkTag"].ToString().Split(',').ToList();
                oppList = ut.SearchOpportunities(defaultsearchtext, Request.Form["chkTag"], Request.Form["chkorderby"]);
            }

            Tag tag = new Tag();
            ListTag = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
        }
        //Response.Redirect("Index");
    }
}
