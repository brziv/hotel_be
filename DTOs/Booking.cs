namespace hotel_be.DTOs
{
    public class BookingRoomsDTO
    {
        public Guid RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public class BookingRequestDTO
    {
        public Guid GuestId { get; set; }
        public List<BookingRoomsDTO>? BRdto { get; set; }
    }

    public class BookingInAdvanceRequestDTO
    {
        public Guid GuestId { get; set; }
        public decimal Deposit { get; set; }
        public List<BookingRoomsDTO>? BRdto { get; set; }
    }

}
