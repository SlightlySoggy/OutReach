using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAffTest.Pages.Clients;
using System.Data.SqlClient;

namespace MyAffTest.Pages.Users
{
    [Authorize]
    public class UsersModel : PageModel
    {
        private readonly ILogger<UsersModel> _logger;

        public List<LoginUserInfo> ListUsers = new List<LoginUserInfo>();

        public UsersModel(ILogger<UsersModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
            {
                try
                {
                    var builder = WebApplication.CreateBuilder();
                    var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");
                    //String connectionString = "Data Source=DESKTOP-9A8N31U\\SQLEXPRESS;Initial Catalog=OutReach;Persist Security Info=True;User ID=Maxwellhuodali;Password=M!1axwelliscool";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        String sql = "SELECT User_Id,UserName,Email,Password='',Created_at=convert(varchar,Created_at)  FROM AspNetUsers  with(nolock) ";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    LoginUserInfo userinfo = new LoginUserInfo();
                                    userinfo.User_Id = reader.GetInt32(0).ToString();
                                    userinfo.UserName = reader.GetString(1);
                                    userinfo.Email = reader.GetString(2);
                                    userinfo.Password = reader.GetString(3);
                                    userinfo.Created_at = reader.GetString(4);

                                ListUsers.Add(userinfo);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                }

            }
        }


    }
