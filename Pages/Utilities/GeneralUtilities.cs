//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;

namespace Outreach.Pages.Utilities
{
    public class GeneralUtilities
    {
        public GeneralUtilities()
        {

        }
        public int GetLoginUserIntIDbyGUID(string guid)
        {
            int intUserId = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select user_id from AspNetUsers with(nolock) where Id='" + guid + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            { 

                                if (reader["user_id"].GetType() != typeof(DBNull))
                                {
                                    intUserId = reader.GetInt32(0);
                                } 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }


            return intUserId;

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
                    string sql = "select top " + count.ToString() + " Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,SiteAddress from Opportunity with(nolock) where EndDate >= getdate() order by CreatedDate desc";
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

                                Tag tag = new Tag();
                                op.Tags = tag.GetReferencedTagsbyOpptunityId(op.Id);
                                 

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
        public List<Opportunity> SearchOpportunities(string searchtxt,string tagids="")
        { 

            List<Opportunity> listOpp = new List<Opportunity>();

            // retrive Opportunity data by Opportunity ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,SiteAddress from Opportunity o with(nolock) where EndDate >= getdate()  and OpportunityTitle like '%" + searchtxt + "%'";
                     
                    if (tagids != "")
                    {
                        sql = sql + " and o.Id in (Select OpportunityId from OpportunityTag where TagId in (" + tagids + "))";
                    }
                    sql = sql + " order by CreatedDate desc ";
                    //string sql = "select Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,Tags,SiteAddress from Opportunity with(nolock) where ID = " + searchtxt;
                    //string sql = "select Id,OpportunityTitle,Description,Responsibility,Requirement,CreatedOrgId,CreatedDate,CreatedUserId,StatusId,StartDate,EndDate,Schedule,StatusId,Tags,SiteAddress from Opportunity with(nolock) where ID = '" + searchtxt + "'";
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

                                Tag tag = new Tag();
                                op.Tags = tag.GetReferencedTagsbyOpptunityId(op.Id);

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
