using Aldesa_Technical_Coding_Test.Models;

namespace Aldesa_Technical_Coding_Test.Models
{
    public class DepartmentsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        // Foreign key to Employee (Head of Department)
        public int? HeadId { get; set; }
        public EmployeesModel Head { get; set; }

    }
}
