using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldesa_Technical_Coding_Test.Migrations
{
    /// <inheritdoc />
    public partial class CreateEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });


            migrationBuilder.Sql(@"
                CREATE PROCEDURE InsertEmployee
                    @FirstName NVARCHAR(50),
                    @LastName NVARCHAR(50),
                    @DepartmentId INT,
                    @Salary DECIMAL(10,2)
                AS
                BEGIN
                    INSERT INTO Employees (FirstName, LastName, DepartmentId, Salary)
                    VALUES (@FirstName, @LastName, @DepartmentId, @Salary);
                    SELECT SCOPE_IDENTITY() AS NewId;
                END
            ");


            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateEmployee
                    @EmployeeId INT,
                    @FirstName NVARCHAR(50),
                    @LastName NVARCHAR(50),
                    @DepartmentId INT,
                    @Salary DECIMAL(18,2)
                AS
                BEGIN
                    SET NOCOUNT ON;  
                    UPDATE Employees
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        DepartmentId = @DepartmentId,
                        Salary = @Salary
                    WHERE Id = @EmployeeId;
                END
            ");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteEmployee
                    @EmployeeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DELETE FROM Employees
                    WHERE Id = @EmployeeId;

                    -- return number of rows affected
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "Employees");
        }
    }
}
