namespace hotel_be.DTOs
{
    public class UpdateProductDto
    {
        public Guid PProductId { get; set; }
        public string PProductName { get; set; } = null!;
        public string? PCategory { get; set; }
        public int? PQuantity { get; set; }
        public string? PUnit { get; set; }
        public decimal PCostPrice { get; set; }
        public decimal PSellingPrice { get; set; }
        public string PCurrency { get; set; } = null!;
        public bool? PIsService { get; set; }
        public bool IsActive { get; set; }
    }
}
