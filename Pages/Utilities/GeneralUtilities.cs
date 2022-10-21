using MyAffTest.Pages.Clients;
using System.Data;
using System.Data.SqlClient;

namespace MyAffTest.Pages.Utilities
{
    public class GeneralUtilities
    {
        public GeneralUtilities()
        {

        }
        public List<Opportunity> GetRecentPostOpportunities(int count)
        {
            if (count <= 0)
                count = 1000;

            List<Opportunity> listOpp = new List<Opportunity>();
             
            // retrive Opportunity data by Opportunity ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select top " + count.ToString() + " Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,Tags,SiteAddress from Opportunity with(nolock) where EndDate >= getdate() order by CreatedDate desc";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Opportunity op = new Opportunity();
                                //ClientInfo clientInfo = new ClientInfo();
                                op.Id = reader.GetInt32(0).ToString();
                                if (reader["OpportunityTitle"].GetType() != typeof(DBNull))
                                {
                                    op.OpportunityTitle = reader["OpportunityTitle"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    op.Description = reader["Description"].ToString();
                                }

                                if (reader["Responsibility"].GetType() != typeof(DBNull))
                                {
                                    op.Responsibility = reader["Responsibility"].ToString();
                                }

                                if (reader["Requirement"].GetType() != typeof(DBNull))
                                {
                                    op.Requirement = reader["Requirement"].ToString();
                                } 
                                op.CreatedOrgId = reader.GetInt32(5).ToString();
                                op.CreatedDate = reader.GetDateTime(6).ToString();
                                op.CreatedUserId = reader.GetInt32(7).ToString();
                                op.StatusId = reader.GetInt32(8).ToString();
                                op.StartDate = reader.GetDateTime(9).ToString();
                                op.EndDate = reader.GetDateTime(10).ToString();
                                //op.Schedule = (reader.GetString(11) == DBNull) ?"" : reader.GetString(11);
                                if (reader["Schedule"].GetType() != typeof(DBNull))
                                { 
                                    op.Schedule = reader["Schedule"].ToString();
                                }
                                 
                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    op.StatusId = reader["StatusId"].ToString();
                                }

                                if (reader["Tags"].GetType() != typeof(DBNull))
                                {
                                    op.Tags = reader["Tags"].ToString();
                                }

                                if (reader["SiteAddress"].GetType() != typeof(DBNull))
                                {
                                    op.SiteAddress = reader["SiteAddress"].ToString();
                                } 

                                listOpp.Add(op);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }

           return listOpp;

        }
    }
}
