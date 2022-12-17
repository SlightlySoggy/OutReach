//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;


namespace Outreach.Pages.Utilities
{
    public class Tag
    { 
        public string Id;
        public string TagName; 
        public string StatusId;  

        public Tag()
        {
            Id = "";
            TagName = ""; 
            StatusId = "";  

        }
        public Tag(string Tag_Id)
        { // retrive Tag data by Tag ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (Tag_Id.Trim() != "")
                        sql = "Select Id,TagName,StatusId from Tag with(nolock) where Id='" + Tag_Id + "'";
                    else
                    sql = "Select Id,TagName,StatusId from Tag with(nolock) where isnull(StatusId,1) = 1 orderby TagName";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Id = reader.GetInt32(0).ToString();
                                if (reader["TagName"].GetType() != typeof(DBNull))
                                {
                                    TagName = reader["TagName"].ToString();
                                } 
                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    StatusId = reader["StatusId"].ToString();
                                }
                                 
                                //if (reader["IsDeleted"].GetType() != typeof(DBNull))
                                //{
                                //    IsDeleted = reader["IsDeleted"].ToString();
                                //}

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

        public List<Tag> GetReferencedTagsbyOpptunityId(string Opp_Id="")
        { // retrive all tags by Tag ID
            List<Tag> listTag = new List<Tag>();
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (Opp_Id.Trim() != "")
                        sql = "Select Id,TagName,StatusId from Tag with(nolock) where isnull(StatusId,1) = 1 and Id in (Select Id=TagId from TagTag  with(nolock) where TagId ='" + Opp_Id + "') order by TagName ";
                    else
                        sql = "Select Id,TagName,StatusId from Tag with(nolock) where isnull(StatusId,1) = 1 order by TagName ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tag tag = new Tag();

                                tag.Id = reader.GetInt32(0).ToString();
                                if (reader["TagName"].GetType() != typeof(DBNull))
                                {
                                    tag.TagName = reader["TagName"].ToString();
                                }
                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    tag.StatusId = reader["StatusId"].ToString();
                                }

 
                                listTag.Add(tag);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }

            return listTag;
        }


        public string Save(string tagid) // int Id, string Tagname, string Description, string Responsibility, string Requirement, int CreatedOrgId, string CreatedDate, int CreatedUserId, int StatusId, string StartDate, string EndDate, string Tags)
        {
            //save the new Tag into the database

            string result = "ok";
            string newProdID = "";
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";

                    if (tagid == "" || tagid == "0")
                    {
                        sql = "INSERT INTO Tag " +
                                      "(Tagname,Statusid) VALUES " +
                                      "(@Tagname,1);" +
                                      "Select newID=MAX(id) FROM Tag";
                    }
                    else if (Convert.ToInt32(tagid) > 0)
                    {
                        sql = "Update Tag " +
                               "set Tagname = @Tagname," +
                                   "StatusId = @StatusId " +
                                " where id = '" + tagid + "' Select newID='" + tagid + "' ";

                    }
                    else if (Convert.ToInt32(tagid) < 0)
                    { // delete Tag
                        int tid = Convert.ToInt32(tagid) * -1;
                        sql = "Update Tag " +
                               "set Tagname = @Tagname," +
                                   "StatusId = @StatusId " + 
                              " where id = '" + tid.ToString() + "' Select newID='" + tid.ToString() + "' ";

                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Tagname", this.TagName); 
                        cmd.Parameters.AddWithValue("@StatusId", Convert.ToInt32(this.StatusId)); 
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
