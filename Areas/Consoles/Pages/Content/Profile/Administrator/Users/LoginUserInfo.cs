
using System.Data;
using System.Data.SqlClient;


namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users
{
    public class LoginUserInfo
    {
        public String Id;
        public String User_Id;
        public String UserName;
        public String firstName;
        public String lastName; 
        public String PhoneNumber; 
        public String Email;
        public String Password;
        public String Created_at;
        public String IsSelected; // sepecial use when project or task select member 
        public LoginUserInfo()
        {
            Id = "";
            User_Id = "";
            UserName = "";
            firstName = "";
            lastName = "";
            PhoneNumber = "";
            Email = "";
            Password = "";
            Created_at = "";
            IsSelected = "";//  "selected"
        }


        public  LoginUserInfo(string User_Id = "")
        { // retrive login user by org ID in the future, now just list all

            List<LoginUserInfo> userlist = new List<LoginUserInfo>();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT Id,User_Id,UserName,Email,Password='',Created_at=convert(varchar,Created_at),firstName, lastName ,PhoneNumber FROM AspNetUsers  with(nolock) Where User_Id='" + User_Id + "' ";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            { 
                                if (reader["Id"].GetType() != typeof(DBNull))
                                {
                                    Id = reader["Id"].ToString();
                                }
                                if (reader["User_Id"].GetType() != typeof(DBNull))
                                {
                                    User_Id = reader["User_Id"].ToString();
                                }
                                if (reader["UserName"].GetType() != typeof(DBNull))
                                {
                                    UserName = reader["UserName"].ToString();
                                }

                                if (reader["Email"].GetType() != typeof(DBNull))
                                {
                                    Email = reader["Email"].ToString();
                                }
                                if (reader["Password"].GetType() != typeof(DBNull))
                                {
                                    Password = reader["Password"].ToString();
                                }
                                if (reader["Created_at"].GetType() != typeof(DBNull))
                                {
                                    Created_at = reader["Created_at"].ToString();
                                }

                                if (reader["firstName"].GetType() != typeof(DBNull))
                                {
                                    firstName = reader["firstName"].ToString();
                                }

                                if (reader["lastName"].GetType() != typeof(DBNull))
                                {
                                    lastName = reader["lastName"].ToString();
                                }

                                if (reader["PhoneNumber"].GetType() != typeof(DBNull))
                                {
                                    PhoneNumber = reader["PhoneNumber"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message.ToString());
            }
             
        }

    }
}
