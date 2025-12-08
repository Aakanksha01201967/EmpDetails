using EmpDetails.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmpDetails.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System.Linq;

namespace EmpDetails.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            // Sorting Parameters
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.SalarySortParm = sortOrder == "Salary" ? "salary_desc" : "Salary";

            // Search filter
            ViewBag.CurrentFilter = searchString;

            // Base query
            var employeesQuery = _context.Employees
                .Include(e => e.Department)
                .AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(searchString))
            {
                employeesQuery = employeesQuery.Where(e =>
                    e.Name.Contains(searchString) ||
                    e.Email.Contains(searchString) ||
                    e.State.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    employeesQuery = employeesQuery.OrderByDescending(e => e.Name);
                    break;
                case "Email":
                    employeesQuery = employeesQuery.OrderBy(e => e.Email);
                    break;
                case "email_desc":
                    employeesQuery = employeesQuery.OrderByDescending(e => e.Email);
                    break;
                case "Salary":
                    employeesQuery = employeesQuery.OrderBy(e => e.Salary);
                    break;
                case "salary_desc":
                    employeesQuery = employeesQuery.OrderByDescending(e => e.Salary);
                    break;
                default:
                    employeesQuery = employeesQuery.OrderBy(e => e.Name);
                    break;
            }

            // Project into ViewModel
            var employeesVMList = await employeesQuery
                .Select(e => new EmployeeDepartmentViewModel
                {
                    EmpID = e.EmpID,
                    Name = e.Name,
                    Email = e.Email,
                    Salary = e.Salary,
                    State = e.State,
                    DepartmentName = e.Department.DepartmentName
                })
                .ToListAsync();

            return View(employeesVMList);
        }

        ////public async Task<IActionResult> Index(string sortOrder, string searchString)
        ////{
        ////    //Sorting Parameters
        ////    ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ////    ViewBag.EmailSortParm = sortOrder == "Email" ? "emai_desc" : "Email";
        ////    ViewBag.SalarySortParm = sortOrder == "Salary" ? "salay_desc" : "Salary";

        ////    //Search filter
        ////    ViewBag.CurrentFilter = searchString;

        ////    //var employees = from e in _context.Employees.Include(e => e.Department)
        ////    //                select e;


        ////    var employeesQuery = _context.Employees
        ////        .Include(e => e.Department)
        ////        .AsQueryable();

        ////    //Searching
        ////    if (!String.IsNullOrEmpty(searchString))
        ////    {
        ////        employees = employees.Where(e => e.Name.Contains(searchString)
        ////                                         || e.Email.Contains(searchString)
        ////                                         || e.State.Contains(searchString));
        ////    }

        ////    switch(sortOrder)
        ////    {
        ////        case "name_desc":
        ////            employeesQuery = employeesQuery.OrderByDescending(e => e.Name);
        ////            break;
        ////        case "Email":
        ////            employeesQuery = employeesQuery.OrderBy(e => e.Email);
        ////            break;
        ////        case "email_desc":
        ////            employeesQuery = employeesQuery.OrderByDescending(e => e.Email);
        ////            break;
        ////        case "Salary":
        ////            employeesQuery = employeesQuery.OrderBy(e => e.Salary);
        ////            break;
        ////        case "salary_desc":
        ////            employeesQuery = employeesQuery.OrderByDescending(e => e.Salary);
        ////            break;
        ////        default:
        ////            employeesQuery = employeesQuery.OrderBy(e => e.Name);
        ////            break;

        ////    }
        ////    var employeesVMList = await employeesQuery
        ////        .Select(e => new EmployeeDepartmentViewModel
        ////        {
        ////            EmpID = e.EmpID,
        ////            Name = e.Name,
        ////            Email = e.Email,
        ////            Salary = e.Salary,
        ////            State = e.State,
        ////            DepartmentName = e.Department.DepartmentName
        ////        })
        ////        .ToListAsync();
        ////    //var employees = await _context.Employees
        ////    //    .Include(e => e.Department)
        ////    //    .Select(e => new EmployeeDepartmentViewModel
        ////    //    {
            //        EmpID = e.EmpID,
            //        Name = e.Name,
            //        Email = e.Email,
            //        Salary = e.Salary,
            //        State = e.State,
            //        DepartmentName = e.Department.DepartmentName
            //    })
            //    .ToListAsync();

        ////    return View(employeesVMList);

        ////}


        //Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeDepartmentViewModel vm)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }
           
                var employee = new Employee
                {
                    Name = vm.Name,
                    Email = vm.Email,
                    Salary = vm.Salary,
                    State = vm.State,
                    DepartmentID  = vm.DepartmentID
                };

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewBag.Departments= new SelectList(_context.Departments, "DepartmentID", "DepartmentName", vm.DepartmentID);
            return View(vm);

        }

        public async Task<IActionResult> Edit(int? id)
        { 
            if (id == null) return NotFound();
            var employee = await _context.Employees.FindAsync(id);
            if(employee == null)return NotFound();

            var vm = new EmployeeDepartmentViewModel
            {
                EmpID = employee.EmpID,
                Name = employee.Name,
                Email= employee.Email,
                Salary= employee.Salary,
                State = employee.State,
                DepartmentID = employee.DepartmentID
            };
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            return View(vm);

        }
        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeDepartmentViewModel vm)
        {
            if (id != vm.EmpID) return NotFound();

           
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null) return NotFound();

                employee.Name = vm.Name;
                employee.Email = vm.Email;
                employee.Salary = vm.Salary;
                employee.State = vm.State;
                employee.DepartmentID = vm.DepartmentID;

                _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", vm.DepartmentID);
            return View(vm);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmpID == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }

}

