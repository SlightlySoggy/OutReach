//using Outreach.Pages.Clients;
using Microsoft.VisualBasic;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
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



        public List<Project> GetProjectListByNameSearch(string NameSearch = "")
        { // retrive Project data by partial of name 

            List<Project> listPro = new List<Project>();  
            string sql = "";

            if (NameSearch.Trim() != "")
            {
                sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) where ProjectName like  '%" + NameSearch + "%' order by ProjectName ";
            }
            else
            { // get all project
                sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) order by ProjectName";
            }

            listPro = GetProjectListBySQLQuery(sql); 

            return listPro;
        }

        public List<Project> GetProjectListByStatusId(string statusId = "")
        { // retrive Project data by status ID
            List<Project> listPro = new List<Project>();
            string sql = "";

            if (statusId.Trim() != "")
            {
                sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) where ProjectTaskStatusId = " + statusId + " order by ProjectName";
            }
            else
            { // get all project
                sql = "select Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,ProjectManagerUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,DurationByDay from Project with(nolock) order by ProjectName";
            }

            listPro = GetProjectListBySQLQuery(sql);

            return listPro;
        }
         
    public List<Project> GetProjectListBySQLQuery(string sql)
        { // retrive Project data by given sql query

            List<Project> listPro = new List<Project>();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {

                                Project p = new Project();

                                p.Id = reader.GetInt32(0).ToString();
                                if (reader["ProjectName"].GetType() != typeof(DBNull))
                                {
                                    p.ProjectName = reader["ProjectName"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    p.Description = reader["Description"].ToString();
                                }

                                if (reader["EstimatedBudget"].GetType() != typeof(DBNull))
                                {
                                    p.EstimatedBudget = reader["EstimatedBudget"].ToString();
                                }

                                if (reader["ActualSpent"].GetType() != typeof(DBNull))
                                {
                                    p.ActualSpent = reader["ActualSpent"].ToString();
                                }


                                if (reader["ActualSpent"].GetType() != typeof(DBNull))
                                {
                                    p.ActualSpent = reader["ActualSpent"].ToString();
                                }


                                if (reader["CreatedOrgId"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedOrgId = reader.GetInt32(5).ToString();
                                }

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedDate = reader.GetDateTime(6).ToString();
                                }

                                if (reader["CreatedUserId"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedUserId = reader.GetInt32(7).ToString();
                                }

                                if (reader["ProjectManagerUserId"].GetType() != typeof(DBNull))
                                {
                                    p.ProjectManagerUserId = reader.GetInt32(8).ToString();
                                }

                                if (reader["StartDate"].GetType() != typeof(DBNull))
                                {
                                    p.StartDate = reader.GetDateTime(9).ToString();
                                }

                                if (reader["DueDate"].GetType() != typeof(DBNull))
                                {
                                    p.DueDate = reader.GetDateTime(10).ToString();
                                }

                                if (reader["CompletionDate"].GetType() != typeof(DBNull))
                                {
                                    p.CompletionDate = reader.GetDateTime(11).ToString();
                                }

                                if (reader["ProjectTaskStatusId"].GetType() != typeof(DBNull))
                                {
                                    p.ProjectTaskStatusId = reader.GetInt32(12).ToString();
                                }


                                listPro.Add(p);
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


        public List<ProjTaskStatus> GetProjTaskStatusList()
        { // retrive ProjTaskStatus data by ProjTaskStatus ID

            List<ProjTaskStatus> listProjTaskStatus = new List<ProjTaskStatus>();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql =  "select Id,StatusName from ProjTaskStatus with(nolock)  order by Id";
                    //else 
                    //    sql = "select Id,Name from ProjTaskStatus with(nolock) order by Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {
                                ProjTaskStatus s = new ProjTaskStatus();

                                s.Id = reader.GetInt32(0).ToString();
                                if (reader["StatusName"].GetType() != typeof(DBNull))
                                {
                                    s.StatusName = reader["StatusName"].ToString();
                                }

                                listProjTaskStatus.Add(s);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message.ToString());
            }

            return listProjTaskStatus;
        }

        public List<LoginUserInfo> GetLoginUserList(string OrgId="")
        { // retrive login user by org ID in the future, now just list all

            List<LoginUserInfo> userlist= new List<LoginUserInfo>();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT User_Id,UserName,Email,Password='',Created_at=convert(varchar,Created_at),firstName, lastName ,PhoneNumber FROM AspNetUsers  with(nolock) ";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoginUserInfo userinfo = new LoginUserInfo();
                                userinfo.User_Id = reader.GetInt32(0).ToString();
                                userinfo.UserName = reader.GetString(1);
                                userinfo.Email = reader.GetString(2);
                                userinfo.Password = reader.GetString(3);
                                userinfo.Created_at = reader.GetString(4);


                                if (reader["firstName"].GetType() != typeof(DBNull))
                                {
                                    userinfo.firstName = reader["firstName"].ToString();
                                }

                                if (reader["lastName"].GetType() != typeof(DBNull))
                                {
                                    userinfo.lastName = reader["lastName"].ToString();
                                }

                                if (reader["PhoneNumber"].GetType() != typeof(DBNull))
                                {
                                    userinfo.PhoneNumber = reader["PhoneNumber"].ToString();
                                }
                                  
                                userlist.Add(userinfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message.ToString());
            }

            return userlist;
        }


    }
}
