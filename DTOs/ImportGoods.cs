namespace hotel_be.DTOs
{
    public class BaseImportGoodsDto
    {
        public Guid IgdId { get; set; }
        public int IgdQuantity { get; set; }
        public decimal IgdCostPrice { get; set; }
    }

    public class ImportGoodsDetailDto : BaseImportGoodsDto
    {
        public string? PProductName { get; set; }
        public string? IgSupplier { get; set; }
        public DateTime? IgImportDate { get; set; }
    }

    public class ImportGoodsByImportDto : BaseImportGoodsDto
    {
        public Guid IgdGoodsId { get; set; }
        public string? PProductName { get; set; }
    }

    public class ImportGoodsByGoodDto : BaseImportGoodsDto
    {
        public Guid IgdImportId { get; set; }
        public Guid IgdGoodsId { get; set; }
        public DateTime? IgImportDate { get; set; }
        public string? IgSupplier { get; set; }
    }
}
