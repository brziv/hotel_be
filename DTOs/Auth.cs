namespace hotel_be.DTOs
{
    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterUser
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
    }
    public class RegisterStaff
    {
        public Guid EmployeeId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        public required string UserName { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
