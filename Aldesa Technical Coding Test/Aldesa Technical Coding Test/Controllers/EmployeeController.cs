using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aldesa_Technical_Coding_Test.Data;
using Aldesa_Technical_Coding_Test.Models;

namespace Aldesa_Technical_Coding_Test.Controllers
{
    [Route("[controller]")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // MVC View: /employees
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // API Endpoint: /api/employees
        [HttpGet]
        [Route("/api/employees")]
        public async Task<ActionResult<object>> GetEmployees(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Join Employees with Departments
            var query = from e in _context.Employees
                        join d in _context.Departments on e.DepartmentId equals d.Id into ed
                        from dept in ed.DefaultIfEmpty()
                        select new
                        {
                            e.Id,
                            e.FirstName,
                            e.LastName,
                            e.Salary,
                            DepartmentName = dept != null ? dept.Name : "N/A"
                        };

            var totalRecords = await query.CountAsync();

            var employees = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = employees
            };

            return Ok(result);
        }



        // API Endpoint: GET /api/employees/by-department?department=2
        [HttpGet]
        [Route("/api/employees/by-department")]
        public async Task<ActionResult> GetEmployeesByDepartment(int department, int pageNumber = 1, int pageSize = 5)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 5;

            // Check if the department exists
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == department);
            if (!departmentExists)
            {
                return NotFound(new { message = "Department not found." });
            }

            // Join Employees with Departments
            var query = from e in _context.Employees
                        join d in _context.Departments on e.DepartmentId equals d.Id into ed
                        from dept in ed.DefaultIfEmpty()
                        where e.DepartmentId == department
                        select new
                        {
                            e.Id,
                            e.FirstName,
                            e.LastName,
                            e.Salary,
                            DepartmentName = dept != null ? dept.Name : "N/A"
                        };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var employees = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                data = employees,
                pageNumber,
                pageSize,
                totalPages,
                totalCount
            });
        }



        // API Endpoint: GET /api/employees/{id}
        [HttpGet("{id}")]
        [Route("/api/employees/{id}")]
        public async Task<ActionResult<EmployeesModel>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound(new { errors = new[] { "Employee not found." } });
            }

            return Ok(employee);
        }



        // API Endpoint: POST /api/employees/add
        [HttpPost]
        [Route("/api/employees/add")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeesModel employee)
        {
            // Server-side validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(employee.FirstName))
                errors.Add("First Name is required.");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                errors.Add("Last Name is required.");

            if (employee.DepartmentId == 0)
                errors.Add("Please select a Department.");

            if (employee.Salary <= 0)
                errors.Add("Salary must be greater than 0.");

            // Check if department exists
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == employee.DepartmentId);
            if (!departmentExists)
                errors.Add("Selected Department does not exist.");

            if (errors.Count > 0)
                return BadRequest(new { errors }); // return validation errors as JSON

            // Save employee
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(employee); // return the created employee
        }

        // API Endpoint: PUT /api/employees/edit/{id}
        [HttpPut]
        [Route("/api/employees/edit/{id}")]
        public async Task<IActionResult> EditEmployee(int id, [FromBody] EmployeesModel updatedEmployee)
        {
            if (id != updatedEmployee.Id)
                return BadRequest(new { errors = new[] { "Employee ID mismatch." } });

            // Server-side validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(updatedEmployee.FirstName))
                errors.Add("First Name is required.");

            if (string.IsNullOrWhiteSpace(updatedEmployee.LastName))
                errors.Add("Last Name is required.");

            if (updatedEmployee.DepartmentId == 0)
                errors.Add("Please select a Department.");

            if (updatedEmployee.Salary <= 0)
                errors.Add("Salary must be greater than 0.");

            // Check if department exists
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == updatedEmployee.DepartmentId);
            if (!departmentExists)
                errors.Add("Selected Department does not exist.");

            if (errors.Count > 0)
                return BadRequest(new { errors });

            // Find existing employee
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { errors = new[] { "Employee not found." } });

            // Update properties
            employee.FirstName = updatedEmployee.FirstName;
            employee.LastName = updatedEmployee.LastName;
            employee.DepartmentId = updatedEmployee.DepartmentId;
            employee.Salary = updatedEmployee.Salary;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }


        // API Endpoint: DELETE /api/employees/delete/{id}
        [HttpDelete]
        [Route("/api/employees/delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { errors = new[] { "Employee not found." } });

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee deleted successfully." });
        }


    }
}
