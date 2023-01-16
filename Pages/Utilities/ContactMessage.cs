//using Outreach.Pages.Clients;
using System.Data;
using System.Data.SqlClient; 


namespace Outreach.Pages.Utilities
{
    public class ContactMessage // Create class properties (This file can be referenced across the solution by typing "using ContactMessage = Outreach.Pages.Utilities.ContactMessage;")
    {
        public string Id; 
        public string Name;
        public string Email; 
        public string CreateDate;
        public string UserId;
        public string Subject;
        public string Message; 
        public string StatusId;
        public string Status;  

        public ContactMessage()
        {
            Id = ""; 
            Name = "";
            Email = ""; 
            CreateDate = "";
            UserId = ""; 
            Subject = "";
            Message = ""; 
            StatusId = "1";//default status is new
            Status = ""; // the status name 

        }
        public ContactMessage(string contactMessageId)
        { // retrive ContactMessage data by ContactMessage ID
            try
            {
                GeneralUtilities ut = new GeneralUtilities();
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "";
//                    if (contactMessageId.Trim() != "")
                    sql = "select t.Id,t.Name,t.Email,t.CreateDate,t.UserId,t.Subject,t.Message,t.StatusId,Status=S.StatusName from ContactMessage t with(nolock) left join StandardStatus2 S on S.Id=t.StatusId where t.Id='" + contactMessageId + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            { 
                                Id = reader.GetInt32(0).ToString();
                                if (reader["Id"].GetType() != typeof(DBNull))
                                {
                                    Id = reader["Id"].ToString();
                                }

                                if (reader["Name"].GetType() != typeof(DBNull))
                                {
                                    Name = reader["Name"].ToString();
                                }

                                if (reader["Email"].GetType() != typeof(DBNull))
                                {
                                    Email = reader["Email"].ToString();
                                }

                                if (reader["CreateDate"].GetType() != typeof(DBNull))
                                {
                                    CreateDate = reader["CreateDate"].ToString();
                                }

                                if (reader["UserId"].GetType() != typeof(DBNull))
                                {
                                    UserId = reader["UserId"].ToString();
                                }

                                if (reader["Subject"].GetType() != typeof(DBNull))
                                {
                                    Subject = ut.EmptyDateConvert(reader["Subject"].ToString());
                                }

                                if (reader["Message"].GetType() != typeof(DBNull))
                                {
                                    Message = ut.EmptyDateConvert(reader["Message"].ToString());
                                }
                                 

                                if (reader["StatusId"].GetType() != typeof(DBNull))
                                {
                                    StatusId = reader["StatusId"].ToString();
                                }

                                if (reader["Status"].GetType() != typeof(DBNull))
                                {
                                    Status = reader["Status"].ToString();
                                } 


                                //contactMessageLinkage = new contactMessageLinkage(contactMessageId);  

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

        public string Save() // int Id, string Name, string Email, string EstimatedBudget, string ActualSpent, int OrganizationId, string CreateDate, int UserId, int StatusId, string Subject, string Message,CompletionDate, string Tags)
        {
            //save the new ContactMessage into the database

            string result = "ok";
            int newcontactMessageID = 0;
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
                        sql = "INSERT INTO ContactMessage " +
                                      "(Name,Email,CreateDate,UserId,Subject,Message,StatusId) VALUES " +
                                      "(@Name,@Email,@CreateDate,@UserId,@Subject,@Message,@StatusId);" +
                                      "Select newID=MAX(id) FROM ContactMessage"; 
                    }
                    else
                    {
                        sql = "Update ContactMessage " +
                               "set Name = @Name," +
                                   "Email = @Email," + 
                                   "CreateDate = @CreateDate," +
                                   "UserId = @UserId," + 
                                   "Subject = @Subject," +
                                   "Message = @Message," +
                                   "StatusId = @StatusId " + 
                                " where id = '" + this.Id + "' ; Select newID=" + this.Id + "";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    { 
                        cmd.Parameters.AddWithValue("@Name", this.Name);
                        cmd.Parameters.AddWithValue("@Email", this.Email); 
                        cmd.Parameters.AddWithValue("@CreateDate", this.CreateDate);
                        cmd.Parameters.AddWithValue("@UserId", this.UserId); 
                        cmd.Parameters.AddWithValue("@Subject", this.Subject);
                        cmd.Parameters.AddWithValue("@Message", this.Message); 
                        cmd.Parameters.AddWithValue("@StatusId", this.StatusId);
                        //cmd.ExecuteNonQuery();
                        newcontactMessageID = (Int32)cmd.ExecuteScalar();
                         
                        //if (this.Id == "" && newcontactMessageID != 0)
                        //{
                        //    this.contactMessageLinkage.contactMessageId = newcontactMessageID.ToString();
                        //    this.contactMessageLinkage.Save();
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                result = "failed" + ex.Message;
            }
            return result;
        }

         

        public string Delete(string contactMessageId) // int Id, string Name, string Email, string EstimatedBudget, string ActualSpent, int OrganizationId, string CreateDate, int UserId, int StatusId, string Subject, string Message,CompletionDate, string Tags)
        {
            //save the new ContactMessage into the database

            string result = "ok";

            try
            {
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("MyAffDBConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "Update ContactMessage Set StatusId=4 WHERE id=@id";  //StandardStatus2 table
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", contactMessageId);

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
