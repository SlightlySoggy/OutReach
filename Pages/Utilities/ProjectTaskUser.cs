//using Outreach.Pages.Clients;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class ProjectTaskUser
    {
        public string Id;
        public string ProjectId;
        public string TaskId; // if it is null then it is project level user, otherwise it is task level user
        public string UserId;
        public string IsLead;

        public ProjectTaskUser()
        {
            Id = "";
            ProjectId = "";
            TaskId = "";
            UserId = "";
            IsLead = "";
        }
        public ProjectTaskUser(string projectTaskUserId)
        { // retrive ProjectTaskUser data by ProjectTaskUser ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (projectTaskUserId.Trim() != "")
                        sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where Id='" + projectTaskUserId + "' order by Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                ProjectId = reader.GetInt32(1).ToString();
                                UserId = reader.GetInt32(3).ToString();

                                if (reader["TaskId"].GetType() != typeof(DBNull))
                                {// task level user
                                    TaskId = reader["TaskId"].ToString();
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


        public string Save() // int Id, string ProjectTaskUserName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjectTaskUserTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new ProjectTaskUser into the database 

            string result = "ok";
            int newProdID = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO ProjectTaskUser " +
                                  "(ProjectId, TaskId, UserId,IsLead) VALUES " +
                                  "(@ProjectId, @TaskId, @UserId,@IsLead);" +
                                  "Select newID=MAX(id) FROM ProjectTaskUser";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", this.UserId);

                        if (this.TaskId != "")
                        { // task levle user, must set Projectid with null
                            cmd.Parameters.AddWithValue("@ProjectId", DBNull.Value);
                            cmd.Parameters.AddWithValue("@TaskId", this.TaskId);
                        }
                        else
                        { // Project levle user, must set taskid with null
                            cmd.Parameters.AddWithValue("@ProjectId", this.ProjectId);
                            cmd.Parameters.AddWithValue("@TaskId", DBNull.Value);
                        }

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



        public string Delete()
        {
            //we should not call this method especially when status is reference by other project or task

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Delete ProjectTaskUser WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", this.Id);

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
