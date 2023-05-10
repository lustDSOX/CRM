using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRM.Pages
{
    public class UsersModel : PageModel
    {
        private static CrmRazorContext db = Manager.db;
        static User user = new User();
        public List<Role> roles = db.Roles.ToList();
        static bool is_new = false;

        public void OnGet()
        {
        }
        public IActionResult OnGetGetData()
        {
            var data = db.Users.ToList();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var json = JsonSerializer.Serialize(data, options);
            return new JsonResult(json);
        }


        public IActionResult OnPostPutData(string name,string login,string password, string role,IFormFile avatar,bool working)
        {
            lock (user)
            {
                user.Name = name;
                user.Login = login;
                user.Password = password;
                user.Role = db.Roles.FirstOrDefault(x => x.Name == role).RoleId;
                if (avatar != null)
                {
                    var fileExtension = Path.GetExtension(avatar.FileName);
                    string avatar_name = "";
                    if (is_new)
                    {
                        avatar_name = (db.Users.Count() + 1).ToString();

                    }
                    else
                    {
                        avatar_name = user.UserId.ToString();
                    }
                    avatar_name = Path.ChangeExtension(avatar_name, fileExtension);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/avatars", avatar_name);
                    // Сохранение файла на сервере
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        avatar.CopyTo(stream);
                    }
                    user.AvatarUrl = "/images/avatars/" + avatar_name;
                }
                else
                {
                    user.AvatarUrl = "/images/base_avatar.svg";
                }
                if (is_new)
                    db.Users.Update(user);
                db.SaveChanges();
            }
            return new OkResult();
        }

        public IActionResult OnGetDeleteData()
        {
            lock (user)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.AvatarUrl);
                System.IO.File.Delete(path);
                db.Users.Remove(user);
                db.SaveChanges();
            }
            return new OkResult();
        }

        public IActionResult OnGetSetData(int id)
        {
            lock (user)
            {
                if (id == -1)
                {
                    user = new User();
                    is_new = true;
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                is_new = false;
                user = db.Users.FirstOrDefault(x => x.UserId == id);
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var json = JsonSerializer.Serialize(user, options);
            return new JsonResult(json);
        }
    }
}
