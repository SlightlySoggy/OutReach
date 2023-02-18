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
using Microsoft.AspNetCore.Http;

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
        public string OrgId = "";
        public string UserId = "";
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
            if (!string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            { // must has an OrgId passed in    
                OrgId = Request.Query["OrgId"];
                orgInfo = new Organization(OrgId); 

            }
            OrgMemberList = GeMemberByCondition(); 

        }

        private List<LoginUserInfo> GeMemberByCondition(string searchType = "", string txtSearch = "")
        {
            List<LoginUserInfo> returnUserList = new List<LoginUserInfo>();
            GeneralUtilities ut = new GeneralUtilities();  

            if (searchType == "2")
            { // search primary contact only 
                returnUserList.Add(orgInfo.PrimaryContactUser);
            }
            else if(searchType == "3")
            { // search lead only 
                returnUserList = ut.GetLoginUserList("1", OrgId, "1");  //get all lead members in this organization 
            }
            else if (searchType == "4")
            { // search Regular Member only 
                returnUserList = ut.GetLoginUserList("1", OrgId, "0");  //get all regular members in this organization 
            } 
            else
            {
                returnUserList = ut.GetLoginUserList("1", OrgId, "");  //get all members in this organization 
            }

            if (txtSearch != "") // if the search bar is not empty then search contacts via email/first name/lastname
            {
                returnUserList = returnUserList.FindAll(x=>x.firstName.Contains(txtSearch) || x.lastName.Contains(txtSearch) || x.Email.Contains(txtSearch)).ToList();
            }

            return returnUserList;

        }


        public async Task<IActionResult> OnPostSearchMemberAsync(string rurl = "")
        {
            string searchType = Request.Form["selSearchType"];
            string txtSearch = Request.Form["Searchkeyword"];
            if (!string.IsNullOrWhiteSpace(Request.Form["hidOrgId1"]))
            { // must has an OrgId passed in    
                OrgId = Request.Form["hidOrgId1"];
                orgInfo = new Organization(OrgId);

            }

            OrgMemberList = GeMemberByCondition(searchType, txtSearch);
            return Page();

        }

        public async Task<IActionResult> OnPostAddMemberAsync(string rurl = "")
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            string returnUrl = "Members?OrgId=" + Request.Form["hidOrgId"] + "&Random=" + randomNumber.ToString(); //Url.Content("~/");

            if (!string.IsNullOrWhiteSpace(Request.Form["hidOrgId"]))
            {
                OrgId = Request.Form["hidOrgId"];
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

                result = ut.ProcessLinkedUsers(memberTobeDeletedIds, validUserId, "1", OrgId, "0");

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
                //oppList = ut.getOpportunityListByCondition(txtSearch, "");

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
                oppList = ut.getOpportunityListByCondition(defaultsearchtext, Request.Form["chkTag"], Request.Form["chkorderby"]);
            }

            Tag tag = new Tag();
            ListTag = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
        }
        //Response.Redirect("Index");
    }
}
