using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Utilities;
using System.Data.SqlClient;

namespace MyAffTest.Pages.Clients
{
    public class EditModel : PageModel
    {
        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            String id = Request.Query["id"];
            ClientInfo client = new ClientInfo();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");
                //String connectionString = "Data Source=DESKTOP-9A8N31U\\SQLEXPRESS;Initial Catalog=OutReach;Persist Security Info=True;User ID=Maxwellhuodali;Password=M!1axwelliscool";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM clients WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientInfo.id = "" + reader.GetInt32(0);
                                clientInfo.name = reader.GetString(1);
                                clientInfo.email = reader.GetString(2);
                                clientInfo.phone = reader.GetString(3);
                                clientInfo.address = reader.GetString(4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            clientInfo.id = Request.Form["id"];
            clientInfo.name = Request.Form["name"];
            clientInfo.email = Request.Form["email"];
            clientInfo.phone = Request.Form["phone"];
            clientInfo.address = Request.Form["address"];

            if (clientInfo.id.Length == 0 || clientInfo.name.Length == 0 ||
                clientInfo.email.Length == 0 || clientInfo.phone.Length == 0 ||
                clientInfo.address.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");
                //String connectionString = "Data Source=DESKTOP-9A8N31U\\SQLEXPRESS;Initial Catalog=OutReach;Persist Security Info=True;User ID=Maxwellhuodali;Password=M!1axwelliscool";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE clients " + "SET name=@name, email=@email, phone=@phone, address=@address " + " WHERE id=@id";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", clientInfo.name);
                        cmd.Parameters.AddWithValue("@email", clientInfo.email);
                        cmd.Parameters.AddWithValue("@phone", clientInfo.phone);
                        cmd.Parameters.AddWithValue("@address", clientInfo.address);
                        cmd.Parameters.AddWithValue("@id", clientInfo.id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            
            Response.Redirect("Index"); 
        }

    }
}
