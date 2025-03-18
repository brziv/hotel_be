namespace hotel_be.DTOs
{
    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
