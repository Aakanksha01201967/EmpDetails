using System.ComponentModel.DataAnnotations;

namespace EmpDetails.Models
{
    public class EmployeeDepartmentViewModel
    {
        public int EmpID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Name can only contain letters, spaces, hyphens, and apostrophes")]
        public string Name {  get; set; }
       

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        public string Email {  get; set; }

        [Range(1000, 100000, ErrorMessage = "Salary must be between 1,000 and 100,000")]
        public decimal Salary {  get; set; }


        [Required(ErrorMessage = "State is required")]
        [StringLength(30, ErrorMessage = "State name cannot exceed 30 characters")]
        public string State {  get; set; }

        [Required]
        public int DepartmentID {  get; set; }


        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department name cannot exceed 50 characters")]
        public string DepartmentName {  get; set; }
    }
}
