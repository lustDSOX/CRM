//using CRM.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
// ��������� �������� ApplicationContext � �������� ������� � ����������
//builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMvc().AddRazorPagesOptions(o =>
{
    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
}); ;
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
