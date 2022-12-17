//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;


namespace Outreach.Pages.Utilities
{
    public class Opportunity
    {
        public string Id;
        public string OpportunityTitle;
        public string Description;
        public string Responsibility;
        public string Requirement;
        public string CreatedOrgId;
        public string CreatedDate;
        public string CreatedUserId;
        public string StartDate;
        public string EndDate;
        public string Schedule;
        public string StatusId;
        public List<Tag> Tags;
        public string SiteAddress;

        public Opportunity()
        {
            Id = "";
            OpportunityTitle = "";
            Description = "";
            Responsibility = "";
            CreatedOrgId = "";
            CreatedDate = "";
            CreatedUserId = "";
            StatusId = "";
            Requirement = "";
            StartDate = "";
            EndDate = "";
            Schedule = "";
            StatusId = "";
            Tags = new List<Tag>();
            SiteAddress = "";

        }
        public Opportunity(string Opp_Id)
        { // retrive Opportunity data by Opportunity ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (Opp_Id.Trim() != "")
                        sql = "select Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,SiteAddress from Opportunity with(nolock) where Id=" + Opp_Id;
                    else
                        sql = "select Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,SiteAddress from Opportunity with(nolock) where  isnull(StatusId,1) = 1  and Id=" + Opp_Id;

                   using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                if (reader["OpportunityTitle"].GetType() != typeof(DBNull))
                                {
                                    OpportunityTitle = reader["OpportunityTitle"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    Description = reader["Description"].ToString();
                                }

                                if (reader["Responsibility"].GetType() != typeof(DBNull))
                                {
                                    Responsibility = reader["Responsibility"].ToString();
                                }

                                if (reader["Requirement"].GetType() != typeof(DBNull))
                                {
                                    Requirement = reader["Requirement"].ToString();
                                }
                                CreatedOrgId = reader.GetInt32(5).ToString();
                                CreatedDate = reader.GetDateTime(6).ToString();
                                CreatedUserId = reader.GetInt32(7).ToString(); 
                                StartDate = reader.GetDateTime(9).ToString();
                                EndDate = reader.GetDateTime(10).ToString();
                                //Schedule = (reader.GetString(11) == DBNull) ?"" : reader.GetString(11);
                                if (reader["Schedule"].GetType() != typeof(DBNull))
                                {
                                    Schedule = reader["Schedule"].ToString();
                                }

                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    StatusId = reader["StatusId"].ToString();
                                }
                                 
                                Tag tag = new Tag();
                                Tags = tag.GetReferencedTagsbyOpptunityId(Opp_Id); 

                                if (reader["SiteAddress"].GetType() != typeof(DBNull))
                                {
                                    SiteAddress = reader["SiteAddress"].ToString();
                                }

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

        public string Save(string oppId) // int Id, string OpportunityTitle, string Description, string Responsibility, string Requirement, int CreatedOrgId, string CreatedDate, int CreatedUserId, int StatusId, string StartDate, string EndDate, string Tags)
        {
            //save the new Opportunity into the database

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

                    if (oppId=="" || oppId == "0")
                    {
                        sql = "INSERT INTO Opportunity " +
                                      "(OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,EndDate,Schedule,StatusId,SiteAddress) VALUES " +
                                      "(@OpportunityTitle,@Description,@Responsibility,@Requirement,@CreatedOrgId,@CreatedDate,@CreatedUserId,@StartDate,@EndDate,@Schedule,@StatusId,@SiteAddress);" +
                                      "Select newID=MAX(id) FROM Opportunity"; 
                    }
                    else
                    {
                        sql = "Update Opportunity " +
                               "set OpportunityTitle = @OpportunityTitle," +
                                   "Description = @Description," +
                                   "Responsibility = @Responsibility," +
                                   "Requirement = @Requirement," +
                                   "CreatedOrgId = @CreatedOrgId," +
                                   "CreatedDate = @CreatedDate," +
                                   "CreatedUserId = @CreatedUserId," +
                                   "StartDate = @StartDate," +
                                   "EndDate = @EndDate," +
                                   "Schedule = @Schedule," +
                                   "StatusId = @StatusId," +
                                   "SiteAddress = @SiteAddress " +
                                "where id = '" + oppId + "' Delete OpportunityTag where OpportunityId='" + oppId + "' Select newID= " + oppId + " ";

                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@OpportunityTitle", this.OpportunityTitle);
                        cmd.Parameters.AddWithValue("@Description", this.Description);
                        cmd.Parameters.AddWithValue("@Responsibility", this.Responsibility);
                        cmd.Parameters.AddWithValue("@Requirement", this.Requirement);
                        cmd.Parameters.AddWithValue("@CreatedOrgId", this.CreatedOrgId);
                        cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedUserId", this.CreatedUserId);
                        cmd.Parameters.AddWithValue("@StartDate", this.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", this.EndDate);
                        cmd.Parameters.AddWithValue("@Schedule", this.Schedule);
                        cmd.Parameters.AddWithValue("@StatusId", this.StatusId); 
                        cmd.Parameters.AddWithValue("@SiteAddress", this.SiteAddress);
                        //cmd.ExecuteNonQuery();
                        newProdID = (Int32)cmd.ExecuteScalar();

                        if (newProdID != 0 && this.Tags.Count > 0)
                        {
                            foreach (Tag tag in this.Tags)
                            { 
                                SaveTags(newProdID.ToString(), tag.Id,"Insert");
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



        public string SaveTags(string oppId, string tagId, string action)
        {
            //save the association between Opportunity and tag

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

                    if (oppId != "" && tagId != "" && action.ToLower() == "insert")
                    {
                        sql = "INSERT INTO OpportunityTag " +
                                      "(OpportunityId,TagId) VALUES " +
                                      "(@OpportunityId,@TagId);";
                    }
                    else if (oppId != "" && tagId != "" && action.ToLower() == "delete")
                    {
                        sql = "Delete INTO OpportunityTag " +
                                      "(OpportunityId,TagId) VALUES " +
                                      "(@OpportunityId,@TagId);";
                    } 

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@OpportunityId", oppId);
                        cmd.Parameters.AddWithValue("@TagId", tagId);  
                        cmd.ExecuteNonQuery(); 
                    }
                }
            }
            catch (Exception ex)
            {
                result = "failed" + ex.Message;
            }
            return result;
        }

        public string Delete(string oppid) // int Id, string OpportunityTitle, string Description, string Responsibility, string Requirement, int CreatedOrgId, string CreatedDate, int CreatedUserId, int StatusId, string StartDate, string EndDate, string Tags)
        {
            //save the new Opportunity into the database

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "DELETE FROM Opportunity WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", oppid);

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
