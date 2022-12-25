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
        public string TaskId;
        public string OrganizationId;
        public string OrganizationName;
        public string TeamId;
        public string TeamName;
        public string ProjectId;
        public string ProjectName;
        public string BelongTo;

        public TaskLinkage()
        { 
            TaskId = "";
            OrganizationId = "";
            OrganizationName = "";
            TeamId = "";
            TeamName = "";
            ProjectId = "";
            ProjectName = "";
            BelongTo = "";
        }
        public TaskLinkage(string taskId)
        { // retrive TeamTask_User data by TeamTask_User ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (taskId.Trim() != "")
                        sql = "select tl.TaskId,tl.OrganizationId,tl.TeamId,tl.ProjectId,OrganizationName=o.name, p.ProjectName,TeamName=t.name from TaskLinkage tl with(nolock) " +
                            " left join Organization o on o.Id = tl.OrganizationId " +
                            " left join Team t on t.Id = tl.TeamId " +
                            " left join Project p on p.Id = tl.ProjectId " +
                            " where TaskId='" + taskId + "'  ";
 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                TaskId = taskId;

                                if (reader["OrganizationId"].GetType() != typeof(DBNull) && reader["OrganizationId"].ToString() !="0")
                                {// Organization level task
                                    OrganizationId = reader["OrganizationId"].ToString();
                                    if (reader["OrganizationName"].GetType() != typeof(DBNull))
                                    {
                                        OrganizationName = reader["OrganizationName"].ToString();
                                        BelongTo = "Organization (" + OrganizationName + ")";
                                    }
                                }
                                else if (reader["TeamId"].GetType() != typeof(DBNull) && reader["TeamId"].ToString() != "0")
                                {// Team  level task
                                    TeamId = reader["TeamId"].ToString();
                                    if (reader["TeamName"].GetType() != typeof(DBNull))
                                    {
                                        TeamName = reader["TeamName"].ToString();
                                        BelongTo = "Team (" + TeamName + ")";
                                    }
                                }
                                else if (reader["ProjectId"].GetType() != typeof(DBNull) && reader["ProjectId"].ToString() != "0")
                                {// Project level task
                                    ProjectId = reader["ProjectId"].ToString();
                                    if (reader["ProjectName"].GetType() != typeof(DBNull))
                                    {
                                        ProjectName = reader["ProjectName"].ToString();
                                        BelongTo = "Project (" + ProjectName + ")";
                                    }
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
                        cmd.Parameters.AddWithValue("@TeamId", this.TeamId);
                        cmd.Parameters.AddWithValue("@ProjectId", this.ProjectId);  

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
