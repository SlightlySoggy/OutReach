//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;


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
        public string ProjectManagerUserId;
        public string StartDate;
        public string DueDate;
        public string CompletionDate; 
        public string DurationByDay;
        public string ProjectTaskStatusId; 

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
            ProjectManagerUserId = "";
            StartDate = "";
            DueDate = "";
            CompletionDate = ""; 
            DurationByDay = "";
            ProjectTaskStatusId = "";

        }
        public Project(string projectId)
        { // retrive Project data by Project ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
//                    if (projectId.Trim() != "")
                    sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) where Id='" + projectId + "'";

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

                                CreatedOrgId = reader.GetInt32(5).ToString();
                                CreatedDate = reader.GetDateTime(6).ToString();
                                CreatedUserId = reader.GetInt32(7).ToString();
                                ProjectManagerUserId = reader.GetInt32(8).ToString();
                                StartDate = reader.GetDateTime(9).ToString();
                                DueDate = reader.GetDateTime(10).ToString();
                                CompletionDate = reader.GetDateTime(11).ToString();
                                ProjectTaskStatusId = reader.GetInt32(12).ToString();

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

        public List<Project> GetProjectListByStatusId(string statusId = "")
        { // retrive Project data by Project ID

            List<Project> listPro = new List<Project>();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (statusId.Trim() != "") 
                        sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) where ProjectTaskStatusId = " + statusId + " order by ID Desc";
                    else 
                        sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) order by ID Desc";
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

                                CreatedOrgId = reader.GetInt32(5).ToString();
                                CreatedDate = reader.GetDateTime(6).ToString();
                                CreatedUserId = reader.GetInt32(7).ToString();
                                ProjectManagerUserId = reader.GetInt32(8).ToString();
                                StartDate = reader.GetDateTime(9).ToString();
                                DueDate = reader.GetDateTime(10).ToString();
                                CompletionDate = reader.GetDateTime(11).ToString();
                                ProjectTaskStatusId = reader.GetInt32(12).ToString();

                                listPro.Add(this);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message.ToString());
            }

            return listPro;
        }

        public string Save(string ProjectId) // int Id, string ProjectName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjectTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
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

                    if (ProjectId=="" || ProjectId == "0")
                    {
                        sql = "INSERT INTO Project " +
                                      "(ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay) VALUES " +
                                      "(@ProjectName,@Description,@EstimatedBudget,@ActualSpent,@CreatedOrgId,@CreatedDate,@CreatedUserId,@StartDate,@DueDate,@CompletionDate,@ProjectTaskStatusId,@DurationByDay);" +
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
                                   "ProjectManagerUserId = @ProjectManagerUserId," +
                                   "StartDate = @StartDate," +
                                   "DueDate = @DueDate," +
                                   "CompletionDate = @CompletionDate," + 
                                   "ProjectTaskStatusId = @ProjectTaskStatusId," +
                                   "DurationByDay = @DurationByDay " +
                                "where id = '" + ProjectId + "'";

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
                        cmd.Parameters.AddWithValue("@ProjectManagerUserId", this.ProjectManagerUserId);
                        cmd.Parameters.AddWithValue("@StartDate", this.StartDate);
                        cmd.Parameters.AddWithValue("@DueDate", this.DueDate);
                        cmd.Parameters.AddWithValue("@CompletionDate", this.CompletionDate); 
                        cmd.Parameters.AddWithValue("@ProjectTaskStatusId", this.ProjectTaskStatusId); 
                        cmd.Parameters.AddWithValue("@DurationByDay", this.DurationByDay);
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



        //public string SaveTags(string ProjectId, string tagId, string action)
        //{ 

        //    string result = "ok";
        //    int newProdID = 0;
        //    try
        //    {
        //        var builder = WebApplication.CreateBuilder();
        //        var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            string sql = "";

        //            if (ProjectId != "" && tagId != "" && action.ToLower() == "insert")
        //            {
        //                sql = "INSERT INTO OpportunityTag " +
        //                              "(OpportunityId,TagId) VALUES " +
        //                              "(@OpportunityId,@TagId);";
        //            }
        //            else if (ProjectId != "" && tagId != "" && action.ToLower() == "delete")
        //            {
        //                sql = "Delete INTO OpportunityTag " +
        //                              "(OpportunityId,TagId) VALUES " +
        //                              "(@OpportunityId,@TagId);";
        //            } 

        //            using (SqlCommand cmd = new SqlCommand(sql, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@OpportunityId", ProjectId);
        //                cmd.Parameters.AddWithValue("@TagId", tagId);  
        //                cmd.ExecuteNonQuery(); 
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "failed" + ex.Message;
        //    }
        //    return result;
        //}

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
