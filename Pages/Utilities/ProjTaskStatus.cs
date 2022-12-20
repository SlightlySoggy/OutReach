//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;


namespace Outreach.Pages.Utilities
{
    public class ProjTaskStatus
    {
        public string Id;
        public string StatusName; 

        public ProjTaskStatus()
        {
            Id = "";
            StatusName = ""; 
        }
        public ProjTaskStatus(string ProjTaskStatusId)
        { // retrive ProjTaskStatus data by ProjTaskStatus ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (ProjTaskStatusId.Trim() != "")
                        sql = "select Id,StatusName from ProjTaskStatus with(nolock) where Id='" + ProjTaskStatusId + "' order by Id";
                    //else 
                    //    sql = "select Id,Name from ProjTaskStatus with(nolock) order by Id";

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

        public string Save(string ProjTaskStatusId) // int Id, string ProjTaskStatusName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int ProjTaskStatusTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new ProjTaskStatus into the database

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

                    if (ProjTaskStatusId=="" || ProjTaskStatusId == "0")
                    {
                        sql = "INSERT INTO ProjTaskStatus " +
                                      "(StatusName) VALUES " +
                                      "(@StatusName));" +
                                      "Select newID=MAX(id) FROM ProjTaskStatus"; 
                    }
                    else
                    {
                        sql = "Update ProjTaskStatus " +
                               "set StatusName = @StatusName "   +
                                "where id = '" + ProjTaskStatusId + "'";

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

         

        public string Delete(string ProjTaskStatusId)
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

                    String sql = "Delete ProjTaskStatus WHERE id=@id"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", ProjTaskStatusId);

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
