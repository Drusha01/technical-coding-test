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
