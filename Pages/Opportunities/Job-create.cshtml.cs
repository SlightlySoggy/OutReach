using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Clients;
using System.Data;
using System.Data.SqlClient;

namespace MyAffTest.Pages.Opportunities
{
    //[Authorize(Roles = "OrganizationContactor")]
    public class JobcreateModel : PageModel
    {
        public Organization orgInfo = new Organization();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            int user_id = 2;
            int org_id = 1;
            orgInfo = new Organization(org_id.ToString());

            //orgInfo = GetOrganizationInfoByOrg_id();
            if (orgInfo == null)
            {

            }



        }
        public void OnPost()
        {
            orgInfo.Name = Request.Form["opptitle"];
            //orgInfo.Id = Request.Form["Id"];
            //orgInfo.Name = Request.Form["Name"];
            //orgInfo.Description = Request.Form["address"];
            //orgInfo.PrimaryContactUserId = Request.Form["address"];
            //orgInfo.Address = Request.Form["address"];
            //orgInfo.CreatedDate = Request.Form["address"];
            //orgInfo.CreatedUserId = Request.Form["address"];
            //orgInfo.StatusId = Request.Form["address"];
            //orgInfo.Phone = Request.Form["address"];
            //orgInfo.Email = Request.Form["address"];
            //orgInfo.WebURL = Request.Form["address"];
            //if (orgInfo.name.Length == 0 || orgInfo.email.Length == 0 ||
            //    orgInfo.phone.Length == 0 || orgInfo.address.Length == 0)
            //{
            //    errorMessage = "All the fields are required";
            //    return;
            //}

            ////save the new organization into the database

            //try
            //{
            //    var builder = WebApplication.CreateBuilder();
            //    var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");
            //    //String connectionString = "Data Source=DESKTOP-9A8N31U\\SQLEXPRESS;Initial Catalog=OutReach;Persist Security Info=True;User ID=Maxwellhuodali;Password=M!1axwelliscool";

            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //    {
            //        connection.Open();
            //        string sql = "INSERT INTO clients " +
            //                     "(name, email, phone, address) VALUES " +
            //                     "(@name, @email, @phone, @address);";
            //        using (SqlCommand cmd = new SqlCommand(sql, connection))
            //        {
            //            cmd.Parameters.AddWithValue("@name", orgInfo.name);
            //            cmd.Parameters.AddWithValue("@email", orgInfo.email);
            //            cmd.Parameters.AddWithValue("@phone", orgInfo.phone);
            //            cmd.Parameters.AddWithValue("@address", orgInfo.address);

            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    errorMessage = ex.Message;
            //    return;
            //}
            //orgInfo.name = ""; orgInfo.email = ""; orgInfo.phone = ""; orgInfo.address = "";
            //successMessage = "New Client Added Correctly";
            //Response.Redirect("Index");
        }
    }

}
