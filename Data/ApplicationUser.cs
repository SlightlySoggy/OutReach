using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace Outreach.Data
{ 
    public class ApplicationUser : IdentityUser
    { // lizhi added it manually to extend the default IdentityUser class  with three new columns on AspNetUsers table
      // https://www.tektutorialshub.com/asp-net-core/add-custom-fields-to-user-in-asp-net-core-identity/


        // beware can not add null value field here: SqlNullValueException: Data is Null.This method or property cannot be called on Null values.

        //public int User_Id { get; set; } // when have set here, it can return the value from datbase, but it fail registration.
        public int User_Id { get; } // User_Id is set automatically in database [User_Id] [int] IDENTITY(1,1) NOT NULL


        //private string _PhoneNumber = "";  // the PhoneNumber field
        //public string PhoneNumber    // the PhoneNumber property
        //{
        //    //get => _PhoneNumber;
        //    //set => _PhoneNumber = (if value is null) ;

        //    get
        //    {
        //        if (_PhoneNumber != null)
        //            return _PhoneNumber;
        //        else 
        //        return "";
        //    }
        //    set
        //    { // doesnot work
        //        if (value != null)
        //            _PhoneNumber = value;
        //        else
        //            _PhoneNumber = "";
        //    }

        //}

        public string firstName { get; set; }
        public string lastName { get; set; } 

    }
}