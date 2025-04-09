namespace hotel_be.DTOs
{
    public class PackageGetDto
    {
        public Guid SpPackageId { get; set; }
        public required string SpPackageName { get; set; }
        public decimal SServiceCostPrice { get; set; }
        public decimal SServiceSellPrice { get; set; }
        public required string ProductsInfo { get; set; }
        public List<PackageDetailDto> PackageDetails { get; set; } = new List<PackageDetailDto>();
    }
    public class PackageRequestDto
    {
        public Guid SpPackageId { get; set; }
        public required string SpPackageName { get; set; }
        public decimal SServiceCostPrice { get; set; }
        public decimal SServiceSellPrice { get; set; }
        public List<PackageDetailDto> PackageDetails { get; set; } = new List<PackageDetailDto>();
    }

    public class UsedServiceGetDto
    {
        public Guid SpPackageID { get; set; }
        public required string SpPackageName { get; set; }
        public decimal SServiceSellPrice { get; set; }
        public required string ProductsInfo { get; set; }
        public required int Quantity { get; set; }
        public List<PackageDetailDto> PackageDetails { get; set; } = new List<PackageDetailDto>();
    }

    public class PackageDto
    {
        public Guid PackageId { get; set; }
        public int Quantity { get; set; }
    }

    public class PackageDetailDto
    {
        public Guid PdProductId { get; set; }
        public int PdQuantity { get; set; }
    }
    public class ApproveServiceRequestDto
    {
        public Guid BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Guid> BookingServiceIds { get; set; } = new List<Guid>();
    }

}
