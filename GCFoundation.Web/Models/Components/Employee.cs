namespace GCFoundation.Web.Models.Components
{
    /// <summary>
    /// Represents a sample employee object.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Id of an employee.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of an employee.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Name of the department of the employee.
        /// </summary>
        public string Department { get; set; } = string.Empty;
    }
}