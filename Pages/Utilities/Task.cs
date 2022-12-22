//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;


namespace Outreach.Pages.Utilities
{
    public class Task
    {
        public string Id;
        public string ProjectId;
        public string CreatedOrgId;
        public string Name;
        public string Description;
        public string EstimatedBudget;
        public string ActualSpent;
        public string CreatedDate;
        public string CreatedUserId;
        public List<ProjectTaskUser> TaskManagerUserIds;
        public List<ProjectTaskUser> TaskMemberUserIds;
        public string StartDate;
        public string DueDate;
        public string CompletionDate; 
        public string ProjectTaskStatusId;
        public string ProjectTaskStatus;

        public Task()
        {
            Id = "";
            ProjectId = "";
            CreatedOrgId = "";
            Name = "";
            Description = "";
            EstimatedBudget = "";
            ActualSpent = "";
            CreatedDate = "";
            CreatedUserId = "";
            TaskManagerUserIds = new List<ProjectTaskUser>();
            TaskMemberUserIds  = new List<ProjectTaskUser>();
            StartDate = "";
            DueDate = "";
            CompletionDate = "";
            ProjectTaskStatusId = "1";//default status is planned 1
            ProjectTaskStatus = ""; // the status name

        }
        public Task(string TaskId)
        { // retrive Task data by Task ID
            try
            {
                GeneralUtilities ut = new GeneralUtilities();
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
//                    if (TaskId.Trim() != "")
                    sql = "select P.Id,ProjectId,Name,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Task p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId where P.Id='" + TaskId + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                if (reader["Id"].GetType() != typeof(DBNull))
                                {
                                    Id = reader["Id"].ToString();
                                }

                                if (reader["ProjectId"].GetType() != typeof(DBNull))
                                {
                                    ProjectId = reader["ProjectId"].ToString();
                                } 

                                if (reader["CreatedOrgId"].GetType() != typeof(DBNull))
                                {
                                    CreatedOrgId = reader["CreatedOrgId"].ToString();
                                }

                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    Name = reader["Name"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    Description = reader["Description"].ToString();
                                }

                                if (reader["EstimatedBudget"].GetType() != typeof(DBNull))
                                {
                                    EstimatedBudget = reader["EstimatedBudget"].ToString();
                                }

                                if (reader["ActualSpent"].GetType() != typeof(DBNull))
                                {
                                    ActualSpent = reader["ActualSpent"].ToString();
                                }
                                ////////////////////////////////////////////

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    CreatedDate = reader["CreatedDate"].ToString();
                                }

                                if (reader["CreatedUserId"].GetType() != typeof(DBNull))
                                {
                                    CreatedUserId = reader["CreatedUserId"].ToString();
                                }

                                if (reader["StartDate"].GetType() != typeof(DBNull))
                                {
                                    StartDate = reader["StartDate"].ToString();
                                }

                                if (reader["DueDate"].GetType() != typeof(DBNull))
                                {
                                    DueDate = reader["DueDate"].ToString();
                                }

                                if (reader["CompletionDate"].GetType() != typeof(DBNull))
                                {
                                    CompletionDate = reader["CompletionDate"].ToString();
                                }

                                if (reader["ProjectTaskStatusId"].GetType() != typeof(DBNull))
                                {
                                    ProjectTaskStatusId = reader["ProjectTaskStatusId"].ToString();
                                }

                                if (reader["ProjectTaskStatus"].GetType() != typeof(DBNull))
                                {
                                    ProjectTaskStatus = reader["ProjectTaskStatus"].ToString();
                                } 

                                TaskManagerUserIds = ut.GetProjectorTaskUserList("", TaskId, "true");
                                TaskMemberUserIds  = ut.GetProjectorTaskUserList("", TaskId, "false");
                                // listOrgs.Add(Org);
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

        public string Save() // int Id, string Name, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new Task into the database

            string result = "ok";
            int newProdID = 0;
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
                        sql = "INSERT INTO Task " +
                                      "(ProjectId,Name,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId) VALUES " +
                                      "(@ProjectId,@Name,@Description,@EstimatedBudget,@ActualSpent,@CreatedOrgId,@CreatedDate,@CreatedUserId,@StartDate,@DueDate,@CompletionDate,@ProjectTaskStatusId);" +
                                      "Select newID=MAX(id) FROM Task"; 
                    }
                    else
                    {
                        sql = "Update Task " +
                               "set ProjectId = @ProjectId, Name = @Name," +
                                   "Description = @Description," +
                                   "EstimatedBudget = @EstimatedBudget," +
                                   "ActualSpent = @ActualSpent," +
                                   "CreatedOrgId = @CreatedOrgId," +
                                   "CreatedDate = @CreatedDate," +
                                   "CreatedUserId = @CreatedUserId," +
                                   //"TaskManagerUserId = @TaskManagerUserId," +
                                   "StartDate = @StartDate," +
                                   "DueDate = @DueDate," +
                                   "CompletionDate = @CompletionDate," +
                                   "ProjectTaskStatusId = @ProjectTaskStatusId " + 
                                " where id = '" + this.Id + "' ; Select newID=" + this.Id + "";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProjectId", this.ProjectId);
                        cmd.Parameters.AddWithValue("@Name", this.Name);
                        cmd.Parameters.AddWithValue("@Description", this.Description);
                        cmd.Parameters.AddWithValue("@EstimatedBudget", this.EstimatedBudget);
                        cmd.Parameters.AddWithValue("@ActualSpent", this.ActualSpent);
                        cmd.Parameters.AddWithValue("@CreatedOrgId", this.CreatedOrgId);
                        cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedUserId", this.CreatedUserId); 
                        cmd.Parameters.AddWithValue("@StartDate", this.StartDate);
                        cmd.Parameters.AddWithValue("@DueDate", this.DueDate);
                        cmd.Parameters.AddWithValue("@CompletionDate", this.CompletionDate); 
                        cmd.Parameters.AddWithValue("@ProjectTaskStatusId", this.ProjectTaskStatusId);  
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

         

        public string Delete(string TaskId) // int Id, string Name, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new Task into the database

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Update Task Set ProjectTaskStatusId=6 WHERE id=@id"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", TaskId);

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
