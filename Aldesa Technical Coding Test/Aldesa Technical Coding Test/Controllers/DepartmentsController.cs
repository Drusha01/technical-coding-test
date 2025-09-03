using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Aldesa_Technical_Coding_Test.Data;
using Aldesa_Technical_Coding_Test.Models;

namespace Aldesa_Technical_Coding_Test.Controllers
{
    [Route("[controller]")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // MVC View: /departments
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // API Endpoint: /api/departments/all
        [HttpGet]
        [Route("/api/departments/all")]
        public async Task<ActionResult<IEnumerable<DepartmentsModel>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        // API Endpoint: /api/departments
        [HttpGet]
        [Route("/api/departments")]
        public async Task<ActionResult<object>> GetDepartments(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Join Employees with Departments
            var query = from d in _context.Departments
                        join e in _context.Employees on d.HeadId equals e.Id into h
                        from head in h.DefaultIfEmpty()
                        select new
                        {
                            d.Id,
                            d.Name,
                            d.Code,
                            d.HeadId,
                            HeadName = head != null ? (head.FirstName+" "+head.LastName) : "Not Assigned"
                        };

            var totalRecords = await query.CountAsync();

            var data = await query
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
                Data = data
            };

            return Ok(result);
        }

        // API Endpoint: POST /api/departments/add
        [HttpPost]
        [Route("/api/departments/add")]
        public async Task<IActionResult> CreateEmployee([FromBody] DepartmentsModel department)
        {
            // Server-side validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(department.Name))
                errors.Add("Department Name is required.");

            if (string.IsNullOrWhiteSpace(department.Code))
                errors.Add("Department Code is required.");


            // Check if employee exists
            if(department.HeadId != null)
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == department.HeadId);
                if (!employeeExists)
                    errors.Add("Selected Employee does not exist.");
            }
            if (errors.Count > 0)
                return BadRequest(new { errors }); // return validation errors as JSON

            // Save employee
            using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("InsertDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", department.Name);
                    cmd.Parameters.AddWithValue("@Code", department.Code);
                    cmd.Parameters.AddWithValue("@HeadId", department.HeadId);

                    var result = await cmd.ExecuteScalarAsync();
                    department.Id = Convert.ToInt32(result);
                }
            }
            return Ok(department); // return the created department
        }

        // API Endpoint: GET /api/employees/{id}
        [HttpGet("{id}")]
        [Route("/api/departments/{id}")]
        public async Task<ActionResult<DepartmentsModel>> GetDepartmentById(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound(new { errors = new[] { "Department not found." } });
            }
            return Ok(department);
        }

        // API Endpoint: PUT /api/department/edit/{id}
        [HttpPut]
        [Route("/api/departments/edit/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentsModel updatedDepartment)
        {
            // Server-side validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(updatedDepartment.Name))
                errors.Add("Department Name is required.");

            if (string.IsNullOrWhiteSpace(updatedDepartment.Code))
                errors.Add("Department Code is required.");


            // Check if employee exists
            if (updatedDepartment.HeadId != null)
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == updatedDepartment.HeadId);
                if (!employeeExists)
                    errors.Add("Selected Employee does not exist.");
            }

            if (errors.Count > 0)
                return BadRequest(new { errors }); // return validation errors as JSON

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound(new { errors = new[] { "Employee not found." } });

            // Save employee
            using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("UpdateDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", updatedDepartment.Name);
                    cmd.Parameters.AddWithValue("@Code", updatedDepartment.Code);
                    cmd.Parameters.AddWithValue("@HeadId", updatedDepartment.HeadId);
                    cmd.Parameters.AddWithValue("@Id", updatedDepartment.Id);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"No employee found with Id = {updatedDepartment.Id}");
                    }
                }
            }
            return Ok(updatedDepartment);
        }

        // API Endpoint: DELETE /api/departments/delete/{id}
        [HttpDelete]
        [Route("/api/departments/delete/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            int rowsAffected = 0;

            using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("DeleteDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    // Execute stored procedure
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                        rowsAffected = Convert.ToInt32(result);
                }
            }
            if (rowsAffected == 0)
                return NotFound(new { errors = new[] { "Department not found." } });

            return Ok(new { message = "Department deleted successfully." });
        }

    }
}
