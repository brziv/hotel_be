namespace hotel_be.DTOs
{
    public class UpdateGuestDto
    {
        public Guid GGuestId { get; set; }
        public required string GFirstName { get; set; }
        public required string GLastName { get; set; }
        public required string GEmail { get; set; }
        public required string GPhoneNumber { get; set; }
    }
}
