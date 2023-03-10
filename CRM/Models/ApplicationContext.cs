using Microsoft.EntityFrameworkCore;

namespace CRM.Models
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated(); 
        }
    }
}
