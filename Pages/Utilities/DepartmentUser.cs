//using Outreach.Pages.Clients;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;


namespace Outreach.Pages.Utilities
{
    public class DepartmentUser
    {
        public string Id;
        public string DepartmentId; 
        public string UserId;
        public string IsLead;

        public DepartmentUser()
        {
            Id = "";
            DepartmentId = ""; 
            UserId = "";
            IsLead = "";
        }
        public DepartmentUser(string DepartmentUserId)
        { // retrive DepartmentTask_User data by DepartmentTask_User ID
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
                    if (DepartmentUserId.Trim() != "")
                        sql = "select Id,DepartmentId,UserId,IsLead from DepartmentUser with(nolock) where Id='" + DepartmentUserId + "' order by Id";
 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())

                            {

                                Id = reader.GetInt32(0).ToString();
                                DepartmentId = reader.GetInt32(1).ToString();
                                UserId = reader.GetInt32(3).ToString();

                                if (reader["IsLead"].GetType() != typeof(DBNull))
                                {// task level user
                                    IsLead = reader["IsLead"].ToString();
                                }

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


        public string Save() // int Id, string DepartmentTask_UserName, string Description, string EstimatedBudget, string ActualSpent, int CreatedOrgId, string CreatedDate, int CreatedUserId, int DepartmentTask_UserTaskStatusId, string StartDate, string DueDate,CompletionDate, string Tags)
        {
            //save the new DepartmentTask_User into the database 

            string result = "ok";
            int newProdID = 0;
            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO DepartmentUser " +
                                  "(DepartmentId,  UserId,IsLead) VALUES " +
                                  "(@DepartmentId,  @UserId,@IsLead);" +
                                  "Select newID=MAX(id) FROM DepartmentUser"; 

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", this.UserId); 
                        cmd.Parameters.AddWithValue("@DepartmentId", this.DepartmentId);

                        if (this.IsLead != "")
                        { // task levle user
                            cmd.Parameters.AddWithValue("@IsLead", this.IsLead);
                        }
                        else
                            cmd.Parameters.AddWithValue("@IsLead", DBNull.Value);
                         
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

         

        public string Delete(string DepartmentTask_UserId)
        { 
            //we should not call this method especially when status is reference by other Department or task

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Delete DepartmentUser WHERE id=@id"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", DepartmentTask_UserId);

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
