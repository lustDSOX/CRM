using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static CRM.Models.CrmRazorContext;
using CRM.Models.Db_classes;
namespace CRM.Pages
{
    public class IndexModel : PageModel
    {
        CrmRazorContext db = new CrmRazorContext();
        List<User> Users = new List<User>();

        public IndexModel()
        {
        }

        public void OnGet()
        {
            Users = db.Users.ToList();
        }
    }
}