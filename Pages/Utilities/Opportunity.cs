using MyAffTest.Pages.Clients;
using System.Data;
using System.Data.SqlClient;


namespace MyAffTest.Pages.Utilities
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
        public string Tags;
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
            Tags = "";
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
                    string sql = "select Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,Tags,SiteAddress from Opportunity with(nolock) where Id=" + Opp_Id;
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
                                StatusId = reader.GetInt32(8).ToString();
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

                                if (reader["Tags"].GetType() != typeof(DBNull))
                                {
                                    Tags = reader["Tags"].ToString();
                                }

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
                Console.WriteLine("Exception: " + ex.ToString());
            }

        }

        public string Save() // int Id, string OpportunityTitle, string Description, string Responsibility, string Requirement, int CreatedOrgId, string CreatedDate, int CreatedUserId, int StatusId, string StartDate, string EndDate, string Tags)
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
                    string sql = "INSERT INTO Opportunity " +
                                 "(OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,EndDate,Schedule,StatusId,Tags,SiteAddress) VALUES " +
                                 "(@OpportunityTitle,@Description,@Responsibility,@Requirement,@CreatedOrgId,@CreatedDate,@CreatedUserId,@StartDate,@EndDate,@Schedule,@StatusId,@Tags,@SiteAddress);";
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
                        cmd.Parameters.AddWithValue("@Tags", this.Tags);
                        cmd.Parameters.AddWithValue("@SiteAddress", this.SiteAddress);
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
    }
}
