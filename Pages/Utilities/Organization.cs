//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;


namespace Outreach.Pages.Utilities
{
    public class Organization
    {
        public string Id;
        public string Name;
        public string Description;
        public string Address;
        public string PrimaryContactUserId;
        public string CreatedDate;
        public string CreatedUserId;
        public string StatusId;
        public string Phone;
        public string Email;
        public string WebURL;
        public Organization()
        {
            Id = "";
            Name = "";
            Description = "";
            Address = "";
            PrimaryContactUserId = "";
            CreatedDate = "";
            CreatedUserId = "";
            StatusId = "";
            Phone = "";
            Email = "";
            WebURL = "";

        }
        public Organization(string Org_id)
        { // retrive Organization data by Organization ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select Id,Name,Description,Address,PrimaryContactUserId,CreatedDate,CreatedUserId,StatusId,PhoneNum,Email WebURL from Organization with(nolock) where Id=" + Org_id;
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //ClientInfo clientInfo = new ClientInfo();
                                Id = reader.GetInt32(0).ToString();
                                Name = reader.GetString(1);
                                Description = reader.GetString(2);
                                Address = reader.GetString(3);
                                PrimaryContactUserId = reader.GetInt32(4).ToString();
                                CreatedDate = reader.GetDateTime(5).ToString();
                                CreatedUserId = reader.GetInt32(6).ToString();
                                StatusId = reader.GetInt32(7).ToString();
                                Phone = reader.GetString(8);
                                Email = reader.GetString(9);
                                WebURL = reader.GetString(10);

                                // listOrgs.Add(Org);
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
        //public GetOrgInfobyContactUserId(user_id)
        //{

        //} 
    }
}
