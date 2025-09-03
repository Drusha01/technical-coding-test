using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldesa_Technical_Coding_Test.Migrations
{
    /// <inheritdoc />
    public partial class CreateDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Employees_HeadId",
                        column: x => x.HeadId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadId",
                table: "Departments",
                column: "HeadId",
                unique: true,
                filter: "[HeadId] IS NOT NULL");


            migrationBuilder.Sql(@"
               CREATE PROCEDURE InsertDepartment
                    @Name NVARCHAR(100),
                    @Code NVARCHAR(50),
                    @HeadId INT = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO Departments (Name, Code, HeadId)
                    VALUES (@Name, @Code, @HeadId);

                    -- return new ID
                    SELECT SCOPE_IDENTITY() AS NewId;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateDepartment
                    @Id INT,
                    @Name NVARCHAR(100),
                    @Code NVARCHAR(50),
                    @HeadId INT = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE Departments
                    SET Name = @Name,
                        Code = @Code,
                        HeadId = @HeadId
                    WHERE Id = @Id;

                    -- return updated row
                    SELECT Id, Name, Code, HeadId
                    FROM Departments
                    WHERE Id = @Id;
                END 
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteDepartment
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DELETE FROM Departments
                    WHERE Id = @Id;

                    -- tell how many rows were deleted
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
