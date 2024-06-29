# Employee Department Project Analysis

This C# console application demonstrates advanced LINQ operations to analyze and manipulate data across three related classes—Employee, Department, and Project. It showcases the use of `GroupBy`, `Join`, `Distinct`, `Where`, and `Select` to perform complex data queries and transformations.

## Setup

The solution consists of three main classes:

- `Employee`: Represents an employee with properties such as `EmployeeID`, `FirstName`, `LastName`, `Salary`, and `DepartmentID`.
- `Department`: Represents a department with properties `DepartmentID` and `DepartmentName`.
- `Project`: Represents a project with properties `ProjectID`, `ProjectName`, and `DepartmentID`.

## Features

The application includes the following methods to perform data analysis:

### 1. GroupByAndAggregate

- Groups employees by their departments.
- Calculates the average salary for each department.
- Finds the department with the highest total salary.
- Groups employees by the projects they are involved in.
- Calculates the total number of projects in each department.

### 2. PerformJoins

- Performs inner joins between the Employee, Department, and Project lists based on the relevant IDs.
- Outputs information about employees, their departments, and projects.

### 3. FilterAndSelect

- Filters employees based on specific conditions, such as employees with a salary above a certain threshold.
- Selects and displays only the `FirstName` and `LastName` of employees and the `ProjectName` of the projects they are involved in.

## Output Format

Each method outputs the results in a clear and user-friendly format, such as formatted strings or structured tables, to the console.

## Getting Started

To run this application, ensure you have a C# development environment set up, such as Visual Studio or Visual Studio Code with the .NET SDK installed. Clone the repository, navigate to the project directory, and run the application.


