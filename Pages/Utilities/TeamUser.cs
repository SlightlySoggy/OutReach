//using Outreach.Pages.Clients;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class TeamUser
    {
        public string Id;
        public string TeamId; 
        public string UserId;
        public string IsLead;

        public TeamUser()
        {
            Id = "";
            TeamId = ""; 
            UserId = "";
            IsLead = "";
        }
        public TeamUser(string TeamUserId)
        { // retrive TeamTask_User data by TeamTask_User ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (TeamUserId.Trim() != "")
                        sql = "select Id,TeamId,UserId,IsLead from TeamUser with(nolock) where Id='" + TeamUserId + "' order by Id";
 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                TeamId = reader.GetInt32(1).ToString();
                                UserId = reader.GetInt32(3).ToString();

                                if (reader["IsLead"].GetType() != typeof(DBNull))
                                {// task level user
                                    IsLead = reader["IsLead"].ToString();
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


        public string Save() // int Id, string TeamTask_UserName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int TeamTask_UserTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new TeamTask_User into the database 

            string result = "ok";
            int newProdID = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO TeamUser " +
                                  "(TeamId,  UserId,IsLead) VALUES " +
                                  "(@TeamId,  @UserId,@IsLead);" +
                                  "Select newID=MAX(id) FROM TeamUser"; 

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", this.UserId); 
                        cmd.Parameters.AddWithValue("@TeamId", this.TeamId);

                        if (this.IsLead != "")
                        { // task levle user
                            cmd.Parameters.AddWithValue("@IsLead", this.IsLead);
                        }
                        else
                            cmd.Parameters.AddWithValue("@IsLead", DBNull.Value);
                         
                        //cmd.ExecuteNonQuery();
                        newProdID = (Int32)cmd.ExecuteScalar();
                         
                    }
                }
            }
            catch (Exception ex)
            {
                result = "failed" + ex.Message;
            }
            return result;
        }

         

        public string Delete(string TeamTask_UserId)
        { 
            //we should not call this method especially when status is reference by other Team or task

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Delete TeamUser WHERE id=@id"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", TeamTask_UserId);

                        command.ExecuteNonQuery();
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
