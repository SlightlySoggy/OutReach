using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Outreach.Data;
using Outreach.Pages.Opportunities;
using Outreach.Pages.Utilities;
using System.Drawing;

using System.IO;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace Outreach.Areas.Identity.Pages.RegisterOrg
{
    public class OrganizationSettingsModel : PageModel
    {
        public Organization orgInfo = new Organization();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JobdetailModel> _logger;

        
        [BindProperty]
        public IFormFile Upload { get; set; }

        public int user_id = 0;
        public string errorMessage = "";
        public string successMessage = "";

        public OrganizationSettingsModel(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              ILogger<JobdetailModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        public async Task<IActionResult> OnPostUploadfileAsync()
        {
            string a = "a";
            return Page();
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
            user_id = ut.GetLoginUserIntIDbyGUID(user.Id);

            //List<LoginUserInfo> LoginUserList = new List<LoginUserInfo>();
            //LoginUserList = ut.GetLoginUserList("1", "", ""); // get all users belong to parent level (organization)


            if (string.IsNullOrWhiteSpace(Request.Query["OrgId"]))
            { // create a brand new Organization  
                //orgInfo.CreatedDate = ??????????????????????????????
                orgInfo.CreatedUserId = user_id.ToString();
                orgInfo.StatusId = "1";
                return Page();
            }
            else
            { // load Organization based on given id

                String OrgId = Request.Query["OrgId"];
                orgInfo = new Organization(OrgId);

                //hid_CurrentOrgId.value = "";

                Organization op = new Organization(OrgId);
                orgInfo = op;

                //OrganizationManagerUserList = ut.ResetUserLinkageList(LoginUserList, op.OrganizationManagerUserIds);

                //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist 
                //LoginUserList = ut.GetLoginUserList("");
                //OrganizationMemberList = ut.ResetUserLinkageList(LoginUserList, op.OrganizationMemberUserIds);


            }

            return Page();

        }

        public void OnPost()
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



            if (string.IsNullOrWhiteSpace(Request.Form["hid_CurrentOrgid"]))
            { //special for new Organization 
                orgInfo.CreatedDate = DateTime.Now.ToString();
                orgInfo.CreatedUserId = Request.Form["hid_userId"];

                result = orgInfo.Save(); // Insert a new Organization 
            }
            else
            {
                orgInfo.Id = Request.Form["hid_CurrentOrgid"];

                ////1 update the Organization leader selection for existing Organization 

                //GeneralUtilities ut = new GeneralUtilities();
                //Organization existingOrganization = new Organization(Request.Form["hid_CurrentOrganizationid"]);
                //List<string> newUserLeadlist = Request.Form["inputOrganizationLeader"].ToString().Split(',').ToList();


                //result = ut.ProcessLinkedUsers(existingOrganization.ManagerUserIds, newUserLeadlist, "1", orgInfo.Id, "1");


                ////2 update the Organization member selection for existing Organization  
                //List<string> newMemberlist = Request.Form["inputOrganizationMember"].ToString().Split(',').ToList();

                ////remove lead user from the member selection  
                //List<string> newMemberlist2 = new List<string>();
                //var differentMembers2 = newMemberlist.Where(p2 => newUserLeadlist.All(p => p2 != p)).ToList<string>();
                //differentMembers2.ForEach(u => newMemberlist2.Add(u));

                //result = ut.ProcessLinkedUsers(existingOrganization.MemberUserIds, newMemberlist2, "1", orgInfo.Id, "0");



                // update existing Organization

                //string imageUrl = SaveImageFile(ImgUpload, "images/new", 600, 600, "images");

                //SaveFile(Attachment);

                result = orgInfo.Save();

            }



            if (result == "ok" && !string.IsNullOrWhiteSpace(Request.Form["hid_CurrentOrganizationid"]))
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);

                Response.Redirect("OrganizationsEditLight?OrganizationId=" + Request.Form["hid_CurrentOrganizationid"] + "&Random=" + randomNumber.ToString());
                //Response.Redirect("#");
            }
            else if (result.Contains("failed") == false && string.IsNullOrWhiteSpace(Request.Form["hid_CurrentOrganizationid"]))
            {
                Response.Redirect("OrganizationSettings?Orgid=" + result);
            }
            else
            {
                errorMessage = result;
            }

        }

        //public IActionResult OnPostUploadFile(IFormFile postedFile, string OrgId)
        //{
        //}

        public void OnPost2(IFormFile logofile, string OrgId)
        { // https://www.aspsnippets.com/Articles/ASPNet-Core-Razor-Pages-Upload-Files-Save-Insert-file-to-Database-and-Download-Files.aspx

            string fileName = Path.GetFileName(logofile.FileName);
            string contentType = logofile.ContentType;
            using (MemoryStream ms = new MemoryStream())
            {
                logofile.CopyTo(ms);
                var builder = WebApplication.CreateBuilder();
                string constr = builder.Configuration.GetConnectionString("MyAffDBConnection");
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "Update Organization set Logo=@Logo where Id=@OrgId";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Logo", ms.ToArray());
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }

            // return RedirectToPage("Index");
        }

        //private bool IsImage(HttpPostedFile file)
        //{
        //    if (file != null && Regex.IsMatch(file.ContentType, "image/\\S+") &&
        //      file.ContentLength > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public string SaveImageFile(File fu, string directoryPath, int MaxWidth, int MaxHeight, string prefixName)
        //{
        //    string serverPath = "", returnString = "";
        //    if (fu.HasFile)
        //    {
        //        Byte[] bytes = fu.FileBytes;
        //        //Int64 len = 0;
        //        prefixName = "Testing" + prefixName;
        //        //directoryPath = "Testing/";
        //        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
        //        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
        //        string dipath = System.Web.HttpContext.Current.Server.MapPath("~/") + directoryPath;
        //        DirectoryInfo di = new DirectoryInfo(dipath);
        //        if (!(di.Exists))
        //        {
        //            di.Create();
        //        }
        //        HttpPostedFile file = fu.PostedFile;
        //        DateTime oldTime = new DateTime(1970, 01, 01, 00, 00, 00);
        //        DateTime currentTime = DateTime.Now;
        //        TimeSpan structTimespan = currentTime - oldTime;
        //        prefixName += ((long)structTimespan.TotalMilliseconds).ToString();
        //        if (IsImage(file))
        //        {
        //            using (Bitmap bitmap = new Bitmap(file.InputStream, false))
        //            {
        //                serverPath = dipath + "//" + prefixName + fu.FileName.Substring(fu.FileName.IndexOf("."));
        //                img.Save(serverPath);
        //                returnString = "~/" + directoryPath + "//" + prefixName + fu.FileName.Substring(fu.FileName.IndexOf("."));
        //            }
        //        }
        //    }
        //    return returnString;
        //}

        //public void SaveFile()
        //{
        //    foreach (var file in Request.Form.Files)
        //    {
        //        Image img = new Image();
        //        img.ImageTitle = file.FileName;

        //        MemoryStream ms = new MemoryStream();
        //        file.CopyTo(ms);
        //        img.ImageData = ms.ToArray();

        //        ms.Close();
        //        ms.Dispose();

        //        db.Images.Add(img);
        //        db.SaveChanges();
        //    } 
        //}

        // Save to file system
        //public FileUploadState SaveFile(HttpPostedFileBase Attachment)
        //{ 
        //    if (Attachment != null)
        //    {
        //        try //attempt to save file to file system
        //        {
        //            var fileName = Path.GetFileName(Attachment.FileName);
        //            //path as parameter can be changed to any desired valid path
        //            var path = Path.Combine((@"C:\Images"), fileName);
        //            Attachment.SaveAs(path);
        //            return FileUploadState.Uploaded;
        //        }
        //        catch //implement your own error handling here 
        //        {
        //            //error handling
        //            return FileUploadState.Failed;
        //        }
        //    }
        //    else
        //    {
        //        return FileUploadState.NoFileSelected;
        //    }
        //}

    }

}

