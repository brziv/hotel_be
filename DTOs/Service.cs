namespace hotel_be.DTOs
{
    public class ServiceGetDto
    {
        public Guid SServiceId { get; set; }
        public required string SServiceName { get; set; }
        public decimal SServiceCostPrice { get; set; }
        public decimal SServiceSellPrice { get; set; }
        public required string GoodsInfo { get; set; }
        public List<ServiceGoodDto> ServiceGoods { get; set; } = new List<ServiceGoodDto>();
    }
    public class ServiceRequestDto
    {
        public Guid SServiceId { get; set; }
        public required string SServiceName { get; set; }
        public decimal SServiceCostPrice { get; set; }
        public decimal SServiceSellPrice { get; set; }
        public List<ServiceGoodDto> ServiceGoods { get; set; } = new List<ServiceGoodDto>();
    }

    public class ServiceGoodDto
    {
        public Guid SgGoodsId { get; set; }
        public int SgQuantity { get; set; }
    }
}
