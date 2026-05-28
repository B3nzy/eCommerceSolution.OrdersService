namespace eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;

public class GetOrderByIdResponse
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalBill { get; set; }
    public List<OrderItemResponse> OrderItems { get; set; }
}

public class OrderItemResponse
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}