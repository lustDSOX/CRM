using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static CRM.Models.CrmRazorContext;
using CRM.Models.Db_classes;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CRM.Pages
{
	[IgnoreAntiforgeryToken]
	public class IndexModel : PageModel
	{
		private static CrmRazorContext db = new CrmRazorContext();
		private static List<User> Users = db.Users.ToList();
		public IndexModel(){ }

		public void OnGet()
		{
			Users = db.Users.ToList();
		}
		public IActionResult OnPost(string _email, string _password)
		{
			foreach (var user in Users)
			{
				if (((_email == user.Email) || (_email == user.Name)) && (_password == user.Password))
				{
					return RedirectToPage("/MainPage", new {user_id = user.UserId });
				}				
			}
			return Page();
		}
	}
}