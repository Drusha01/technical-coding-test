# Technical Coding Test for Software Developer Position

## Overview
This test evaluates your skills in **C#, ASP.NET MVC, SQL Server, HTML, and JavaScript**. You will build a simple web application to manage a list of employees, including backend and frontend components. The test should take approximately **2-3 hours** to complete. Please submit your solution as a zipped project folder, including all source code, database scripts, and a brief README explaining how to run the application.

## Requirements
Create a web application with the following features:
1. **Backend**:
   - Use **ASP.NET MVC** (C#) to create a web application.
   - Implement a **RESTful API** endpoint to retrieve a list of employees.
   - Use **SQL Server** to store employee data (fields: Id, FirstName, LastName, Department, Salary).
   - Include a **stored procedure** to fetch employees by department.
2. **Frontend**:
   - Create a webpage using **HTML** and **JavaScript** to display the list of employees in a table.
   - Use **AJAX** (via JavaScript) to fetch employee data from the API and dynamically populate the table.
   - Add a dropdown to filter employees by department, calling the stored procedure via the API.
3. **Constraints**:
   - Use Entity Framework for database operations.
   - Ensure the application handles basic error cases (e.g., no employees found).
   - Apply basic styling to the table using **CSS** (can be inline or separate file).

## Tasks
1. **Database Setup**:
   - Create a SQL Server database with an `Employees` table.
   - Write a stored procedure `GetEmployeesByDepartment` that accepts a department name and returns matching employees.
2. **Backend Development**:
   - Set up an ASP.NET MVC project.
   - Create a model for `Employee`.
   - Implement a controller with an API endpoint (`/api/employees`) to return all employees.
   - Implement another endpoint (`/api/employees/by-department`) that calls the stored procedure.
3. **Frontend Development**:
   - Create an HTML page with a table to display employee data (Id, FirstName, LastName, Department, Salary).
   - Use JavaScript (AJAX) to fetch data from the `/api/employees` endpoint and populate the table.
   - Add a dropdown with department options (e.g., HR, IT, Finance) that triggers an AJAX call to `/api/employees/by-department` when selected.
4. **Bonus (Optional)**:
   - Add client-side validation to ensure a department is selected before filtering.
   - Implement basic error handling on the frontend (e.g., display a message if no employees are found).

## Submission Instructions
- Provide the complete solution as a zipped folder.
- Include a `README.md` with:
  - Instructions to set up and run the application.
  - Any assumptions or design decisions made.
  - Notes on the bonus tasks (if attempted).
- Ensure the solution is runnable on a standard development environment (e.g., Visual Studio, SQL Server Express).

## Evaluation Criteria
- **Correctness**: Does the application meet all functional requirements?
- **Code Quality**: Is the code well-organized, readable, and maintainable?
- **Error Handling**: Are edge cases and errors handled appropriately?
- **Adherence to Technologies**: Proper use of C#, MVC, SQL Server, HTML, and JavaScript.
- **Bonus Tasks**: Extra points for implementing optional features.

## Sample Code Structure
Below is a suggested structure for your solution. You may adapt it as needed.

### SQL (Employees Table and Stored Procedure)
```sql
CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Department NVARCHAR(50),
    Salary DECIMAL(10,2)
);

CREATE PROCEDURE GetEmployeesByDepartment
    @Department NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Employees WHERE Department = @Department;
END;
```

### C# (Employee Model)
```csharp
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
}
```

### C# (Controller Example)
```csharp
public class EmployeesController : Controller
{
    private readonly YourDbContext _context;

    public EmployeesController(YourDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/employees")]
    public IActionResult GetEmployees()
    {
        var employees = _context.Employees.ToList();
        return Ok(employees);
    }

    [HttpGet]
    [Route("api/employees/by-department")]
    public IActionResult GetEmployeesByDepartment(string department)
    {
        var employees = _context.Employees
            .FromSqlRaw("EXEC GetEmployeesByDepartment @Department", 
                new SqlParameter("@Department", department))
            .ToList();
        return Ok(employees);
    }
}
```

### HTML/JavaScript (Frontend)
```html
<!DOCTYPE html>
<html>
<head>
    <title>Employee Management</title>
    <style>
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid black; padding: 8px; text-align: left; }
    </style>
</head>
<body>
    <select id="departmentFilter">
        <option value="">Select Department</option>
        <option value="HR">HR</option>
        <option value="IT">IT</option>
        <option value="Finance">Finance</option>
    </select>
    <table id="employeeTable">
        <thead>
            <tr>
                <th>ID</th>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Department</th>
                <th>Salary</th>
            </tr>
        </thead>
        <tbody></tbody>
    </table>

    <script>
        function loadEmployees() {
            fetch('/api/employees')
                .then(response => response.json())
                .then(data => {
                    const tbody = document.querySelector('#employeeTable tbody');
                    tbody.innerHTML = '';
                    data.forEach(employee => {
                        const row = `<tr>
                            <td>${employee.id}</td>
                            <td>${employee.firstName}</td>
                            <td>${employee.lastName}</td>
                            <td>${employee.department}</td>
                            <td>${employee.salary}</td>
                        </tr>`;
                        tbody.innerHTML += row;
                    });
                });
        }

        document.getElementById('departmentFilter').addEventListener('change', function() {
            const department = this.value;
            if (department) {
                fetch(`/api/employees/by-department?department=${department}`)
                    .then(response => response.json())
                    .then(data => {
                        const tbody = document.querySelector('#employeeTable tbody');
                        tbody.innerHTML = '';
                        data.forEach(employee => {
                            const row = `<tr>
                                <td>${employee.id}</td>
                                <td>${employee.firstName}</td>
                                <td>${employee.lastName}</td>
                                <td>${employee.department}</td>
                                <td>${employee.salary}</td>
                            </tr>`;
                            tbody.innerHTML += row;
                        });
                    });
            } else {
                loadEmployees();
            }
        });

        // Load employees on page load
        loadEmployees();
    </script>
</body>
</html>
```

## Notes
- Populate the `Employees` table with at least 5 sample records for testing.
- Ensure the application is secure (e.g., validate inputs to prevent SQL injection).
- Document any additional features or optimizations in your README.

Good luck!
