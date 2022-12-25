//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;


namespace Outreach.Pages.Utilities
{
    public class Project
    {
        public string Id;
        public string ProjectName;
        public string Description;
        public string EstimatedBudget;
        public string ActualSpent;
        public string CreatedOrgId;
        public string CreatedDate;
        public string CreatedUserId;
        public List<UserLinkage> ProjectManagerUserIds;
        public List<UserLinkage> ProjectMemberUserIds;
        public string StartDate;
        public string DueDate;
        public string CompletionDate; 
        public string Estimated_DurationByDay;
        public string ProjectTaskStatusId;
        public string ProjectTaskStatus;

        public Project()
        {
            Id = "";
            ProjectName = "";
            Description = "";
            EstimatedBudget = "";
            ActualSpent = "";
            CreatedOrgId = "";
            CreatedDate = "";
            CreatedUserId = "";
            ProjectManagerUserIds = new List<UserLinkage>();
            ProjectMemberUserIds  = new List<UserLinkage>();
            StartDate = "";
            DueDate = "";
            CompletionDate = "";
            Estimated_DurationByDay = "";
            ProjectTaskStatusId = "1";//default status is planned 1
            ProjectTaskStatus = ""; // the status name

        }
        public Project(string projectId)
        { // retrive Project data by Project ID
            try
            {
                GeneralUtilities ut = new GeneralUtilities();
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
//                    if (projectId.Trim() != "")
                    sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId where P.Id='" + projectId + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                if (reader["ProjectName"].GetType() != typeof(DBNull))
                                {
                                    ProjectName = reader["ProjectName"].ToString();
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

                                if (reader["CreatedOrgId"].GetType() != typeof(DBNull))
                                {
                                    CreatedOrgId = reader["CreatedOrgId"].ToString();
                                }

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    CreatedDate = ut.EmptyDateConvert(reader["CreatedDate"].ToString());
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
                                 
                                ProjectManagerUserIds = ut.GetLinkedUserList("3", projectId, "true");
                                ProjectMemberUserIds = ut.GetLinkedUserList("3", projectId, "false");
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

        public string Save() // int Id, string ProjectName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new Project into the database

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
                        sql = "INSERT INTO Project " +
                                      "(ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId) VALUES " +
                                      "(@ProjectName,@Description,@EstimatedBudget,@ActualSpent,@CreatedOrgId,@CreatedDate,@CreatedUserId,@StartDate,@DueDate,@CompletionDate,@ProjectTaskStatusId);" +
                                      "Select newID=MAX(id) FROM Project"; 
                    }
                    else
                    {
                        sql = "Update Project " +
                               "set ProjectName = @ProjectName," +
                                   "Description = @Description," +
                                   "EstimatedBudget = @EstimatedBudget," +
                                   "ActualSpent = @ActualSpent," +
                                   "CreatedOrgId = @CreatedOrgId," +
                                   "CreatedDate = @CreatedDate," +
                                   "CreatedUserId = @CreatedUserId," +
                                   //"ProjectManagerUserId = @ProjectManagerUserId," +
                                   "StartDate = @StartDate," +
                                   "DueDate = @DueDate," +
                                   "CompletionDate = @CompletionDate," +
                                   "ProjectTaskStatusId = @ProjectTaskStatusId " + 
                                " where id = '" + this.Id + "' ; Select newID=" + this.Id + "";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProjectName", this.ProjectName);
                        cmd.Parameters.AddWithValue("@Description", this.Description);
                        cmd.Parameters.AddWithValue("@EstimatedBudget", this.EstimatedBudget);
                        cmd.Parameters.AddWithValue("@ActualSpent", this.ActualSpent);
                        cmd.Parameters.AddWithValue("@CreatedOrgId", this.CreatedOrgId);
                        cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedUserId", this.CreatedUserId);
                        //cmd.Parameters.AddWithValue("@ProjectManagerUserId", this.ProjectManagerUserId);
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

         

        public string Delete(string ProjectId) // int Id, string ProjectName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new Project into the database

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Update Project Set ProjectTaskStatusId=6 WHERE id=@id"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", ProjectId);

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
