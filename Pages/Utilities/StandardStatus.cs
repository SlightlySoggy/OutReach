//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;


namespace Outreach.Pages.Utilities
{
    public class StandardStatus
    {
        public string Id;
        public string StatusName; 

        public StandardStatus()
        {
            Id = "";
            StatusName = ""; 
        }
        public StandardStatus(string StandardStatusId)
        { // retrive StandardStatus data by StandardStatus ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (StandardStatusId.Trim() != "")
                        sql = "select Id,StatusName from StandardStatus with(nolock) where Id='" + StandardStatusId + "' order by Id";
                    //else 
                    //    sql = "select Id,Name from StandardStatus with(nolock) order by Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                if (reader["StatusName"].GetType() != typeof(DBNull))
                                {
                                    StatusName = reader["StatusName"].ToString();
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

        public string Save(string StandardStatusId) // int Id, string StandardStatusName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int StandardStatusTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new StandardStatus into the database

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

                    if (StandardStatusId=="" || StandardStatusId == "0")
                    {
                        sql = "INSERT INTO StandardStatus " +
                                      "(StatusName) VALUES " +
                                      "(@StatusName));" +
                                      "Select newID=MAX(id) FROM StandardStatus"; 
                    }
                    else
                    {
                        sql = "Update StandardStatus " +
                               "set StatusName = @StatusName "   +
                                "where id = '" + StandardStatusId + "'";

                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@StatusName", this.StatusName); 
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

         

        public string Delete(string StandardStatusId)
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

                    String sql = "Delete StandardStatus WHERE id=@id"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", StandardStatusId);

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
