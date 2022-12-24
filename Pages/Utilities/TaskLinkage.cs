//using Outreach.Pages.Clients;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class TaskLinkage
    {
        public string Id;
        public string TaskId; 
        public string OrganizationId;
        public string TeamId;
        public string ProjectId;

        public TaskLinkage()
        {
            Id = "";
            TaskId = ""; 
            OrganizationId = "";
            TeamId = "";
            ProjectId = "";
        }
        public TaskLinkage(string TaskId)
        { // retrive TeamTask_User data by TeamTask_User ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (TaskId.Trim() != "")
                        sql = "select Id,TaskId,OrganizationId,TeamId,ProjectId from TaskLinkage with(nolock) where TaskId='" + TaskId + "' order by Id";
 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                TaskId = reader.GetInt32(1).ToString();
                                OrganizationId = reader.GetInt32(3).ToString();

                                if (reader["TeamId"].GetType() != typeof(DBNull))
                                {// task level user
                                    TeamId = reader["TeamId"].ToString();
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


        public string UpdateTaskLinkage()
        {
            string result = "ok";

            return result;

        }


        public string Save()
        {
            //save the new TaskLinkage into the database, One task can only link to one thing: Org, team or project

            string result = "ok";
            int newProdID = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO TaskLinkage " +
                                  "(TaskId,OrganizationId,TeamId,ProjectId) VALUES " +
                                  "(@TaskId,@OrganizationId,@TeamId,@ProjectId);" +
                                  "Select newID=TaskId FROM TaskLinkage where TaskId='" + this.TaskId + "'"; 

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", this.TaskId);
                        cmd.Parameters.AddWithValue("@OrganizationId", this.OrganizationId);
                        cmd.Parameters.AddWithValue("@TeamId", this.TaskId);
                        cmd.Parameters.AddWithValue("@ProjectId", this.TaskId); 

                        if (this.TeamId != "")
                        { // task levle user
                            cmd.Parameters.AddWithValue("@TeamId", this.TeamId);
                        }
                        else
                            cmd.Parameters.AddWithValue("@TeamId", DBNull.Value);
                         
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

         

        //public string Delete(string TeamTask_OrganizationId)
        //{ 
        //    //we should not call this method especially when status is reference by other Team or task

        //    string result = "ok";

        //    try
        //    {
        //        var builder = WebApplication.CreateBuilder();
        //        var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            String sql = "Delete TaskLinkage WHERE id=@id"; 
        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                command.Parameters.AddWithValue("@id", TeamTask_OrganizationId);

        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "failed" + ex.Message;
        //    }
        //    return result;
        //}
    }



}
