//using Outreach.Pages.Clients;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class UserLinkage
    {
        public string Id;
        public string UserId;
        public string GroupTypeId;  //1:Organization,2:Team,3:Project,4:Task
        public string LinkedGroupId; //value can be: OrganizationId, TeamId, ProjectId, TaskId
        public string IsLead;        //"true" lead, "false" regular member
        public string OrganizationId;
        public string OrganizationName;
        public string TeamId;
        public string TeamName;
        public string ProjectId;
        public string ProjectName;
        public string LinkTo;

        public UserLinkage()
        { 
            Id = "";
            UserId = "";
            GroupTypeId = "";
            LinkedGroupId = "";
            IsLead = ""; 
            OrganizationId = "";
            OrganizationName = "";
            TeamId = "";
            TeamName = "";
            ProjectId = "";
            ProjectName = "";
            LinkTo = "";
        }
        public UserLinkage(string id)
        { // retrive TeamTask_User data by TeamTask_User ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (Id.Trim() != "")

                        sql = "select ul.Id,ul.UserId,ul.GroupTypeId,ul.LinkedGroupId,ul.IsLead,OrganizationId=o.Id,OrganizationName=o.name,ProjectId=p.Id,p.ProjectName,TeamId=t.ID,TeamName=t.name ,TaskId=tk.ID,TaskName=tk.name " +
                            " from UserLinkage ul with(nolock) " +
                            " left join Organization o on o.Id = ul.LinkedGroupId and ul.GroupTypeId =1 " +
                            " left join Project p on p.Id = ul.LinkedGroupId  and ul.GroupTypeId =2 " +
                            " left join Team t on t.Id = ul.LinkedGroupId and ul.GroupTypeId =3 " +
                            " left join Task tk on tk.Id = ul.LinkedGroupId and ul.GroupTypeId =4 " +
                            " where Id='" + id + "'  ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            { 
                                Id = Id;

                                if (reader["OrganizationId"].GetType() != typeof(DBNull))
                                {// Organization level task
                                    OrganizationId = reader["OrganizationId"].ToString();
                                    if (reader["OrganizationName"].GetType() != typeof(DBNull))
                                    {
                                        OrganizationName = reader["OrganizationName"].ToString();
                                        LinkTo = "Organization: " + OrganizationName;
                                    }
                                }
                                else if (reader["TeamId"].GetType() != typeof(DBNull))
                                {// Team  level task
                                    TeamId = reader["TeamId"].ToString();
                                    if (reader["TeamName"].GetType() != typeof(DBNull))
                                    {
                                        TeamName = reader["TeamName"].ToString();
                                        LinkTo = "Team: " + TeamName;
                                    }
                                }
                                else if (reader["ProjectId"].GetType() != typeof(DBNull))
                                {// Project level task
                                    ProjectId = reader["ProjectId"].ToString();
                                    if (reader["ProjectName"].GetType() != typeof(DBNull))
                                    {
                                        ProjectName = reader["ProjectName"].ToString();
                                        LinkTo = "Project: " + ProjectName;
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


        public string UpdateUserLinkage()
        {
            string result = "ok";

            return result;

        }


        public string Save()
        {
            //save the new UserLinkage into the database, One task can only link to one thing: Org, team or project

            string result = "failed";
            int newProdID = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //string sql = " INSERT INTO UserLinkage " +
                    //              " (UserId,GroupTypeId,LinkedGroupId,IsLead) VALUES " +
                    //              " (@UserId,@GroupTypeId,@LinkedGroupId,@IsLead) " +
                    string sql = " INSERT INTO UserLinkage " +
                                  " Select UserId=" + this.UserId + ", GroupTypeId=" + this.GroupTypeId + " , LinkedGroupId=" + this.LinkedGroupId + " , IsLead=" + this.IsLead + " " + 
                                  //" Where not exists (Select 1 from UserLinkage u where u.UserId=@UserId and u.GroupTypeId=@GroupTypeId and u.LinkedGroupId=@LinkedGroupId) " + // require to declare @UserId again 
                                  " Where not exists (Select 1 from UserLinkage u where u.UserId='" + this.UserId + "' and u.GroupTypeId='" + this.GroupTypeId + "' and u.LinkedGroupId='" + this.LinkedGroupId + "' ) " +
                                  " Select top 1 Id from UserLinkage u2 where u2.UserId='" + this.UserId + "' and u2.GroupTypeId='" + this.GroupTypeId + "' and u2.LinkedGroupId='" + this.LinkedGroupId + "' and u2.IsLead = '" + this.IsLead + "' ";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    { 
                        //cmd.Parameters.AddWithValue("@UserId", this.UserId);
                        //cmd.Parameters.AddWithValue("@GroupTypeId", this.GroupTypeId);
                        //cmd.Parameters.AddWithValue("@LinkedGroupId", this.LinkedGroupId);
                        //cmd.Parameters.AddWithValue("@IsLead", this.IsLead);
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //cmd.ExecuteNonQuery();
                                    //newProdID = (Int32)cmd.ExecuteScalar(); may threw exception if nothing to return

                                    newProdID = reader.GetInt32(0);
                                    result = " ok";
                                }
                            }
                        }
                         
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

                    String sql = "Delete UserLinkage WHERE id=@id";
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


 