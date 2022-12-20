namespace Outreach.Areas.Consoles.Pages.Content.Profile.Administrator.Users
{
    public class LoginUserInfo
    {
        public String Id;
        public String User_Id;
        public String UserName;
        public String firstName;
        public String lastName; 
        public String PhoneNumber; 
        public String Email;
        public String Password;
        public String Created_at;
        public String IsSelected; // sepecial use when project or task select member 
        public LoginUserInfo()
        {
            Id = "";
            User_Id = "";
            UserName = "";
            firstName = "";
            lastName = "";
            PhoneNumber = "";
            Email = "";
            Password = "";
            Created_at = "";
            IsSelected = "";//  "selected"
        }
    }
}
