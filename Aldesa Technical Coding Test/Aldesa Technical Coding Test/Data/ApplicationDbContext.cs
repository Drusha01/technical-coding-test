using Microsoft.EntityFrameworkCore;
using Aldesa_Technical_Coding_Test.Models;

namespace Aldesa_Technical_Coding_Test.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSets for your models
        public DbSet<EmployeesModel> Employees { get; set; }
        public DbSet<DepartmentsModel> Departments { get; set; }
    }
}
