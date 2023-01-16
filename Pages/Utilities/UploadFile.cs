//using Outreach.Pages.Clients;
using Microsoft.CodeAnalysis;
using NuGet.ProjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class UploadFile
    {
        public string Id;
        public string Name;
        public byte[] Data;
        public string ContentType;
        public string FileTypeId; //1: Logo,2:
        public string GroupTypeId;  //1:Organization,2:Team,3:Project,4:Task
        public string LinkedGroupId; //value can be: UploadDate, TeamId, Data, TaskId 
        public string UploadUserId;
        public string UploadDate; 
        public string TeamId;
        public string LinkTo; //Name in GroupType table

        public UploadFile()
        { 
            Id = "";
            Name = "";
            Data = new byte[0];
            ContentType = "";
            FileTypeId = "";
            GroupTypeId = "";
            LinkedGroupId = "";
            UploadUserId = "";
            UploadDate = ""; 
            TeamId = "";
            LinkTo = "";
        }
        public UploadFile(string id)
        { // retrive TeamTask_User data by TeamTask_User ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (id.Trim() != "")

                        sql = " Select ul.Id, ul.Name, ul.Data, ul.ContentType, ul.FileTypeId, ul.GroupTypeId, ul.LinkedGroupId, ul.UploadUserId, ul.UploadDate, LinkTo=gt.name " +
                            " from UploadFile ul with(nolock) " +
                            " left join GroupType gt on gt.Id = ul.GroupTypeId " +
                            " where ul.Id='" + id + "'  ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            Id = id;

                            if (reader["Name"].GetType() != typeof(DBNull))
                            {// Organization level task
                                Name = reader["Name"].ToString();
                            }

                            if (reader["Data"].GetType() != typeof(DBNull))
                            {// Organization level task
                                Data = (byte[])reader["Data"];
                            }

                            if (reader["FileTypeId"].GetType() != typeof(DBNull))
                            {// Organization level task
                                FileTypeId = reader["FileTypeId"].ToString();
                            }

                            if (reader["GroupTypeId"].GetType() != typeof(DBNull))
                            {// Organization level task
                                GroupTypeId = reader["GroupTypeId"].ToString();
                            }
                            if (reader["GroupTypeId"].GetType() != typeof(DBNull))
                            {// Organization level task
                                GroupTypeId = reader["GroupTypeId"].ToString();
                            }

                            if (reader["LinkedGroupId"].GetType() != typeof(DBNull))
                            {// Organization level task
                                LinkedGroupId = reader["LinkedGroupId"].ToString();
                            }

                            if (reader["UploadUserId"].GetType() != typeof(DBNull))
                            {// Organization level task
                                UploadUserId = reader["UploadUserId"].ToString();
                            }

                            if (reader["ContentType"].GetType() != typeof(DBNull))
                            {// Organization level task
                                ContentType = reader["ContentType"].ToString();
                            }

                            if (reader["UploadDate"].GetType() != typeof(DBNull))
                            {// Organization level task
                                UploadDate = reader["UploadDate"].ToString();
                            }

                            if (reader["LinkTo"].GetType() != typeof(DBNull))
                            {// Organization level task
                                LinkTo = reader["LinkTo"].ToString();
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


        public string UpdateUploadFile()
        {
            string result = "ok";

            return result;

        }


        public string Save()
        {
            //save the new UploadFile into the database, One task can only link to one thing: Org, team or project
            // https://www.aspsnippets.com/Articles/ASPNet-Core-Razor-Pages-Upload-Files-Save-Insert-file-to-Database-and-Download-Files.aspx 

            string result = ""; // will return fileID or "Failed" message
            int newID = 0;
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
                        sql = "INSERT INTO UploadFile " +
                                      "(Name,Data,ContentType,FileTypeId,GroupTypeId,LinkedGroupId,UploadUserId,UploadDate) VALUES " +
                                      "(@Name,@Data,@ContentType,@FileTypeId,@GroupTypeId,@LinkedGroupId,@UploadUserId,@UploadDate);" +
                                      "Select newID=MAX(id) FROM UploadFile";
                    }
                    else
                    {
                        sql = "Update UploadFile " +
                               "set Name = @Name," +
                                   "Data = @Data," +
                                   "ContentType = @ContentType," +
                                   "FileTypeId = @FileTypeId," +
                                   "GroupTypeId = @GroupTypeId," +
                                   "LinkedGroupId = @LinkedGroupId," +
                                   "UploadUserId = @UploadUserId," +
                                   "UploadDate = @UploadDate " +
                                " where id = '" + this.Id + "' ; Select newID=" + this.Id + "";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", this.Name);
                        cmd.Parameters.AddWithValue("@Data", this.Data);
                        cmd.Parameters.AddWithValue("@ContentType", this.ContentType);
                        cmd.Parameters.AddWithValue("@FileTypeId", this.FileTypeId);
                        cmd.Parameters.AddWithValue("@GroupTypeId", this.GroupTypeId);
                        cmd.Parameters.AddWithValue("@LinkedGroupId", this.LinkedGroupId);
                        cmd.Parameters.AddWithValue("@UploadUserId", this.UploadUserId);
                        cmd.Parameters.AddWithValue("@UploadDate", this.UploadDate);
                        //cmd.ExecuteNonQuery();
                        newID = (Int32)cmd.ExecuteScalar();
                        Id = newID.ToString();
                        result = Id;
                    }
                }
            }
            catch (Exception ex)
            {
                result = "failed" + ex.Message;
            }
            return result;
        }



        public string Delete()
        {
            //we should not call this method especially when status is reference by other project or task

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Delete UploadFile WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", this.Id);

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


 