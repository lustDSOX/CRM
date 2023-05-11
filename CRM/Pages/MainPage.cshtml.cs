using CRM.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Pages
{
    public class MainPageModel : PageModel
    {
        static User current_user = null;
        private static CrmRazorContext db = Manager.db;
        private static List<User> Users = db.Users.ToList();
        public string username;
        public MainPageModel()
        {
            Manager.GetTickets1Min();
        }
        public void OnGet(string user_id)
        {
            //foreach (var user in Users)
            //{
            //    if (user.UserId == int.Parse(user_id))
            //    {
            //        current_user = user;
            //        username = user.Name;
            //    }
            //}
        }
    }
}
