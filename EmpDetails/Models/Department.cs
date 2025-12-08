namespace EmpDetails.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        // Navigation property
        public ICollection<Employee> Employees { get; set; }

    }
}
