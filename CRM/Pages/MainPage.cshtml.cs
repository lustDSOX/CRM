using CRM.Classes;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CRM.Pages
{
    public class MainPageModel : PageModel
    {
        public string username;
        public string avatar;
        public int id;
        public MainPageModel()
        {
            Manager.GetTickets1Min();
        }
        public IActionResult OnGetUser(int id)
        {
            using (var dbContext = new CrmRazorContext())
            {
                User user = dbContext.Users.AsNoTracking().FirstOrDefault(x => x.UserId == id);
                if (user != null)
                {
                    var result = new { user.Name, user.AvatarUrl };
                    string json = JsonConvert.SerializeObject(result);
                    return new JsonResult(json);
                }
                return new NotFoundResult();
            }
        }
        public void OnGet(string user_id)
        {
            using (var dbContext = new CrmRazorContext())
            {
                Manager.currentUser = dbContext.Users.Find(Int32.Parse(user_id));
                username = Manager.currentUser.Name;
                avatar = Manager.currentUser.AvatarUrl;
                id = Manager.currentUser.UserId;
            }
        }
    }
}
