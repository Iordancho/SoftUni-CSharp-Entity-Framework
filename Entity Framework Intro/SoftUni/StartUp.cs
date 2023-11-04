using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetEmployeesFullInformation(context));
            Console.WriteLine(GetEmployeesWithSalaryOver50000(context));
            Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));
            Console.WriteLine(AddNewAddressToEmployee(context));
            Console.WriteLine(GetEmployeesInPeriod(context));
            Console.WriteLine(GetAddressesByTown(context));
            Console.WriteLine(GetEmployee147(context));
            Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));
            Console.WriteLine(GetLatestProjects(context));
            Console.WriteLine(IncreaseSalaries(context));
            Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
            Console.WriteLine(DeleteProjectById(context));
            Console.WriteLine(RemoveTown(context));
        }

        //PROBLEM 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList();

            var result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));
            return result;
        }
        //PROBLEM 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                }).ToList();

            var result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));
            
            return result;
        }
        //PROBLEM 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                }).ToList();
            var result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"));
            
            return result;
        }
        //PROBLEM 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");
            
            employee.Address = address;
            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.Address.AddressId)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .Take(10);
            
            var result = string.Join(Environment.NewLine,
                employees.Select(e => e.AddressText));

            return result;

        }
        //PROBLEM 7
        public static string GetEmployeesInPeriod(SoftUniContext context)
         {
             var employees = context.Employees
                 .Select(e => new
                 {
                     FirstName = e.FirstName,
                     LastName = e.LastName,
                     ManagerFirstName = e.Manager.FirstName,
                     ManagerLastName = e.Manager.LastName,
                     Projects = e.EmployeesProjects
                         .Select(ep => new
                         {
                             ProjectName = ep.Project.Name,
                             ProjectStartDate = ep.Project.StartDate,
                             ProjectEndDate = ep.Project.EndDate
                         })
                         .Where(p => p.ProjectStartDate.Year >= 2001 && p.ProjectStartDate.Year <= 2003)
                         .ToList()
                 })
                 .Take(10)
                 .ToList();
        
             StringBuilder result = new StringBuilder();
        
             foreach (var employee in employees)
             {
                 result.AppendLine(
                     $"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");
                 
                 
                 foreach (var project in employee.Projects)
                 {
                     var endDate = project.ProjectEndDate.HasValue
                         ? project.ProjectEndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                         : "not finished";
                     result.AppendLine($"--{project.ProjectName} - {project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt")} - {endDate}");
                 }
             }
        
             return result.ToString().Trim();
         }
        //PROBLEM 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count())
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeeCount = a.Employees.Count()
                }).ToList();

             return string.Join(Environment.NewLine,
                addresses.Select(a => $"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees"));
        }
        //PROBLEM 9
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(ep => ep.Project.Name)
                        .OrderBy(pn => pn)
                        .ToList()
                })
                .Single();
            

            StringBuilder result = new StringBuilder();

            result.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            result.AppendLine(string.Join(Environment.NewLine, employee.Projects));
            
            return result.ToString().Trim();
        }
        //PROBLEM 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerName = $"{d.Manager.FirstName} {d.Manager.LastName}",
                    Employees = d.Employees
                        .Select(e => new
                        {
                            EmployeeFirstName = e.FirstName,
                            EmployeeLastName = e.LastName,
                            EmployeeJobTitle = e.JobTitle
                        })
                        .OrderBy(e => e.EmployeeFirstName)
                        .ThenBy(e => e.EmployeeLastName)
                        .ToList()
                });

            StringBuilder result = new StringBuilder();

            foreach (var d in departments)
            {
                result.AppendLine($"{d.DepartmentName} - {d.ManagerName}");

                foreach (var e in d.Employees)
                {
                    result.AppendLine($"{e.EmployeeFirstName} {e.EmployeeLastName} - {e.EmployeeJobTitle}");
                }
            }

            return result.ToString().Trim();
        }
        //PROBLEM 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    ProjectName = p.Name,
                    ProjectDescription = p.Description,
                    ProjectStartDate = p.StartDate
                })
                .OrderBy(p => p.ProjectName);

            StringBuilder result = new StringBuilder();

            foreach (var p in projects)
            {
                result.AppendLine(p.ProjectName);
                result.AppendLine(p.ProjectDescription);
                result.AppendLine(p.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            return result.ToString().Trim();
        }
        //PROBLEM 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employees)
            {
                e.Salary *= 1.12m;
            }

            return string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} (${e.Salary:f2})"));
        }
        //PROBLEM 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            return string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));
        }
        //PROBLEM 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeesProjectsToDelete = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2);
            var projectToDelete = context.Projects.Find(2);

            foreach (var ep in employeesProjectsToDelete)
            {
                context.EmployeesProjects.Remove(ep);
            }
            context.Projects.Remove(projectToDelete);
            
            context.SaveChanges();

            StringBuilder result = new StringBuilder();

            context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList()
                .ForEach(p => result.AppendLine(p));

            return result.ToString().Trim();
        }
        //PROBLEM 15
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .First(t => t.Name == "Seattle");

            var addressesToDel = context
                .Addresses
                .Where(s => s.TownId == town.TownId);
            int addressesCount = addressesToDel.Count();

            var employees = context.Employees
                .Where(e => addressesToDel.Any(a => a.AddressId == e.AddressId));

            foreach (var e in employees)
            {
                e.AddressId = null;
            }

            foreach (var a in addressesToDel)
            {
                context.Addresses.Remove(a);
            }

            context.Towns.Remove(town);

            context.SaveChanges();

            return $"{addressesCount} addresses in {town.Name} were deleted";
        }
    }
}