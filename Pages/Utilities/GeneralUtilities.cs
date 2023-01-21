//using Outreach.Pages.Clients;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic;
using Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public List<Opportunity> SearchOpportunities(string searchtxt, string tagids = "", string orderby = "")
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

                    if (tagids != null && tagids != "")
                    {
                        sql = sql + " and o.Id in (Select OpportunityId from OpportunityTag where TagId in (" + tagids + "))";
                    }
                    if (orderby == "title")
                    {
                        sql = sql + " order by OpportunityTitle ";
                    }
                    else if (orderby == "date")
                    {
                        sql = sql + " order by CreatedDate ";
                    }
                    else 
                        sql = sql + " order by CreatedDate desc";

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

            if (statusId.Trim() != "" && statusId.Trim() != "-1") 
            {
                sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId where ProjectTaskStatusId = " + statusId + " order by ProjectName";
            }
            else if (statusId.Trim() == "-1")
            { // get planed and in progress projects list
                sql = "select P.Id,ProjectName,Description,EstimatedBudget,ActualSpent,CreatedOrgId,CreatedDate,CreatedUserId,StartDate,DueDate,CompletionDate,ProjectTaskStatusId,ProjectTaskStatus=S.StatusName from Project p with(nolock) left join ProjectTaskStatus S on S.Id=P.ProjectTaskStatusId where ProjectTaskStatusId in (1,2) order by ProjectName";
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

            GeneralUtilities ut = new GeneralUtilities();
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

                                if (reader["CreatedOrgId"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedOrgId = reader["CreatedOrgId"].ToString();
                                }

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedDate = ut.EmptyDateConvert(reader["CreatedDate"].ToString());
                                }

                                if (reader["CreatedUserId"].GetType() != typeof(DBNull))
                                {
                                    p.CreatedUserId = reader["CreatedUserId"].ToString();
                                }

                                if (reader["StartDate"].GetType() != typeof(DBNull))
                                {
                                    p.StartDate = ut.EmptyDateConvert(reader["StartDate"].ToString());
                                }

                                if (reader["DueDate"].GetType() != typeof(DBNull))
                                {
                                    p.DueDate = ut.EmptyDateConvert(reader["DueDate"].ToString());
                                }

                                if (reader["CompletionDate"].GetType() != typeof(DBNull))
                                {
                                    p.CompletionDate = ut.EmptyDateConvert(reader["CompletionDate"].ToString());
                                }
                                if (reader["ProjectTaskStatusId"].GetType() != typeof(DBNull))
                                {
                                    p.ProjectTaskStatusId = reader["ProjectTaskStatusId"].ToString();
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

       public List<UserLinkage> GetLinkedUserList(string groupTypeId, string linkedGroupId, string isLead = "")
        { // return all userIds who link to specified linkedGroupId 
          // GroupTypeId;  //1:Organization,2:Team,3:Project,4:Task
          // LinkedGroupId; //value can be: OrganizationId, TeamId, ProjectId, TaskId


            List<UserLinkage> userlist = new List<UserLinkage>();

            string sql = "";

            if (isLead.Trim().ToLower() == "true" || isLead.Trim().ToLower() == "1")
                sql = "select Id,UserId,IsLead from UserLinkage with(nolock) where GroupTypeId='" + groupTypeId + "' and LinkedGroupId = '" + linkedGroupId + "' and  isnull(IsLead,0) = 1 order by Id";
            else if (isLead.Trim().ToLower() == "false" || isLead.Trim().ToLower() == "0")
                sql = "select Id,UserId,IsLead from UserLinkage with(nolock) where GroupTypeId='" + groupTypeId + "' and LinkedGroupId = '" + linkedGroupId + "' and  isnull(IsLead,0) = 0 order by Id";

 
            userlist = GetLinkedUserListbySQL(sql);

            return userlist;
        }

        public List<UserLinkage> GetLinkedUserListbySQL(string sql)
        { // retrive ProjectTask_User data by ProjectTask_User ID
            List<UserLinkage> userlist = new List<UserLinkage>();
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
                                UserLinkage ul = new UserLinkage();
                                if (reader["Id"].GetType() != typeof(DBNull))
                                {
                                    ul.Id = reader["Id"].ToString();
                                }
                                if (reader["UserId"].GetType() != typeof(DBNull))
                                {
                                    ul.UserId = reader["UserId"].ToString();
                                }
                                if (reader["IsLead"].GetType() != typeof(DBNull))
                                {
                                    ul.IsLead = reader["IsLead"].ToString(); 
                                }

                                // other infor do not needed at this point 

                                userlist.Add(ul);
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
         


        public string DeleteAllUserLinkage(string ProjectId = "", string TaskId = "", string IsLead = "")
        {  
            string result = "";
            List<UserLinkage> userlist = new List<UserLinkage>();

            string sql = "";

            if (TaskId.Trim() != "")
            {// get all task level users
                if (IsLead.Trim().ToLower() == "true")
                    sql = "Delete UserLinkage where TaskId='" + TaskId + "' and isnull(IsLead,0) = 1 ";
                else if (IsLead.Trim().ToLower() == "false")
                    sql = "Delete UserLinkage where TaskId='" + TaskId + "' and isnull(IsLead,0) = 0 ";
                else // (IsLead.Trim() == "")
                    sql = "Delete UserLinkage where TaskId='" + TaskId + "' ";
            }
            else
            { // get all project level users  
                if (IsLead.Trim().ToLower() == "true")
                    sql = "Delete UserLinkage where ProjectId='" + ProjectId + "' and isnull(IsLead,0) = 1 ";
                else if (IsLead.Trim().ToLower() == "false")
                    sql = "Delete UserLinkage where ProjectId='" + ProjectId + "' and isnull(IsLead,0) = 0 ";
                else // (IsLead.Trim() == "")
                    sql = "Delete UserLinkage where ProjectId='" + ProjectId + "' "; 
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

        public string DeleteAllTeamUser(string teamId = "", string IsLead = "")
        { //  

            string result = "";
            List<UserLinkage> userlist = new List<UserLinkage>();

            string sql = " ";
             
            if (IsLead.Trim().ToLower() == "true")
                sql = "Delete TeamUser where TeamId='" + teamId + "' and isnull(IsLead,0) = 1 ";
            else if (IsLead.Trim().ToLower() == "false")
                sql = "Delete TeamUser where TeamId='" + teamId + "' and isnull(IsLead,0) = 0 ";
            else // (IsLead.Trim() == "")
                sql = "Delete TeamUser where TeamId='" + teamId + " ";


            result = DeleteTableDataBySQL(sql);

            return result;
        } 

        public List<LoginUserInfo> ResetUserLinkageList(List<LoginUserInfo> originalloginUserlist, List<UserLinkage> UserList)
        { // mark if user is selected
            List<LoginUserInfo> finalloginUserlist = new List<LoginUserInfo>();

            //originalloginUserlist.ForEach(u => finalloginUserlist.Add(u));
            finalloginUserlist = originalloginUserlist.ToList();
            //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist


            if (UserList != null && UserList.Count > 0)
            {
                foreach (UserLinkage uid in UserList)
                {
                    foreach (LoginUserInfo userinfo in finalloginUserlist)
                    {
                        if (uid.UserId == userinfo.User_Id)
                        {
                            userinfo.IsSelected = "selected";
                        }
                    }
                }
            }
            return finalloginUserlist;
        }


        public List<LoginUserInfo> ResetTeamUserList(List<LoginUserInfo> originalloginUserlist, List<TeamUser> ptuserlist)
        { // mark if user is selected
            List<LoginUserInfo> finalloginUserlist = new List<LoginUserInfo>();

            //originalloginUserlist.ForEach(u => finalloginUserlist.Add(u));
            finalloginUserlist = originalloginUserlist.ToList();
            //since the originalloginUserlist will be changed along with finalloginUserlist, the next call should reload originalloginUserlist


            if (ptuserlist != null && ptuserlist.Count > 0)
            {
                foreach (TeamUser ptu in ptuserlist)
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

        public Boolean IsProjTaskMemberChanged(List<UserLinkage> ptuserlist, List<string> newuidlist)
        { // check if member selection has changed
            Boolean ifchanged = false;

            //if (ptuserlist != null && newuidlist != null)
            if (ptuserlist.Count > 0 && newuidlist.Count > 0 && ptuserlist.Count != newuidlist.Count)
            {//if two list has different count, they are definitely changed.
                List<string> ori_uidlist = new List<string>();
                ptuserlist.ForEach(u => ori_uidlist.Add(u.UserId));
                ifchanged = CompareTwoList(ori_uidlist, newuidlist);
            }
            return ifchanged;
        }

        public string ProcessLinkedUsers(List<UserLinkage> ptuserlist, List<string> newuidlist, string groupTypeId, string linkedGroupId, string isLead = "")
        { // Delete the members from UserLinkage if they are not in the newuidlist, add new member if 
          // GroupTypeId;  //1:Organization,2:Team,3:Project,4:Task
          // LinkedGroupId; //value can be: OrganizationId, TeamId, ProjectId, TaskId

        string result = "ok";
            List<string> memberTobeDeletedIds = new List<string>(); //return the Id list of member need to be deleted.
            List<string> memberTobeAddedIds = new List<string>(); //return the Id list of member need to be deleted.
             
            if (ptuserlist.Count > 0 && newuidlist.Count == 0)
            {//delete all, no new member
                ptuserlist.ForEach(u => memberTobeDeletedIds.Add(u.UserId));
            }
            else if (ptuserlist.Count == 0 && newuidlist.Count > 0)
            {//all new members
                memberTobeAddedIds = newuidlist;
            }
            else  
            { 
                var differentMembers1 = ptuserlist.Where(p => newuidlist.All(p2 => p2 != p.UserId)).ToList<UserLinkage>();
                differentMembers1.ForEach(u => memberTobeDeletedIds.Add(u.Id));

                var differentMembers2 = newuidlist.Where(p2 => ptuserlist.All(p => p2 != p.UserId)).ToList<string>();
                differentMembers2.ForEach(u => memberTobeAddedIds.Add(u)); 
            } 

            if (memberTobeDeletedIds.Count > 0)
            {  
                foreach (string id in memberTobeDeletedIds)
                {
                    if (IsNumeric(id))
                    { // save Task level lead user
                        UserLinkage ptu = new UserLinkage();
                        ptu.Id = id;
                        result = ptu.Delete(); 
                    }
                }
            }


            foreach (string uid in memberTobeAddedIds)
            { //add new members
                if (IsNumeric(uid))
                { // save Task level lead user
                    UserLinkage ptu = new UserLinkage(); 
                    ptu.UserId = uid;
                    ptu.GroupTypeId = groupTypeId;
                    ptu.LinkedGroupId = linkedGroupId;
                    ptu.IsLead = isLead; //regulare member
                    result = ptu.Save();
                }
            }

            return result;
        }
        public Boolean IsTeamMemberChanged(List<TeamUser> ptuserlist, List<string> newuidlist)
        { // mark if user is selected
            Boolean match = false;

            //if (ptuserlist != null && newuidlist != null) 
            if (ptuserlist.Count > 0 && newuidlist.Count > 0 && ptuserlist.Count != newuidlist.Count)
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
         


        public List<Team> GetTeamListByNameSearch(string NameSearch = "")
        { // retrive Team/Team info by part of its name 

            List<Team> listPro = new List<Team>();
            string sql = "";

            if (NameSearch.Trim() != "")
            {
                sql = "Select t.Id,Name,Description,OrganizationId,CreatedDate,CreatedUserId,StatusId ,Status=st.StatusName from Team t with(nolock)  left join StandardStatus st on st.Id = t.StatusId  where t.statusid=1 and t.Name like  '%" + NameSearch + "%' order by t.Name ";
            }
            else
            { // get all active Team/Team
                sql = "Select t.Id,Name,Description,OrganizationId,CreatedDate,CreatedUserId,StatusId ,Status=st.StatusName from Team t with(nolock)  left join StandardStatus st on st.Id = t.StatusId  where t.statusid=1  order by t.Name ";
            }

            listPro = GetTeamListBySQLQuery(sql);

            return listPro;
        }


        public List<Team> GetTeamListBySQLQuery(string sql)
        { // retrive Project data by given sql query

            List<Team> listPro = new List<Team>();

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

                                Team p = new Team();

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
                                if (reader["Status"].GetType() != typeof(DBNull))
                                {
                                    p.Status = reader["Status"].ToString();
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


        public List<TeamUser> GetTeamUserList(string TeamId = "", string IsLead = "")
        { // retrive login user by org ID in the future, now just list all

            List<TeamUser> userlist = new List<TeamUser>();

            string sql = "";

              // get all Team level users  
            if (IsLead.Trim().ToLower() == "true")
                sql = "select Id,TeamId,UserId,IsLead from TeamUser with(nolock) where TeamId='" + TeamId + "' and isnull(IsLead,0) = 1 order by Id";
            else if (IsLead.Trim().ToLower() == "false")
                sql = "select Id,TeamId,UserId,IsLead from TeamUser with(nolock) where TeamId='" + TeamId + "' and isnull(IsLead,0) = 0 order by Id";
            else // (IsLead.Trim() == "")
                sql = "select Id,TeamId,UserId,IsLead from TeamUser with(nolock) where TeamId='" + TeamId + "' order by Id";
          

            userlist = GetTeamUserListbySQL(sql);

            return userlist;
        }

        public List<TeamUser> GetTeamUserListbySQL(string sql)
        { // retrive TeamTask_User data by TeamTask_User ID
            List<TeamUser> userlist = new List<TeamUser>();
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
                                TeamUser userinfo = new TeamUser();
                                userinfo.Id = reader.GetInt32(0).ToString();
                                userinfo.TeamId = reader.GetInt32(1).ToString();
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




        public List<Task> GetTaskListByNameSearch(string NameSearch = "")
        { // retrive Task data by partial of name 

            List<Task> listTask= new List<Task>();
            string sql = " select t.Id,t.Name,t.Description,t.CreatedDate,t.CreatedUserId,t.StartDate,t.DueDate,t.CompletionDate,t.ProjectTaskStatusId,ProjectTaskStatus=S.StatusName,tl.OrganizationId,tl.TeamId,tl.ProjectId,OrganizationName=o.name, p.ProjectName,TeamName=tm.name " +
                         " from Task t with(nolock) " +
                         " left join ProjectTaskStatus S on S.Id=t.ProjectTaskStatusId  " +
                         " left join TaskLinkage tl on tl.taskId=t.Id " +
                         " left join Organization o on o.Id = tl.OrganizationId " +
                         " left join Project p on p.Id = tl.ProjectId " +
                         " left join Team tm on tm.Id = tl.TeamId ";
            if (NameSearch.Trim() != "")
            {
                sql = sql  +  " where t.Name like  '%" + NameSearch + "%'  ";
            }
            else
            { // get all Task;  // dangours to return all task
            }

            sql = sql + "  order by t.Name ";

            listTask = GetTaskListBySQLQuery(sql);

            return listTask;
        }

        public List<Task> GetTaskListByStatusId(string statusId = "")
        { // retrive Task data by status ID
            List<Task> listTask = new List<Task>();
            string sql = " select t.Id,t.Name,t.Description,t.CreatedDate,t.CreatedUserId,t.StartDate,t.DueDate,t.CompletionDate,t.ProjectTaskStatusId,ProjectTaskStatus=S.StatusName,tl.taskId,tl.OrganizationId,tl.TeamId,tl.ProjectId,OrganizationName=o.name, p.ProjectName,TeamName=tm.name " +
                         " from Task t with(nolock) " +
                         " left join ProjectTaskStatus S on S.Id=t.ProjectTaskStatusId  " +
                         " left join TaskLinkage tl on tl.taskId=t.Id " +
                         " left join Organization o on o.Id = tl.OrganizationId " +
                         " left join Project p on p.Id = tl.ProjectId " +
                         " left join Team tm on tm.Id = tl.TeamId ";

            if (statusId.Trim() != "")
            {
                sql = sql + " where t.ProjectTaskStatusId = '" + statusId + "'  ";
            }
            else
            { // get all Task;  // dangours to return all task
            }

            sql = sql + "  order by t.Name ";

            listTask = GetTaskListBySQLQuery(sql);

            return listTask; 
        }

        public List<Task> GetTaskListBySQLQuery(string sql)
        { // retrive Task data by given sql query

            List<Task> listPro = new List<Task>();

            GeneralUtilities ut = new GeneralUtilities();
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

                                Task t= new Task();

                                t.Id = reader.GetInt32(0).ToString();
                                if (reader["Id"].GetType() != typeof(DBNull))
                                {
                                    t.Id = reader["Id"].ToString();
                                }

                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    t.Name = reader["Name"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    t.Description = reader["Description"].ToString();
                                }

                                if (reader["CreatedDate"].GetType() != typeof(DBNull))
                                {
                                    t.CreatedDate = reader["CreatedDate"].ToString();
                                }

                                if (reader["CreatedUserId"].GetType() != typeof(DBNull))
                                {
                                    t.CreatedUserId = reader["CreatedUserId"].ToString();
                                }

                                if (reader["StartDate"].GetType() != typeof(DBNull))
                                {
                                    t.StartDate = ut.EmptyDateConvert(reader["StartDate"].ToString());
                                }

                                if (reader["DueDate"].GetType() != typeof(DBNull))
                                {
                                    t.DueDate = ut.EmptyDateConvert(reader["DueDate"].ToString());
                                }

                                if (reader["CompletionDate"].GetType() != typeof(DBNull))
                                {
                                    t.CompletionDate = ut.EmptyDateConvert(reader["CompletionDate"].ToString());
                                }

                                if (reader["ProjectTaskStatusId"].GetType() != typeof(DBNull))
                                {
                                    t.ProjectTaskStatusId = reader["ProjectTaskStatusId"].ToString();
                                }

                                if (reader["ProjectTaskStatus"].GetType() != typeof(DBNull))
                                {
                                    t.ProjectTaskStatus = reader["ProjectTaskStatus"].ToString();
                                }

                                //p.TaskManagerUserIds = ut.GetProjectorTaskUserList("", TaskId, "true");
                                //p.TaskMemberUserIds = ut.GetProjectorTaskUserList("", TaskId, "false");

                                // Linkage : 
                                t.TaskLinkage = new TaskLinkage();


                                if (reader["taskId"].GetType() != typeof(DBNull))
                                {
                                    t.TaskLinkage.TaskId = reader["taskId"].ToString();
                                }

                                if (reader["OrganizationId"].GetType() != typeof(DBNull))
                                {// Organization level task
                                    t.TaskLinkage.OrganizationId = reader["OrganizationId"].ToString();
                                    if (reader["OrganizationName"].GetType() != typeof(DBNull))
                                    {
                                        t.TaskLinkage.OrganizationName = reader["OrganizationName"].ToString();
                                        t.TaskLinkage.BelongTo = "Organization: " + t.TaskLinkage.OrganizationName;
                                    }
                                }
                                else if (reader["TeamId"].GetType() != typeof(DBNull))
                                {// Team  level task
                                    t.TaskLinkage.TeamId = reader["TeamId"].ToString();
                                    if (reader["TeamName"].GetType() != typeof(DBNull))
                                    {
                                        t.TaskLinkage.TeamName = reader["TeamName"].ToString();
                                        t.TaskLinkage.BelongTo = "Team: " + t.TaskLinkage.TeamName;
                                    }
                                }
                                else if (reader["ProjectId"].GetType() != typeof(DBNull))
                                {// Project level task
                                    t.TaskLinkage.ProjectId = reader["ProjectId"].ToString();
                                    if (reader["ProjectName"].GetType() != typeof(DBNull))
                                    {
                                        t.TaskLinkage.ProjectName = reader["ProjectName"].ToString();
                                        t.TaskLinkage.BelongTo = "Project: " + t.TaskLinkage.ProjectName;
                                    }
                                }

                                listPro.Add(t);
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

        public string EmptyDateConvert(string inputdate)
        {// when database return date like "1/1/1900 12:00:00 AM" then convert it to ""

            string result = "";

            if (inputdate != "1/1/1900 12:00:00 AM")
                result = inputdate;
            else
                result = "";

            return result;

        }



        public List<Organization> GetOrganizationListByNameSearch(string NameSearch = "")
        { // retrive Organization info by part of its name 

            List<Organization> listPro = new List<Organization>();
            string sql = "";

            if (NameSearch.Trim() != "")
            {
                sql = "select o.Id,Name,Description,Address,PrimaryContactUserId,CreatedDate,CreatedUserId,StatusId,Phone,Email,WebURL,Status=st.StatusName from Organization o with(nolock) left join StandardStatus st on st.Id = o.StatusId where o.statusid=1 and o.Name like  '%" + NameSearch + "%' order by o.Name ";
            }
            else
            { // get all active Organization

                sql = "select o.Id,Name,Description,Address,PrimaryContactUserId,CreatedDate,CreatedUserId,StatusId,Phone,Email,WebURL,SStatus=st.StatusName from Organization o with(nolock) left join StandardStatus st on st.Id = o.StatusId where o.statusid=1  order by o.Name ";
            }

            listPro = GetOrganizationListBySQLQuery(sql);

            return listPro;
        }
        public Boolean IsUserAuthorizedtoAccessOrganization(string Guid = "",string OrgId = "")
        { // retrive Organization info by part of its name 
            Boolean result = false;
            List<Organization> lo = GetOrganizationListByUserGUID(Guid);
            foreach (Organization o in lo)
            {
                if (o.Id== OrgId)
                {
                    result = true;

                }
            }

            return result;
        }
        public List<Organization> GetOrganizationListByUserGUID(string Guid = "")
        { // retrive Organization info by part of its name 

            List<Organization> listPro = new List<Organization>();
            string sql = "";

            if (Guid.Trim() != "")
            {
                sql = "Select o.Id,O.Name,O.Description,O.Address,O.PrimaryContactUserId,O.CreatedDate,O.CreatedUserId,O.StatusId,O.Phone,O.Email,O.WebURL,Status=st.StatusName  from Organization o with(nolock) " +
                    " left join AspNetUsers u on u.User_Id = o.PrimaryContactUserId " +
                    " left join StandardStatus st on st.Id = o.StatusId    " +
                    " where o.statusid=1 and ((u.Id= '" + Guid + "') or " +
                    " exists (select ul.UserId from UserLinkage ul inner join AspNetUsers u2 on u2.User_Id = ul.UserId where ul.GroupTypeId=1 and ul.LinkedGroupId=o.Id and ul.IsLead=1 " +
                    " and u2.Id= '" + Guid + "'))"; 
            }
            //else
            //{ // get all active Organization

            //    sql = "select o.Id,Name,Description,Address,PrimaryContactUserId,CreatedDate,CreatedUserId,StatusId,Phone,Email,WebURL,Logo,Status=st.StatusName from Organization o with(nolock) left join StandardStatus st on st.Id = o.StatusId where o.statusid=1  order by o.Name ";
            //}

            listPro = GetOrganizationListBySQLQuery(sql);

            return listPro;
        }


        public List<Organization> GetOrganizationListBySQLQuery(string sql)
        { // retrive Project data by given sql query

            List<Organization> listPro = new List<Organization>();

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

                                Organization p = new Organization();

                                p.Id = reader.GetInt32(0).ToString();

                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    p.Name = reader["Name"].ToString();
                                }

                                if (reader["Description"].GetType() != typeof(DBNull))
                                {
                                    p.Description = reader["Description"].ToString();
                                }

                                if (reader["Address"].GetType() != typeof(DBNull))
                                {
                                    p.Address = reader["Address"].ToString();
                                }

                                if (reader["PrimaryContactUserId"].GetType() != typeof(DBNull))
                                {
                                    p.PrimaryContactUserId = reader["PrimaryContactUserId"].ToString();

                                    p.PrimaryContactUser = new LoginUserInfo(p.PrimaryContactUserId);
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

                                if (reader["Status"].GetType() != typeof(DBNull))
                                {
                                    p.Status = reader["Status"].ToString();
                                }

                                if (reader["Phone"].GetType() != typeof(DBNull))
                                {
                                    p.Phone = reader["Phone"].ToString();
                                }

                                if (reader["Email"].GetType() != typeof(DBNull))
                                {
                                    p.Email = reader["Email"].ToString();
                                }

                                if (reader["WebURL"].GetType() != typeof(DBNull))
                                {
                                    p.WebURL = reader["WebURL"].ToString();
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


        public List<LoginUserInfo> GetLoginUserList(string groupTypeId, string linkedGroupId, string isLead = "")
        { // return all userIds who link to specified linkedGroupId 
          // GroupTypeId;  //1:Organization,2:Team,3:Project,4:Task
          // LinkedGroupId; //value can be: OrganizationId, TeamId, ProjectId, TaskId


            List<LoginUserInfo> userlist = new List<LoginUserInfo>();

            string sql = "SELECT u.Id,u.User_Id,u.UserName,u.Email,Password='',Created_at=convert(varchar,Created_at),u.firstName, u.lastName,u.PhoneNumber " + 
                         " FROM AspNetUsers U with(nolock) " +
                         " Left join UserLinkage ul with(nolock) on ul.UserId = u.User_Id " + 
                         " Where ul.LinkedGroupId = '" + linkedGroupId + "' and ul.GroupTypeId = '" + groupTypeId + "' ";

            if (isLead.Trim().ToLower() == "true" || isLead.Trim().ToLower() == "1")
                sql = sql + " and Isnull(ul.isLead,0) = 1 ";
            else if (isLead.Trim().ToLower() == "false" || isLead.Trim().ToLower() == "0")
                sql = sql + " and Isnull(ul.isLead,0) = 0 "; 

            userlist = GetLoginUserListbySQL(sql);

            return userlist;
        }


        public List<LoginUserInfo> GetLoginUserListbySQL(string sql = "")
        {   // retrive login user by linkage
            //GroupTypeId  : 1:Organization,2:Team,3:Project,4:Task
            //LinkedGroupId: OrganizationId, TeamId, ProjectId, TaskId

            List<LoginUserInfo> userlist = new List<LoginUserInfo>();
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
                                LoginUserInfo userinfo = new LoginUserInfo();

                                if (reader["Id"].GetType() != typeof(DBNull))
                                {
                                    userinfo.Id = reader["Id"].ToString();
                                }
                                if (reader["User_Id"].GetType() != typeof(DBNull))
                                {
                                    userinfo.User_Id = reader["User_Id"].ToString();
                                }
                                if (reader["UserName"].GetType() != typeof(DBNull))
                                {
                                    userinfo.UserName = reader["UserName"].ToString();
                                }

                                if (reader["Email"].GetType() != typeof(DBNull))
                                {
                                    userinfo.Email = reader["Email"].ToString();
                                }
                                if (reader["Password"].GetType() != typeof(DBNull))
                                {
                                    userinfo.Password = reader["Password"].ToString();
                                }
                                if (reader["Created_at"].GetType() != typeof(DBNull))
                                {
                                    userinfo.Created_at = reader["Created_at"].ToString();
                                }

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


        public string SaveUploadFile(IFormFile uploadfile, string GroupTypeId, string LinkedGroupId, string FileTypeId, string UserId)
        { // https://www.aspsnippets.com/Articles/ASPNet-Core-Razor-Pages-Upload-Files-Save-Insert-file-to-Database-and-Download-Files.aspx
            string result = "ok";
            string fileName = Path.GetFileName(uploadfile.FileName);
            string contentType = uploadfile.ContentType;
            using (MemoryStream ms = new MemoryStream())
            {
                uploadfile.CopyTo(ms);

                UploadFile uf = new UploadFile();
                uf.Id = "";
                uf.Name = fileName;
                uf.Data = ms.ToArray();
                uf.ContentType = contentType;
                uf.GroupTypeId = GroupTypeId;
                uf.LinkedGroupId = LinkedGroupId;
                uf.FileTypeId = FileTypeId;
                uf.UploadUserId = UserId;
                uf.UploadDate = DateTime.Now.ToString();

                result = uf.Save();

            }

            return result;
        }


        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

    } 
} 
