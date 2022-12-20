//using Outreach.Pages.Clients;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
                sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId where ProjectName like  '%" + NameSearch + "%' order by ProjectName ";
            }
            else
            { // get all project
                sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId order by ProjectName "; 
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
                sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId where ProjectTaskStatusId = " + statusId + " order by ProjectName";
            }
            else
            { // get all project
                sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId order by ProjectName "; 
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

                                //if (reader["ProjectManagerUserId"].GetType() != typeof(DBNull))
                                //{
                                //    p.ProjectManagerUserId = reader.GetInt32(8).ToString();
                                //}

                                if (reader["StartDate"].GetType() != typeof(DBNull))
                                {
                                    p.StartDate = reader.GetDateTime(8).ToString();
                                }

                                if (reader["DueDate"].GetType() != typeof(DBNull))
                                {
                                    p.DueDate = reader.GetDateTime(9).ToString();
                                }

                                if (reader["CompletionDate"].GetType() != typeof(DBNull))
                                {
                                    p.CompletionDate = reader.GetDateTime(10).ToString();
                                }

                                if (reader["ProjectTaskStatusId"].GetType() != typeof(DBNull))
                                {
                                    p.ProjectTaskStatusId = reader.GetInt32(11).ToString();
                                }

                                if (reader["ProjectTaskStatus"].GetType() != typeof(DBNull))
                                {
                                    p.ProjectTaskStatus = reader["ProjectTaskStatus"].ToString();
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
                    string sql = "select Id,StatusName from ProjectTaskStatus with(nolock) order by Id";
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


        public List<StandardStatus> GetStandardStatusList()
        { // retrive StandardStatus data by StandardStatus ID

            List<StandardStatus> listStandardStatus = new List<StandardStatus>();

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select Id,StatusName from StandardStatus with(nolock) order by Id";
                    //else 
                    //    sql = "select Id,Name from StandardStatus with(nolock) order by Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {
                                StandardStatus s = new StandardStatus();

                                s.Id = reader.GetInt32(0).ToString();
                                if (reader["StatusName"].GetType() != typeof(DBNull))
                                {
                                    s.StatusName = reader["StatusName"].ToString();
                                }

                                listStandardStatus.Add(s);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message.ToString());
            }

            return listStandardStatus;
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
                    String sql = "SELECT User_Id,UserName,Email,Password='',Created_at=convert(varchar,Created_at),firstName, lastName ,PhoneNumber FROM AspNetUsers  with(nolock) order by firstName, lastName";
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



        public List<ProjectTaskUser> GetProjectorTaskUserList(string ProjectId = "", string TaskId = "", string IsLead = "")
        { // retrive login user by org ID in the future, now just list all

            List<ProjectTaskUser> userlist = new List<ProjectTaskUser>(); 
            
            string sql = "";

            if (TaskId.Trim() != "")
            {// get all task level users
                if (IsLead.Trim().ToLower() == "true")
                    sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where TaskId='" + TaskId + "' and isnull(IsLead,0) = 1 order by Id";
                else if(IsLead.Trim().ToLower() == "false")
                    sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where TaskId='" + TaskId + "' and isnull(IsLead,0) = 0 order by Id";
                else // (IsLead.Trim() == "")
                    sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where TaskId='" + TaskId + "' order by Id"; 
            }
            else
            { // get all project level users  
                if (IsLead.Trim().ToLower() == "true")
                    sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where ProjectId='" + ProjectId + "' and isnull(IsLead,0) = 1 order by Id";
                else if (IsLead.Trim().ToLower() == "false")
                    sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where ProjectId='" + ProjectId + "' and isnull(IsLead,0) = 0 order by Id";
                else // (IsLead.Trim() == "")
                    sql = "select Id,ProjectId,TaskId,UserId,IsLead from ProjectTaskUser with(nolock) where ProjectId='" + ProjectId + "' order by Id";
            }

            userlist = GetProjectorTaskUserListbySQL(sql);

            return userlist;
        }

        public List<ProjectTaskUser> GetProjectorTaskUserListbySQL(string sql)
        { // retrive ProjectTask_User data by ProjectTask_User ID
            List<ProjectTaskUser> userlist = new List<ProjectTaskUser>();
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
                                ProjectTaskUser userinfo = new ProjectTaskUser();
                                userinfo.Id = reader.GetInt32(0).ToString();
                                userinfo.ProjectId = reader.GetInt32(1).ToString();
                                userinfo.UserId = reader.GetInt32(3).ToString();

                                if (reader["TaskId"].GetType() != typeof(DBNull))
                                {// task level user
                                    userinfo.TaskId = reader["TaskId"].ToString();
                                }
                                if (reader["IsLead"].GetType() != typeof(DBNull))
                                {// task level user
                                    userinfo.IsLead = reader["IsLead"].ToString();
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


        public string DeleteAllProjectTaskUser(string ProjectId = "", string TaskId = "", string IsLead = "")
        {  
            string result = "";
            List<ProjectTaskUser> userlist = new List<ProjectTaskUser>();

            string sql = "";

            if (TaskId.Trim() != "")
            {// get all task level users
                if (IsLead.Trim().ToLower() == "true")
                    sql = "Delete ProjectTaskUser where TaskId='" + TaskId + "' and isnull(IsLead,0) = 1 ";
                else if (IsLead.Trim().ToLower() == "false")
                    sql = "Delete ProjectTaskUser where TaskId='" + TaskId + "' and isnull(IsLead,0) = 0 ";
                else // (IsLead.Trim() == "")
                    sql = "Delete ProjectTaskUser where TaskId='" + TaskId + "' ";
            }
            else
            { // get all project level users  
                if (IsLead.Trim().ToLower() == "true")
                    sql = "Delete ProjectTaskUser where ProjectId='" + ProjectId + "' and isnull(IsLead,0) = 1 ";
                else if (IsLead.Trim().ToLower() == "false")
                    sql = "Delete ProjectTaskUser where ProjectId='" + ProjectId + "' and isnull(IsLead,0) = 0 ";
                else // (IsLead.Trim() == "")
                    sql = "Delete ProjectTaskUser where ProjectId='" + ProjectId + "' "; 
            }

            result = DeleteTableDataBySQL(sql);

            return result;
        }

        public string DeleteTableDataBySQL(string sql)
        {// move to utility
            string result = "ok";
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
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

        public string DeleteAllDepartmentUser(string Dept_Id = "", string IsLead = "")
        { //  

            string result = "";
            List<ProjectTaskUser> userlist = new List<ProjectTaskUser>();

            string sql = " ";
             
            if (IsLead.Trim().ToLower() == "true")
                sql = "Delete DepartmentUser where DepartmentId='" + Dept_Id + "' and isnull(IsLead,0) = 1 ";
            else if (IsLead.Trim().ToLower() == "false")
                sql = "Delete DepartmentUser where DepartmentId='" + Dept_Id + "' and isnull(IsLead,0) = 0 ";
            else // (IsLead.Trim() == "")
                sql = "Delete DepartmentUser where DepartmentId='" + Dept_Id + " ";


            result = DeleteTableDataBySQL(sql);

            return result;
        } 

        public List<LoginUserInfo> ResetProjectTaskUserList(List<LoginUserInfo> originalloginUserlist, List<ProjectTaskUser> ptuserlist)
        { // mark if user is selected
            List<LoginUserInfo> finalloginUserlist = new List<LoginUserInfo>();

            //originalloginUserlist.ForEach(u => finalloginUserlist.Add(u));
            finalloginUserlist = originalloginUserlist.ToList();
            //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist


            if (ptuserlist != null && ptuserlist.Count > 0)
            {
                foreach (ProjectTaskUser ptu in ptuserlist)
                {
                    foreach (LoginUserInfo userinfo in finalloginUserlist)
                    {
                        if (ptu.UserId == userinfo.User_Id)
                        {
                            userinfo.IsSelected = "selected";
                        }
                    }
                }
            }
            return finalloginUserlist;
        }


        public List<LoginUserInfo> ResetDepartmentUserList(List<LoginUserInfo> originalloginUserlist, List<DepartmentUser> ptuserlist)
        { // mark if user is selected
            List<LoginUserInfo> finalloginUserlist = new List<LoginUserInfo>();

            //originalloginUserlist.ForEach(u => finalloginUserlist.Add(u));
            finalloginUserlist = originalloginUserlist.ToList();
            //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist


            if (ptuserlist != null && ptuserlist.Count > 0)
            {
                foreach (DepartmentUser ptu in ptuserlist)
                {
                    foreach (LoginUserInfo userinfo in finalloginUserlist)
                    {
                        if (ptu.UserId == userinfo.User_Id)
                        {
                            userinfo.IsSelected = "selected";
                        }
                    }
                }
            }
            return finalloginUserlist;
        }

        public Boolean IsProjTaskMemberChanged(List<ProjectTaskUser> ptuserlist, List<string> newuidlist)
        { // mark if user is selected
            Boolean match = false;

            if (ptuserlist != null && newuidlist != null)
            {
                List<string> ori_uidlist = new List<string>();
                ptuserlist.ForEach(u => ori_uidlist.Add(u.UserId));
                match = CompareTwoList(ori_uidlist, newuidlist);
            }
            return match;
        }
        public Boolean IsDepartmentMemberChanged(List<DepartmentUser> ptuserlist, List<string> newuidlist)
        { // mark if user is selected
            Boolean match = false;

            if (ptuserlist != null && newuidlist != null)
            {
                List<string> ori_uidlist = new List<string>();
                ptuserlist.ForEach(u => ori_uidlist.Add(u.UserId));
                match = CompareTwoList(ori_uidlist, newuidlist);
            }
            return match;
        }

        public Boolean CompareTwoList(List<string> list1, List<string> list2)
        { // check if two list of string matched
            Boolean match = false; 

            List<string> Commonlist = list1.Intersect(list2).ToList();
            //var ids = list1.Select(x => x.Id).Intersect(list2.Select(x => x.Id));


            if (Commonlist.Count > 0 && Commonlist.Count == list1.Count && list1.Count == list2.Count )
            {
                match = true;
            }

            return match;
        }
        public bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
         


        public List<Department> GetDepartmentListByNameSearch(string NameSearch = "")
        { // retrive Team/Department info by part of its name 

            List<Department> listPro = new List<Department>();
            string sql = "";

            if (NameSearch.Trim() != "")
            {
                sql = "Select Id,Name,Description,OrganizationId,CreatedDate,CreatedUserId,StatusId from Department with(nolock) where statusid=1 and Name like  '%" + NameSearch + "%' order by Name ";
            }
            else
            { // get all active Team/Department
                sql = "Select Id,Name,Description,OrganizationId,CreatedDate,CreatedUserId,StatusId from Department with(nolock) where statusid=1  order by Name ";
            }

            listPro = GetDepartmentListBySQLQuery(sql);

            return listPro;
        }


        public List<Department> GetDepartmentListBySQLQuery(string sql)
        { // retrive Project data by given sql query

            List<Department> listPro = new List<Department>();

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

                                Department p = new Department();

                                p.Id = reader.GetInt32(0).ToString();
                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    p.Name = reader["Name"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    p.Description = reader["Description"].ToString();
                                } 

                                if (reader["OrganizationId"].GetType() != typeof(DBNull))
                                {
                                    p.OrganizationId = reader["OrganizationId"].ToString();
                                }

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedDate = reader["CreatedDate"].ToString();
                                }

                                if (reader["CreatedUserId"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedUserId = reader["CreatedUserId"].ToString();
                                } 
                                 
                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    p.StatusId = reader["StatusId"].ToString();
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


        public List<DepartmentUser> GetDepartmentUserList(string DepartmentId = "", string IsLead = "")
        { // retrive login user by org ID in the future, now just list all

            List<DepartmentUser> userlist = new List<DepartmentUser>();

            string sql = "";

              // get all Department level users  
            if (IsLead.Trim().ToLower() == "true")
                sql = "select Id,DepartmentId,UserId,IsLead from DepartmentUser with(nolock) where DepartmentId='" + DepartmentId + "' and isnull(IsLead,0) = 1 order by Id";
            else if (IsLead.Trim().ToLower() == "false")
                sql = "select Id,DepartmentId,UserId,IsLead from DepartmentUser with(nolock) where DepartmentId='" + DepartmentId + "' and isnull(IsLead,0) = 0 order by Id";
            else // (IsLead.Trim() == "")
                sql = "select Id,DepartmentId,UserId,IsLead from DepartmentUser with(nolock) where DepartmentId='" + DepartmentId + "' order by Id";
          

            userlist = GetDepartmentUserListbySQL(sql);

            return userlist;
        }

        public List<DepartmentUser> GetDepartmentUserListbySQL(string sql)
        { // retrive DepartmentTask_User data by DepartmentTask_User ID
            List<DepartmentUser> userlist = new List<DepartmentUser>();
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
                                DepartmentUser userinfo = new DepartmentUser();
                                userinfo.Id = reader.GetInt32(0).ToString();
                                userinfo.DepartmentId = reader.GetInt32(1).ToString();
                                userinfo.UserId = reader.GetInt32(2).ToString();

                                if (reader["IsLead"].GetType() != typeof(DBNull))
                                {// task level user
                                    userinfo.IsLead = reader["IsLead"].ToString();
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
