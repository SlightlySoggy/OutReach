//using Outreach.Pages.Clients;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
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
        public string PrimaryContactUserId;// will be the user who create this org online
        public LoginUserInfo PrimaryContactUser; 
        public string CreatedDate;
        public string CreatedUserId;
        public string StatusId;
        public string Status;
        public string Phone;
        public string Email;        // general email
        public string WebURL;
        public UploadFile Logo;
        public List<UserLinkage> ManagerUserIds;
        public List<UserLinkage> MemberUserIds;
        public Organization()
        {
            Id = "";
            Name = "";
            Description = "";
            Address = "";
            PrimaryContactUserId = "";
            PrimaryContactUser = new LoginUserInfo();
            CreatedDate = "";
            CreatedUserId = "";
            StatusId = "";
            Status = "";
            Phone = "";
            Email = "";
            WebURL = "";
            Logo = new UploadFile();
            ManagerUserIds = new List<UserLinkage>();
            MemberUserIds = new List<UserLinkage>();

        }
        public Organization(string OrgId)
        { // retrive Organization data by Organization ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select o.Id,Name,Description,Address,PrimaryContactUserId,CreatedDate,CreatedUserId,StatusId,Phone,Email,WebURL,Status=st.StatusName " +
                        " ,LogoFileID = (Select top 1 ul.Id from UploadFile ul where ul.FileTypeId = 1 and ul.GroupTypeId = 1 and ul.LinkedGroupId = o.Id )" +
                        " from Organization o with(nolock) " +
                        " left join StandardStatus st on st.Id = o.StatusId where o.Id=" + OrgId;
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //ClientInfo clientInfo = new ClientInfo();
                                Id = reader.GetInt32(0).ToString(); 

                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    Name = reader["Name"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    Description = reader["Description"].ToString();
                                }

                                if (reader["Address"].GetType() != typeof(DBNull))
                                {
                                    Address = reader["Address"].ToString();
                                }

                                if (reader["PrimaryContactUserId"].GetType() != typeof(DBNull))
                                {
                                    PrimaryContactUserId = reader["PrimaryContactUserId"].ToString();

                                    PrimaryContactUser = new LoginUserInfo(PrimaryContactUserId);
                                }

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    CreatedDate = reader["CreatedDate"].ToString();
                                }

                                if (reader["CreatedUserId"].GetType() != typeof(DBNull))
                                {
                                    CreatedUserId = reader["CreatedUserId"].ToString();
                                }

                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    StatusId = reader["StatusId"].ToString();
                                }

                                if (reader["Status"].GetType() != typeof(DBNull))
                                {
                                    Status = reader["Status"].ToString();
                                }

                                if (reader["Phone"].GetType() != typeof(DBNull))
                                {
                                    Phone = reader["Phone"].ToString();
                                }

                                if (reader["Email"].GetType() != typeof(DBNull))
                                {
                                    Email = reader["Email"].ToString();
                                }

                                if (reader["WebURL"].GetType() != typeof(DBNull))
                                {
                                    WebURL = reader["WebURL"].ToString();
                                }

                                if (reader["LogoFileID"].GetType() != typeof(DBNull))
                                {
                                    string LogoFileID = reader["LogoFileID"].ToString();
                                    Logo = new UploadFile(LogoFileID);
                                }
                                else 
                                    Logo = new UploadFile();

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


        public string Save() // int Id, string Name, string Description, string EstimatedBudget, string ActualSpent, int OrganizationId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new Task into the database

            string result = "ok";
            int newTaskID = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";

                    if (this.Id == "" || this.Id == "0")
                    {
                        sql = "INSERT INTO [Organization] " +
                                      "(Name,Description,Address,PrimaryContactUserId,CreatedDate,CreatedUserId,StatusId,Phone,Email,WebURL ) VALUES " +
                                      "(@Name,@Description,@Address,@PrimaryContactUserId,@CreatedDate,@CreatedUserId,@StatusId,@Phone,@Email,@WebURL );" +
                                      "Select newID=MAX(id) FROM Organization";
                    }
                    else
                    {
                        sql = "Update [Organization] " +
                               "set Name = @Name," +
                                   "Description = @Description," +
                                   "Address = @Address," +
                                   "PrimaryContactUserId = @PrimaryContactUserId," +
                                   "CreatedDate = @CreatedDate," +
                                   "CreatedUserId = @CreatedUserId," +
                                   "StatusId = @StatusId," +
                                   "Phone = @Phone," +
                                   "Email = @Email," +
                                   "WebURL = @WebURL " + 
                                " where id = '" + this.Id + "' ; Select newID=" + this.Id + "";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", this.Name);
                        cmd.Parameters.AddWithValue("@Description", this.Description);
                        cmd.Parameters.AddWithValue("@Address", this.Address);
                        cmd.Parameters.AddWithValue("@PrimaryContactUserId", this.PrimaryContactUserId);
                        cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedUserId", this.CreatedUserId);
                        cmd.Parameters.AddWithValue("@StatusId", this.StatusId);
                        cmd.Parameters.AddWithValue("@Phone", this.Phone);
                        cmd.Parameters.AddWithValue("@Email", this.Email);
                        cmd.Parameters.AddWithValue("@WebURL", this.WebURL);
                       // cmd.ExecuteNonQuery();
                        newTaskID = (Int32)cmd.ExecuteScalar();

                        //if (this.Id == "" && newTaskID != 0)
                        //{
                        //    this.TaskLinkage.TaskId = newTaskID.ToString();
                        //    this.TaskLinkage.Save();
                        //}

                        result = newTaskID.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                result = "failed" + ex.Message;
            }
            return result;
        }


    }
}
