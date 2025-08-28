using Aldesa_Technical_Coding_Test.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldesa_Technical_Coding_Test.Models
{
    public class EmployeesModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Salary { get; set; }

        // Foreign Key
        public int? DepartmentId { get; set; }
        public DepartmentsModel departmentsModel { get; set; }

        // Convenience property for API response
        [NotMapped]
        public string DepartmentName => departmentsModel != null ? departmentsModel.Name : "N/A";

    }
}
