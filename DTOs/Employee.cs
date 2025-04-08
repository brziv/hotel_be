namespace hotel_be.DTOs
{
    public class UpdateEmployeeDto
    {
        public Guid EEmployeeId { get; set; }
        public required string EFirstName { get; set; }
        public required string ELastName { get; set; }
        public required string EEmail { get; set; }
        public required string EPhoneNumber { get; set; }
        public required string EPosition { get; set; }
        public decimal ESalary { get; set; }
    }
}
