using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Outreach.Areas.Consoles.Pages.Content.Profile.Org.OrgDetails
{
    public class OrgDetailsModel : PageModel
    {
        public void OnGet()
        {
        }

        //public IActionResult OnPostUpload(FileUpload fileUpload)
        //{
        //    Creating upload folder
        //    if (!Directory.Exists(fullPath))
        //    {
        //        Directory.CreateDirectory(fullPath);
        //    }
        //    var formFile = fileUpload.FormFile;
        //    var filePath = Path.Combine(fullPath, formFile.FileName);

        //    using (var stream = System.IO.File.Create(filePath))
        //    {
        //        formFile.CopyToAsync(stream);
        //    }

        //    Process uploaded files
        //    Don't rely on or trust the FileName property without validation.
        //    ViewData["SuccessMessage"] = formFile.FileName.ToString() + " files uploaded!!";
        //    return Page();
        //}
        //public class FileUpload
        //{
        //    [Required]
        //    [Display(Name = "File")]
        //    public IFormFile FormFile { get; set; }
        //    public string SuccessMessage { get; set; }
        //}

        public async Task<IActionResult> OnPostAsync()
        {

            return Page();


        }
    }
}
