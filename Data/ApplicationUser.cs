using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Outreach.Data
{ 
    public class ApplicationUser : IdentityUser
    { // lizhi added it manually to extend the default IdentityUser class  with three new columns on AspNetUsers table
        // https://www.tektutorialshub.com/asp-net-core/add-custom-fields-to-user-in-asp-net-core-identity/
        public string User_Id { get; }  // User_Id is set [User_Id] [int] IDENTITY(1,1) NOT NULL
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}