using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace employee_department_project_analysis
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Declaring three variables (employees, departments, projects) and assigning them the values returned by the DataInitialization method
            var (employees, departments, projects) = DataInitialization();

            GroupByAndAggregate(employees, departments, projects);
            PerformJoins(employees, departments, projects);
            FilterAndSelect(employees, projects, 80000);
        }

        static (List<Employee>, List<Department>, List<Project>) DataInitialization()
        {
            List<Employee> employees = new List<Employee>
            {
                // Creating a new instance of the Employee class with specified property values
                new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe", Salary = 70000, DepartmentID = 1 },
                new Employee { EmployeeID = 2, FirstName = "Jane", LastName = "Doe", Salary = 80000, DepartmentID = 1 },
                new Employee { EmployeeID = 3, FirstName = "Jim", LastName = "Beam", Salary = 60000, DepartmentID = 2 },
                new Employee { EmployeeID = 4, FirstName = "Jack", LastName = "Daniels", Salary = 90000, DepartmentID = 2 },
                new Employee { EmployeeID = 5, FirstName = "Jill", LastName = "Valentine", Salary = 75000, DepartmentID = 3 },
                new Employee { EmployeeID = 6, FirstName = "Chris", LastName = "Redfield", Salary = 85000, DepartmentID = 3 },
                new Employee { EmployeeID = 7, FirstName = "Leon", LastName = "Kennedy", Salary = 95000, DepartmentID = 1 },
                new Employee { EmployeeID = 8, FirstName = "Claire", LastName = "Redfield", Salary = 65000, DepartmentID = 2 },
                new Employee { EmployeeID = 9, FirstName = "Ada", LastName = "Wong", Salary = 99000, DepartmentID = 3 },
                new Employee { EmployeeID = 10, FirstName = "Albert", LastName = "Wesker", Salary = 105000, DepartmentID = 1 }
            };

            List<Department> departments = new List<Department>
            {
                new Department { DepartmentID = 1, DepartmentName = "IT" },
                new Department { DepartmentID = 2, DepartmentName = "HR" },
                new Department { DepartmentID = 3, DepartmentName = "Finance" }
            };

            List<Project> projects = new List<Project>
            {
                new Project { ProjectID = 1, ProjectName = "Project A", DepartmentID = 1 },
                new Project { ProjectID = 2, ProjectName = "Project B", DepartmentID = 1 },
                new Project { ProjectID = 3, ProjectName = "Project C", DepartmentID = 2 },
                new Project { ProjectID = 4, ProjectName = "Project D", DepartmentID = 2 },
                new Project { ProjectID = 5, ProjectName = "Project E", DepartmentID = 3 }
            };

            return (employees, departments, projects);
        }
        
        static void GroupByAndAggregate(List<Employee> employees, List<Department> departments, List<Project> projects)
        {
            //The result of the GroupBy method is an IEnumerable < IGrouping < TKey, TElement >> where TKey represents the type of the key(in this case, int for the department ID) and TElement represents the type of the elements in the group(in this case, Employee). Each group in the result has a key that corresponds to the department ID.
            var employeesByDepartment = employees.GroupBy(e => e.DepartmentID) // Group employees by their department
                .Select(g => new
                {
                    DepartmentID = g.Key, // Get the department ID from the group key
                    AverageSalary = g.Average(e => e.Salary), // Calculate the average salary of employees in the department
                    TotalSalary = g.Sum(e => e.Salary) // Calculate the total salary of employees in the department
                });

            // Order the departments by total salary in descending order and select the department with the highest salary
            var departmentWithHighestSalary = employeesByDepartment.OrderByDescending(d => d.TotalSalary).First();

            var projectsByDepartment = projects.GroupBy(p => p.DepartmentID) // Group the projects by the department
                .Select(g => new
                {
                    DepartmentID = g.Key, // Get the department ID from the group key
                    ProjectCount = g.Count() // Get the total number of projects in each department
                });

            Console.WriteLine("Average and total salary by department: ");
            // ":C" formats numbers as currency, e.g., 1000 becomes $1,000.00
            foreach (var dept in employeesByDepartment)
            {
                // Retrieving the department name for the current department ID from the departments list using the FirstOrDefault method.
                var departmentName = departments.FirstOrDefault(d => d.DepartmentID == dept.DepartmentID).DepartmentName;
                
                Console.WriteLine($"Department: {departmentName}, Average salary: {dept.AverageSalary:C}, Total salary: {dept.TotalSalary:C}"); 
            }

            Console.WriteLine("\nDepartment with the highest total salary:");
            var topSalaryDepartmentName = departments.FirstOrDefault(d => d.DepartmentID == departmentWithHighestSalary.DepartmentID).DepartmentName;
            Console.WriteLine($"Department: {topSalaryDepartmentName}, Total salary: {departmentWithHighestSalary.TotalSalary:C}");

            Console.WriteLine("\nTotal number of projects in each department:");
            foreach (var proj in projectsByDepartment)
            {
                var deptName = departments.FirstOrDefault(d => d.DepartmentID == proj.DepartmentID).DepartmentName;
                Console.WriteLine($"Department: {deptName}, Number of projects: {proj.ProjectCount}");
            }
        }
 
        static void PerformJoins(List<Employee> employees, List<Department> departments, List<Project> projects)
        {
            Console.WriteLine("\n\nEmployee details with department and project:");

            // Combine employees, departments, and projects by DepartmentID
            var joinedResult = employees
                // Join employees with departments
                .Join(
                    departments, // The collection to join with
                    e => e.DepartmentID, // A function that selects the key from the outer collection
                    d => d.DepartmentID, // A function that selects the key from the inner collection
                                         // Creates an anonymous object that includes all properties from both the Employee and Department objects
                    (e, d) => new { e, d }// A function that creates a new result object from the joined data
                )
                .GroupBy(ed => ed.d.DepartmentName) // Group by department name
                .Select(g => new    // Create a new anonymous object for each group
                {
                    DepartmentName = g.Key, // Department name is the key of the group
                    Employees = g
                        .Select(ed => ed.e) // Selects the employee object from the joined data
                        .Distinct(), // Distinct list of employees in the department
                    ProjectNames = projects
                        // Filters the projects based on the department ID of the first department in the current group. It uses the Where method to iterate through the projects collection and select only the projects where the DepartmentID matches the DepartmentID of the first department in the current group. This ensures that only the projects belonging to the current department are included in the result. The filtered projects are then used to retrieve the project names in the subsequent Select statement.
                        .Where(p => p.DepartmentID == g.First().d.DepartmentID)
                        .Select(p => p.ProjectName) // Select project names
                });

            foreach (var result in joinedResult)
            {
                Console.WriteLine($"Department: {result.DepartmentName}");

                Console.WriteLine("Employees:");
                foreach (var employee in result.Employees)
                {
                    Console.WriteLine($"- {employee.FirstName} {employee.LastName}");
                }

                Console.WriteLine("Projects:");
                foreach (var projectName in result.ProjectNames)
                {
                    Console.WriteLine($"- {projectName}");
                }

                Console.WriteLine();
            }
        }

        static void FilterAndSelect(List<Employee> employees, List<Project> projects, decimal salaryThreshold)
        {
            Console.WriteLine($"\nEmployees earning more than {salaryThreshold:C}:");

            var filteredEmployees = employees
                .Where(employee => employee.Salary > salaryThreshold) // Filter employees based on salary threshold
                .Join(
                    projects,
                    employee => employee.DepartmentID,
                    project => project.DepartmentID,
                    (employee, project) => new
                    {
                        employee.FirstName,
                        employee.LastName,
                        project.ProjectName
                    }) // Join employees with projects based on department ID
                .GroupBy(e => new { e.FirstName, e.LastName }) // Group employees by their first and last name
                .Select(group => new
                {
                    EmployeeName = group.Key.FirstName + " " + group.Key.LastName, // Create a new anonymous object with employee name
                    Projects = string.Join(", ", group.Select(g => g.ProjectName)) // Aggregate project names using string.Join
                }); // Select the employee name and aggregated project names for each group

            foreach (var employee in filteredEmployees)
            {
                Console.WriteLine($"- Employee: {employee.EmployeeName}, Project(s): {employee.Projects}");
            }
        }
    }
}