using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private static CrmRazorContext db = Manager.db;
        private static List<User> Users = db.Users.ToList();
        public IndexModel() { }

        public void OnGet()
        {
            Users = db.Users.ToList();
        }
        public IActionResult OnPost(string _email, string _password)
        {
            foreach (var user in Users)
            {
                if (((_email == user.Login) || (_email == user.Name)) && (_password == user.Password))
                {
                    return RedirectToPage("/MainPage", new { user_id = user.UserId });
                }
            }
            return Page();
        }
    }
}