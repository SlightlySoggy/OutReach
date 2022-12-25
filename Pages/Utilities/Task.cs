//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class Task
    {
        public string Id; 
        public string Name;
        public string Description; 
        public string CreatedDate;
        public string CreatedUserId;
        public List<UserLinkage> TaskManagerUserIds;
        public List<UserLinkage> TaskMemberUserIds;
        public string StartDate;
        public string DueDate;
        public string CompletionDate; 
        public string ProjectTaskStatusId;
        public string ProjectTaskStatus; 
        public TaskLinkage TaskLinkage;

        public Task()
        {
            Id = ""; 
            Name = "";
            Description = ""; 
            CreatedDate = "";
            CreatedUserId = "";
            TaskManagerUserIds = new List<UserLinkage>();
            TaskMemberUserIds  = new List<UserLinkage>();
            StartDate = "";
            DueDate = "";
            CompletionDate = "";
            ProjectTaskStatusId = "1";//default status is planned 1
            ProjectTaskStatus = ""; // the status name
            TaskLinkage = new TaskLinkage();

        }
        public Task(string taskId)
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
                    sql = "select t.Id,t.Name,t.Description,t.CreatedDate,t.CreatedUserId,t.StartDate,t.DueDate,t.CompletionDate,t.ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Task t with(nolock) left join ProjectTaskStatus S on S.Id=t.ProjectTaskStatusId where t.Id='" + taskId + "'";

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

                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    Name = reader["Name"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    Description = reader["Description"].ToString();
                                }

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
                                    StartDate = ut.EmptyDateConvert(reader["StartDate"].ToString());
                                }

                                if (reader["DueDate"].GetType() != typeof(DBNull))
                                {
                                    DueDate = ut.EmptyDateConvert(reader["DueDate"].ToString());
                                }

                                if (reader["CompletionDate"].GetType() != typeof(DBNull))
                                {
                                    CompletionDate = ut.EmptyDateConvert(reader["CompletionDate"].ToString());
                                }

                                if (reader["ProjectTaskStatusId"].GetType() != typeof(DBNull))
                                {
                                    ProjectTaskStatusId = reader["ProjectTaskStatusId"].ToString();
                                }

                                if (reader["ProjectTaskStatus"].GetType() != typeof(DBNull))
                                {
                                    ProjectTaskStatus = reader["ProjectTaskStatus"].ToString();
                                }

                                TaskManagerUserIds = ut.GetLinkedUserList("4", taskId, "true");
                                TaskMemberUserIds  = ut.GetLinkedUserList("4", taskId, "false");


                                TaskLinkage = new TaskLinkage(taskId);  

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

        public string Save() // int Id, string Name, string Description, string EstimatedBudget, string ActualSpent, int OrganizationId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
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
                                      "(Name,Description,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId) VALUES " +
                                      "(@Name,@Description,@CreatedDate,@CreatedUserId,@StartDate,@DueDate,@CompletionDate,@ProjectTaskStatusId);" +
                                      "Select newID=MAX(id) FROM Task"; 
                    }
                    else
                    {
                        sql = "Update Task " +
                               "set Name = @Name," +
                                   "Description = @Description," + 
                                   "CreatedDate = @CreatedDate," +
                                   "CreatedUserId = @CreatedUserId," + 
                                   "StartDate = @StartDate," +
                                   "DueDate = @DueDate," +
                                   "CompletionDate = @CompletionDate," +
                                   "ProjectTaskStatusId = @ProjectTaskStatusId " + 
                                " where id = '" + this.Id + "' ; Select newID=" + this.Id + "";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    { 
                        cmd.Parameters.AddWithValue("@Name", this.Name);
                        cmd.Parameters.AddWithValue("@Description", this.Description); 
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

         

        public string Delete(string TaskId) // int Id, string Name, string Description, string EstimatedBudget, string ActualSpent, int OrganizationId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
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
