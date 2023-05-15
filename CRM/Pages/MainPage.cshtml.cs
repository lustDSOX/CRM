using CRM.Classes;
using CRM.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Pages
{
    public class MainPageModel : PageModel
    {
        CrmRazorContext db = Manager.db;
        public string username;
        public string avatar;
        public int id;
        public MainPageModel()
        {
            Manager.GetTickets1Min();
        }
        public void OnGet(string user_id)
        {
            Manager.currentUser = db.Users.Find(Int32.Parse(user_id));
            username = Manager.currentUser.Name;
            avatar = Manager.currentUser.AvatarUrl;
            id = Manager.currentUser.UserId;
        }
    }
}
