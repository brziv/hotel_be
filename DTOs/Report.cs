namespace hotel_be.DTOs
{
    public class BookingTotalDto
    {
        public required string RoomNumber { get; set; }
        public int TotalBookings { get; set; }
        public int Period { get; set; }
    }

    public class BookingStatusDto
    {
        public required string BookingStatus { get; set; }
        public int RoomCount { get; set; }
        public int Period { get; set; }
    }

    public class OccupancyRateDto
    {
        public required string Floor { get; set; }
        public int Period { get; set; }
        public decimal TotalOccupiedRoomDays { get; set; }
        public decimal OccupancyRate { get; set; }
    }

    public class RoomTimelineDto
    {
        public string? RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public class RevenueDto
    {
        public decimal Amount { get; set; }
        public int Period { get; set; }
    }

    public class InventoryStockDto
    {
        public required string ProductName { get; set; }
        public int StockLevel { get; set; }
        public string? Unit { get; set; }
    }

    public class TrendRoomTypeDto
    {
        public required string RoomType { get; set; }
        public int BookingCount { get; set; }
        public int Period { get; set; }
    }

    public class TrendServiceDto
    {
        public required string PackageName { get; set; }
        public int UsageCount { get; set; }
        public int Period { get; set; }
    }
}
