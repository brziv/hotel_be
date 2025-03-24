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

    public class PackageDetailDto
    {
        public Guid PdProductId { get; set; }
        public int PdQuantity { get; set; }
    }
}
