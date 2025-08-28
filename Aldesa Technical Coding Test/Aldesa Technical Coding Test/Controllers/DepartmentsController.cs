using Microsoft.AspNetCore.Mvc;
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

        // MVC View: /employees
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // API Endpoint: /api/departments
        [HttpGet]
        [Route("/api/departments")]
        public async Task<ActionResult<IEnumerable<DepartmentsModel>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }
    }
}
