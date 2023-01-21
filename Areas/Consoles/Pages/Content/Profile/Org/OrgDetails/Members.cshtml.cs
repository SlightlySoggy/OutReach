using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Outreach.Pages.Utilities;
using System.Security.Cryptography;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Org.OrgDetails
{
    public class MembersModel : PageModel
    {

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
        public void OnGet()
        {
            GeneralUtilities ut = new GeneralUtilities();

            if (!string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            { // must has an OrgID passed in    
                orgId = Request.Query["OrgId"];
                orgInfo = new Organization(orgId);
            }

            Tag tag = new Tag();
            ListTag = tag.GetReferencedTagsbyOpptunityId(""); // get all active tags
        }
         
        public async Task<IActionResult> OnPostAsync(IFormFile postedFile)
        {
            string result = "";

            orgInfo.Name = Request.Form["inputName"];
            orgInfo.Description = Request.Form["inputDescription"];
            orgInfo.Email = Request.Form["inputEmail"];

            //if (!string.IsNullOrWhiteSpace(Request.Form["inputOrganizationStatus"]))
            //{
            //    orgInfo.OrganizationTaskStatusId = Request.Form["inputOrganizationStatus"];
            //}
            //else
            //    orgInfo.OrganizationTaskStatusId = "1";

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
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

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
        public void OnPostAddMember()
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
