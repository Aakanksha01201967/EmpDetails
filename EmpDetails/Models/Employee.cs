namespace EmpDetails.Models
{
    public class Employee
    {
        public int EmpID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public string State { get; set;}

        // Foreign key
        public int DepartmentID { get; set; }

        // Navigation property
        public Department Department { get; set; }
    }
}
