-- Drop the database if it exists and force disconnect any users
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'hris')
BEGIN
    ALTER DATABASE hris SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE hris;
END
GO

-- Create the database
CREATE DATABASE hris;
GO

-- Switch to the new database
USE hris;
GO

-- Create Departments table
CREATE TABLE Departments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    HeadId INT NULL
);
GO

INSERT INTO Departments (Name, Code, HeadId)
VALUES 
    ('HR', 'HR01', NULL),
    ('IT', 'IT01', NULL),
    ('Finance', 'FIN01', NULL);
GO

-- Create Employees table
CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Salary DECIMAL(10,2),
    DepartmentId INT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId)
        REFERENCES Departments(Id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);
GO


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

GO

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

GO

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
GO


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
GO

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
GO

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
GO










